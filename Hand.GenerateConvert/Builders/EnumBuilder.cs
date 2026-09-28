using Hand.Converters;
using Hand.Enums;
using Hand.Enums.Builders;
using Hand.Enums.Bundles;
using Hand.Members;
using Hand.Reflection;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 枚举类型构造器
/// </summary>
/// <param name="builder"></param>
/// <param name="compilation"></param>
public class EnumBuilder(ConvertBuilder builder, Compilation compilation)
{
    #region 配置
    private readonly ConvertBuilder _builder = builder;
    private readonly Compilation _compilation = compilation;
    private readonly EnumBundleBuilder _bundles = new(compilation);
    #endregion
    /// <summary>
    /// 转化为枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ToEnum(ITypeSymbolInfo source, EnumTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Enum => EnumToEnum((EnumTypeInfo)source, dest),
            //TypeSymbolKind.Enumeration => EnumerationToEnum((EnumerationTypeInfo)source, dest),
            TypeSymbolKind.Primitive => PrimitiveToEnum((PrimitiveTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToEnum((EntityPropertyTypeInfo)source, dest),
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
    public ISyntaxConverter? EnumToEnum(EnumTypeInfo source, EnumTypeInfo dest)
    {
        (var convertToInfo, var converter) = ConvertBuilder.GetConverter(_compilation, source.Symbol, dest.Symbol);
        if (converter is not null)
            return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
        converter = EnumToEnum(source, dest, _bundles.Get(source), _bundles.Get(dest), convertToInfo);
        if (converter is null)
            return null;
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    #region ToEnum
    /// <summary>
    /// 基础类型转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? PrimitiveToEnum(PrimitiveTypeInfo source, EnumTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        (var convertToInfo, var converter) = ConvertBuilder.GetConverter(_compilation, sourceSymbol, dest.Symbol);
        if (converter is not null)
            return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
        // string转枚举
        if (sourceSymbol.IsString())
            return EnumFromString(dest, _bundles.Get(dest), convertToInfo);
        if (sourceSymbol.IsNumericType())
            return ConvertBuilder.CheckSource(new CastConverter(dest), source.IsNullable, dest);
        var underConverter = _builder.PrimitiveToPrimitive(source, dest.ElementInfo);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        converter = new CompositeConverter(underConverter, new CastConverter(dest));
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 复杂类型转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ComplexToEnum(ComplexTypeInfo source, EnumTypeInfo dest)
    {
        var converter = _builder.ComplexToOther(source, dest);
        if (converter is not null)
            return converter;
        var under = dest.ElementInfo;
        var underConverter = _builder.ComplexToOther(source, under);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举
        converter = new CompositeConverter(underConverter, new CastConverter(dest));
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 其他类型转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? OtherToEnum(ITypeSymbolInfo source, EnumTypeInfo dest)
    {
        var converter = _builder.GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var underInfo = dest.ElementInfo;
        // 尝试系统转化
        var underConverter = _builder.GetCommonConversion(underInfo, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举
        converter = new CompositeConverter(underConverter, new CastConverter(dest));
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    #endregion
    #region FromEnum
    /// <summary>
    /// 枚举转基础类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumToPrimitive(EnumTypeInfo source, PrimitiveTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = ConvertBuilder.GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
        // 含枚举转string和数字
        converter = _builder.GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        (_, converter) = ConvertBuilder.GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
        return null;
    }
    /// <summary>
    /// 枚举转化为复杂类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumToComplex(EnumTypeInfo source, ComplexTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = ConvertBuilder.GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
        converter = _builder.OtherToComplex(source, dest);
        if (converter is not null)
            return converter;
        var under = source.ElementInfo;
        var underConverter = _builder.OtherToComplex(under, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为复杂类型
        converter = new CompositeConverter(new CastConverter(under), underConverter);
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 枚举转集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumToCollection(EnumTypeInfo source, ICollectionTypeInfo dest)
    {
        var converter = _builder.OtherToCollection(source, dest);
        if (converter is not null)
            return converter;
        var under = source.ElementInfo;
        var underConverter = _builder.OtherToCollection(under, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为复杂类型
        converter = new CompositeConverter(new CastConverter(under), underConverter);
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 枚举转未知类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumToUnknow(EnumTypeInfo source, ITypeSymbolInfo dest)
    {
        var converter = _builder.GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var underInfo = source.ElementInfo;
        // 尝试系统转化
        var underConverter = _builder.GetCommonConversion(underInfo, dest);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为未知类型
        converter = new CompositeConverter(new CastConverter(underInfo), underConverter);
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    #endregion
    #region EntityEnum
    /// <summary>
    /// 实体属性转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EntityToEnum(EntityPropertyTypeInfo source, EnumTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = ConvertBuilder.GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
        var underConverter = _builder.EntityToPrimitive(source, dest.ElementInfo);
        if (underConverter is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        converter = new CompositeConverter(underConverter, new CastConverter(dest));
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 枚举转实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumToEntity(EnumTypeInfo source, EntityPropertyTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = ConvertBuilder.GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
        var element = dest.ElementInfo;
        var underConverter = EnumToPrimitive(source, element);
        if (underConverter is null)
            return null;
        var entityConverter = ConvertBuilder.ConstructorBySingle(element.Symbol, dest);
        if (entityConverter is null)
            return null;
        // 先转为子类型, 再转为实体属性
        converter = new CompositeConverter(underConverter, entityConverter);
        return ConvertBuilder.CheckSource(converter, source.IsNullable, dest);
    }
    #endregion
    /// <summary>
    /// Enum转其他类型
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public ISyntaxConverter? FromEnum(EnumTypeInfo sourceInfo, ITypeSymbolInfo destInfo, ConvertSourceInfo convertToInfo)
    {
        var enumBundle = _bundles.Get(sourceInfo);
        var underType = sourceInfo.Element;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, destType))
        //    return new CastConverter(destType.ToSyntax());

        // 枚举转string
        // 正常情况下该逻辑应该不会命中,之前ConvertBuilder已经处理ToString了
        //if (destType.IsString())
        //    return ToStringConverter.Instance;
        var destSymbol = destInfo.Symbol;
        if (destInfo.Kind == TypeSymbolKind.Enum && destInfo is EnumTypeInfo enumType)
        {
            var destBundle = _bundles.Get(enumType);
            // 枚举转枚举
            if (destBundle is not null)
                return EnumToEnum(sourceInfo, enumType, enumBundle, destBundle, convertToInfo);
        }
        var original = _builder.Get(underType, destSymbol);
        if (original is null)
            return null;
        // 先转为底层类型, 再转目标类型
        return new CompositeConverter(new CastConverter(sourceInfo.ElementInfo), original);
    }
    /// <summary>
    ///其他类型转Enum
    /// </summary>
    /// <param name="sourceInfo"></param>
    /// <param name="destInfo"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public ISyntaxConverter? ToEnum(ITypeSymbolInfo sourceInfo, EnumTypeInfo destInfo, ConvertSourceInfo convertToInfo)
    {
        var enumBundle = _bundles.Get(destInfo);
        var sourceSymbol = sourceInfo.Symbol;
        // string转枚举
        if (sourceSymbol.IsString())
            return EnumFromString(destInfo, enumBundle, convertToInfo);
        var underType = destInfo.Element;
        // 底层类型直接转
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (SymbolTypeDescriptor.CheckEquals(underType, sourceType))
        //    return new CastConverter(enumType.ToSyntax());

        // 使用Enum.ToObject转化,支持整数类型
        // 正常情况下该逻辑应该不会命中, 之前ConvertBuilder的ClassifyCommonConversion逻辑会覆盖
        //if (EnumToObjectConverter.CheckSupported(sourceType.SpecialType))
        //    return new EnumToObjectConverter(enumType.ToSyntax());

        var compatible = _builder.Get(sourceSymbol, underType);
        if (compatible is null)
            return null;
        // 先转为底层类型, 再转为枚举类型
        return new CompositeConverter(compatible, new CastConverter(destInfo));
    }
    /// <summary>
    /// string转Enum
    /// </summary>
    /// <param name="enumInfo"></param>
    /// <returns></returns>
    public ISyntaxConverter EnumFromString(EnumTypeInfo enumInfo)
    {
        var enumInfoSymbol = enumInfo.Symbol;
        var sourceSymbol = _compilation.GetSpecialType(SpecialType.System_String);
        (var convertToInfo, var converter) = ConvertBuilder.GetConverter(_compilation, sourceSymbol, enumInfoSymbol);
        if (converter is not null)
            return converter;
        return EnumFromString(enumInfo, _bundles.Get(enumInfo), convertToInfo);
    }
    /// <summary>
    /// string转Enum
    /// </summary>
    /// <param name="enumInfo"></param>
    /// <param name="bundle"></param>
    /// <param name="convertToInfo"></param>
    /// <returns></returns>
    public ISyntaxConverter EnumFromString(EnumTypeInfo enumInfo, IEnumBundle bundle, ConvertSourceInfo convertToInfo)
    {
        // 枚举特殊成员
        var members = bundle.Fields
            .Where(field => !string.IsNullOrWhiteSpace(field.Member))
            .ToArray();
        // 默认通过Enum.Parse转化
        if (members.Length == 0)
            return EnumParseConverter.Create(enumInfo, true);

        // 扩展类非partial,无法扩展,直接使用Enum.Parse转化
        if (!convertToInfo.IsPartial)
            return EnumParseConverter.Create(enumInfo, true);

        var typeInfo = convertToInfo.TypeInfo;
        var methodName = convertToInfo.MethodInfo.Name;
        if (bundle.HasFlag && bundle is FlagEnumBundle flagBundle)
        {
            var source = new FlagEnumFromMemberStringSource(_compilation, enumInfo, methodName, members);
            _builder.AddSource(source, typeInfo);
        }
        else
        {
            var source = new EnumFromMemberStringSource(_compilation, enumInfo, methodName, members);
            _builder.AddSource(source, typeInfo);
        }
        //return new StaticMethodConverter(typeInfo.Type.Access(methodName));
        return convertToInfo.Create();
    }
    private ISyntaxConverter? EnumToEnum(EnumTypeInfo sourceInfo, EnumTypeInfo destInfo, IEnumBundle sourceBundle, IEnumBundle destBundle, ConvertSourceInfo convertToInfo)
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
        _builder.AddSource(source, typeInfo);
        return convertToInfo.Create();
        //return new StaticMethodConverter(typeInfo.Type.Access(methodName));
    }
}
