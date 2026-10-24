using Hand.Converters;
using Hand.Converters.Members;
using Hand.Enumerations.Converters;
using Hand.Enums;
using Hand.Members;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp;

namespace Hand.Builders;

/// <summary>
/// 枚举类转化构造器
/// </summary>
public partial class ConvertBuilder
{
    #region ToEnumeration
    /// <summary>
    /// 转化为枚举类
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ToEnumeration(ITypeSymbolInfo source, EnumerationTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Enum => EnumToEnumeration((EnumTypeInfo)source, dest),
            TypeSymbolKind.Enumeration => EnumerationToEnumeration((EnumerationTypeInfo)source, dest),
            TypeSymbolKind.Primitive => PrimitiveToEnumeration((PrimitiveTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToEnumeration((EntityPropertyTypeInfo)source, dest),
            //TypeSymbolKind.Complex => ComplexToComplex((ComplexTypeInfo)source, dest),
            //TypeSymbolKind.Generic => ComplexToComplex((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Void => null,
            _ => OtherToComplex(source, dest),
        };
    }
    /// <summary>
    /// 枚举类转枚举类
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumerationToEnumeration(EnumerationTypeInfo source, EnumerationTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        var nameConverter = EnumerationFromName(dest);
        if (nameConverter is null)
            return null;
        return CheckSource(new CompositeConverter(MemberConverter.Name, nameConverter), source.IsNullable, dest);
    }
    /// <summary>
    /// 实体属性转枚举类
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EntityToEnumeration(EntityPropertyTypeInfo source, EnumerationTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);

        var elementConverter = PrimitiveToEnumeration(source.ElementInfo, dest);
        if (elementConverter is null)
            return null;
        // 先转为子类型, 再转为枚举类型
        converter = new CompositeConverter(MemberConverter.Original, elementConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 基础类型转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? PrimitiveToEnumeration(PrimitiveTypeInfo source, EnumerationTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        // string转枚举
        if (sourceSymbol.IsString())
            return EnumerationFromName(dest);
        if (sourceSymbol.IsLong())
            return EnumerationFromOriginal(dest);
        if (sourceSymbol.IsNumericType())
        {
            var original = dest.OriginalInfo;
            var originalConverter = Get(source, original);
            if (originalConverter is null)
                return null;
            converter = Get(original, dest);
            if (converter is null)
                return null;
            // 先转为原始类型, 再转为枚举类
            return CheckSource(new CompositeConverter(originalConverter, converter), source.IsNullable, dest);
        }
        return null;
    }
    private NameConverter? EnumerationFromName(EnumerationTypeInfo enumerationInfo)
    {
        var provider = _providerBuilder.Get(enumerationInfo);
        if (provider is null)
            return null;
        return new NameConverter(provider);
    }
    private OriginalConverter? EnumerationFromOriginal(EnumerationTypeInfo enumerationInfo)
    {
        var provider = _providerBuilder.Get(enumerationInfo);
        if (provider is null)
            return null;
        return new OriginalConverter(provider);
    }
    /// <summary>
    /// 枚举转枚举类
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumToEnumeration(EnumTypeInfo source, EnumerationTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (var convertToInfo, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);

        var nameConverter = EnumerationFromName(dest);
        if (nameConverter is null)
            return null;
        return CheckSource(new CompositeConverter(MemberConverter.Original, nameConverter), source.IsNullable, dest);
    }
    //private ISyntaxConverter? EnumToEnumeration(EnumTypeInfo sourceInfo, EnumerationTypeInfo destInfo, IEnumBundle sourceBundle, ConvertSourceInfo convertToInfo)
    //{
    //    // 扩展类非partial,无法扩展,不处理
    //    if (!convertToInfo.IsPartial)
    //        return null;
    //    var underConverter = Get(sourceInfo.ElementInfo, destInfo.OriginalInfo);
    //    if (underConverter is null)
    //        return null;
    //    //var typeInfo = convertToInfo.TypeInfo;
    //    //var methodName = convertToInfo.MethodInfo.Name;
    //    //var source = new EnumToEnumerationSource(_compilation, SyntaxFactory.IdentifierName(sourceInfo.Symbol.Name), destInfo, methodName, sourceBundle, underConverter);
    //    //AddSource(source, typeInfo);
    //    //return convertToInfo.Create();
    //    return null;
    //}
    #endregion
    #region FromEnumeration
    /// <summary>
    /// 枚举类转枚举
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumerationToEnum(EnumerationTypeInfo source, EnumTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        converter = EnumFromString(dest);
        return CheckSource(new CompositeConverter(new MemberConverter(SyntaxFactory.IdentifierName("Name")), converter), source.IsNullable, dest);
    }
    /// <summary>
    /// 枚举转实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumerationToEntity(EnumerationTypeInfo source, EntityPropertyTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        var elementSymbol = dest.Symbol;
        if (elementSymbol.IsString())
        {
            var identifierConverter = PrimitiveToEntity(source.NameInfo, dest);
            if (identifierConverter is null)
                return null;
            return CheckSource(new CompositeConverter(new MemberConverter(SyntaxFactory.IdentifierName("Name")), identifierConverter), source.IsNullable, dest);
        }
        else if (elementSymbol.IsNumericType())
        {
            var originalConverter = PrimitiveToEntity(source.OriginalInfo, dest);
            if (originalConverter is null)
                return null;
            return CheckSource(new CompositeConverter(new MemberConverter(SyntaxFactory.IdentifierName("Original")), originalConverter), source.IsNullable, dest);
        }
        return null;
    }
    /// <summary>
    /// 枚举类转集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EnumerationToCollection(EnumerationTypeInfo source, ICollectionTypeInfo dest)
    {
        var converter = OtherToCollection(source, dest);
        if (converter is not null)
            return converter;
        var elementSymbol = dest.Symbol;
        if (elementSymbol.IsString())
        {
            var identifierConverter = OtherToCollection(source.NameInfo, dest);
            if (identifierConverter is null)
                return null;
            return CheckSource(new CompositeConverter(new MemberConverter(SyntaxFactory.IdentifierName("Name")), identifierConverter), source.IsNullable, dest);
        }
        else if (elementSymbol.IsNumericType())
        {
            var originalConverter = OtherToCollection(source.OriginalInfo, dest);
            if (originalConverter is null)
                return null;
            return CheckSource(new CompositeConverter(new MemberConverter(SyntaxFactory.IdentifierName("Original")), originalConverter), source.IsNullable, dest);
        }
        return null;
    }
    #endregion
}
