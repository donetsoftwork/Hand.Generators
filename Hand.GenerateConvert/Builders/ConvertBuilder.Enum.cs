using Hand.Converters;
using Hand.Enums;
using Hand.Members;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 枚举转化构造器
/// </summary>
public partial class ConvertBuilder
{
    /// <summary>
    /// 转化为数组
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? ToEnum(ITypeSymbolInfo source, EnumTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Enum => EnumToEnum((EnumTypeInfo)source, dest),
            TypeSymbolKind.Primitive => PrimitiveToEnum((PrimitiveTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToEnum((EntityTypeInfo)source, dest),
            TypeSymbolKind.Complex => ComplexToEnum((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Generic => ComplexToEnum((ComplexTypeInfo)source, dest),
            _ => OtherToEnum(source, dest),
        };
    }
    /// <summary>
    /// 枚举转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? EnumToEnum(EnumTypeInfo source, EnumTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (var convertToInfo, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        converter = EnumToEnum(source, dest, _bundles.Get(sourceSymbol), _bundles.Get(destSymbol), convertToInfo);
        if (converter is null)
            return null;
        return CheckSource(converter, source.IsNullable, dest);
    }
    #region ToEnum
    /// <summary>
    /// 基础类型转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? PrimitiveToEnum(PrimitiveTypeInfo source, EnumTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (var convertToInfo, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        // string转枚举
        if (sourceSymbol.IsString())
            return EnumFromString(dest, _bundles.Get(destSymbol), convertToInfo);
        if (sourceSymbol.IsNumericType())
            return CheckSource(new CastConverter(dest), source.IsNullable, dest);
        var underConverter = PrimitiveToPrimitive(source, dest.ElementInfo);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        converter = new CompatibleConverter(underConverter, new CastConverter(dest));
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 复杂类型转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? ComplexToEnum(ComplexTypeInfo source, EnumTypeInfo dest)
    {
        var converter = ComplexToOther(source, dest);
        if (converter is not null)
            return converter;
        var under = dest.ElementInfo;
        var underConverter = ComplexToOther(source, under);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举
        converter = new CompatibleConverter(underConverter, new CastConverter(dest));
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 其他类型转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? OtherToEnum(ITypeSymbolInfo source, EnumTypeInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var underInfo = dest.ElementInfo;
        // 尝试系统转化
        var underConverter = GetCommonConversion(underInfo, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举
        converter = new CompatibleConverter(underConverter, new CastConverter(dest));
        return CheckSource(converter, source.IsNullable, dest);
    }
    #endregion
    #region FromEnum
    /// <summary>
    /// 枚举转基础类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? EnumToPrimitive(EnumTypeInfo source, PrimitiveTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        // 含枚举转string和数字
        converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        (_, converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        return null;
    }
    /// <summary>
    /// 枚举转化为复杂类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? EnumToComplex(EnumTypeInfo source, ComplexTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        converter = OtherToComplex(source, dest);
        if (converter is not null)
            return converter;
        var under = source.ElementInfo;
        var underConverter = OtherToComplex(under, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为复杂类型
        converter = new CompatibleConverter(new CastConverter(under), underConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    ///// <summary>
    ///// 枚举转集合
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //public IConverter? EnumToArray(EnumTypeInfo source, ArrayTypeInfo dest)
    //{
    //    var converter = OtherToArray(source, dest);
    //    if (converter is not null)
    //        return converter;
    //    var under = source.ElementInfo;
    //    var underConverter = OtherToArray(under, dest);
    //    if (underConverter is null)
    //        return null;
    //    // 先转为底层类型, 再转为复杂类型
    //    converter = new CompatibleConverter(new CastConverter(under.Symbol.ToSyntax()), underConverter);
    //    return CheckSource(converter, source.IsNullable, dest);
    //}
    /// <summary>
    /// 枚举转集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? EnumToCollection(EnumTypeInfo source, ICollectionSymbolInfo dest)
    {
        var converter = OtherToCollection(source, dest);
        if (converter is not null)
            return converter;
        var under = source.ElementInfo;
        var underConverter = OtherToCollection(under, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为复杂类型
        converter = new CompatibleConverter(new CastConverter(under), underConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 枚举转未知类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? EnumToUnknow(EnumTypeInfo source, ITypeSymbolInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var underInfo = source.ElementInfo;
        // 尝试系统转化
        var underConverter = GetCommonConversion(underInfo, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为未知类型
        converter = new CompatibleConverter(new CastConverter(underInfo), underConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    #endregion
    #region EntityEnum
    /// <summary>
    /// 实体属性转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? EntityToEnum(EntityTypeInfo source, EnumTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        var underConverter = EntityToPrimitive(source, dest.ElementInfo);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        converter = new CompatibleConverter(underConverter, new CastConverter(dest));
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 枚举转实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? EnumToEntity(EnumTypeInfo source, EntityTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        var element = dest.ElementInfo;
        var underConverter = EnumToPrimitive(source, element);
        if (underConverter is null)
            return null;
        var entityConverter = ConstructorBySingle(element.Symbol, dest);
        if (entityConverter is null)
            return null;
        // 先转为子类型, 再转为实体属性
        converter = new CompatibleConverter(underConverter, entityConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    #endregion
    /// <summary>
    /// Enum转其他类型
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public IConverter? FromEnum(EnumTypeInfo sourceInfo, ITypeSymbolInfo destInfo, ConvertSourceInfo convertToInfo)
    {
        var enumBundle = _bundles.Get(sourceInfo.Symbol);
        var underType = enumBundle.UnderType;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, destType))
        //    return new CastConverter(destType.ToSyntax());

        // 枚举转string
        // 正常情况下该逻辑应该不会命中,之前ConvertBuilder已经处理ToString了
        //if (destType.IsString())
        //    return ToStringConverter.Instance;
        var destSymbol = destInfo.Symbol;
        if (destInfo.IsEnum() && destInfo is EnumTypeInfo enumType)
        {
            var destBundle = _bundles.Get(enumType.Symbol);
            // 枚举转枚举
            if (destBundle is not null)
                return EnumToEnum(sourceInfo, enumType, enumBundle, destBundle, convertToInfo);
        }
        var original = Get(underType, destSymbol);
        if (original is null)
            return null;
        // 先转为底层类型, 再转目标类型
        return new CompatibleConverter(new CastConverter(sourceInfo.ElementInfo), original);
    }
    /// <summary>
    ///其他类型转Enum
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public IConverter? ToEnum(ITypeSymbolInfo sourceInfo, EnumTypeInfo destInfo, ConvertSourceInfo convertToInfo)
    {
        var enumBundle = _bundles.Get(destInfo.Symbol);
        var sourceSymbol = sourceInfo.Symbol;
        // string转枚举
        if (sourceSymbol.IsString())
            return EnumFromString(destInfo, enumBundle, convertToInfo);
        var underType = enumBundle.UnderType;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, sourceType))
        //    return new CastConverter(enumType.ToSyntax());

        // 使用Enum.ToObject转化,支持整数类型
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (EnumToObjectConverter.CheckSupported(sourceType.SpecialType))
        //    return new EnumToObjectConverter(enumType.ToSyntax());

        var compatible = Get(sourceSymbol, underType);
        if (compatible is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        return new CompatibleConverter(compatible, new CastConverter(destInfo));
    }
    /// <summary>
    /// string转Enum
    /// </summary>
    /// <param name="enumInfo"></param>
    /// <param name="bundle"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    private IConverter EnumFromString(EnumTypeInfo enumInfo, IEnumBundle bundle, ConvertSourceInfo convertToInfo)
    {
        // 枚举特殊成员
        var members = bundle.Fields
            .Where(field => !string.IsNullOrWhiteSpace(field.Member))
            .ToArray();
        var enumSymbol = enumInfo.Symbol;
        // 默认通过Enum.Parse转化
        if (members.Length == 0)
            return new EnumParseConverter(enumSymbol.ToSyntax(), true);

        //var enumProvider = SourceProvider.CreateByExtension(compilation, enumType);
        //var convertFromInfo = enumProvider.ConvertFrom(stringType.Name);
        //var convertFromMethod = enumProvider.GetConvertMethod(convertFromInfo.MethodInfo, stringType);
        //if(convertFromMethod is null)
        //{
        //    var stringProvider = SourceProvider.CreateByExtension(compilation, stringType);
        //    var convertToInfo = stringProvider.ConvertTo(enumType.Name);
        //    var convertToMethod = stringProvider.GetConvertMethod(convertToInfo.MethodInfo, enumType);
        //}

        //var stringProvider = SourceProvider.CreateByExtension(_compilation, stringType);
        //var convertToInfo = stringProvider.ConvertTo(enumType.Name);
        //var methodInfo = convertToInfo.MethodInfo;
        //var convertToMethod = stringProvider.GetConvertMethod(methodInfo, enumType);
        //var typeInfo = convertToInfo.TypeInfo;
        //if (convertToMethod is not null)
        //    return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
        // 扩展类非partial,无法扩展,直接使用Enum.Parse转化
        if (!convertToInfo.IsPartial)
            return new EnumParseConverter(enumSymbol.ToSyntax(), true);
        
        var typeInfo = convertToInfo.TypeInfo;
        var methodName = convertToInfo.MethodInfo.Name;
        if(bundle.HasFlag && bundle is FlagEnumBundle flagBundle)
        {
            var source = new FlagEnumFromMemberStringSource(_compilation, enumInfo, methodName, members);
            AddSource(source, typeInfo);
        }
        else
        {
            var source = new EnumFromMemberStringSource(_compilation, enumInfo, methodName, members);
            AddSource(source, typeInfo);
        }
        //return new StaticMethodConverter(typeInfo.Type.Access(methodName));
        return convertToInfo.Create();
    }
    private IConverter? EnumToEnum(EnumTypeInfo sourceInfo, EnumTypeInfo destInfo, IEnumBundle sourceBundle, IEnumBundle destBundle, ConvertSourceInfo convertToInfo)
    {
        //var methodInfo = convertToInfo.MethodInfo;        
        //if (convertToMethod is not null)
        //    return new StaticMethodConverter(typeInfo.Type.Access(convertToMethod.Name));
        // 扩展类非partial,无法扩展,不支持
        if (!convertToInfo.IsPartial)
            return null;

        var typeInfo = convertToInfo.TypeInfo;
        var methodName = convertToInfo.MethodInfo.Name;
        var source = new EnumToEnumSource(_compilation, SyntaxFactory.IdentifierName(sourceInfo.Symbol.Name), destInfo, methodName, sourceBundle, destBundle);
        AddSource(source, typeInfo);
        return convertToInfo.Create();
        //return new StaticMethodConverter(typeInfo.Type.Access(methodName));
    }
}
