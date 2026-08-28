using Hand.Converters;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;

namespace Hand.Builders;

/// <summary>
/// 基础类型转化
/// </summary>
public partial class ConvertBuilder
{
    #region ToPrimitive
    /// <summary>
    /// 转化为基础类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? ToPrimitive(ITypeSymbolInfo source, PrimitiveTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Enum => EnumToPrimitive((EnumTypeInfo)source, dest),
            TypeSymbolKind.Primitive => PrimitiveToPrimitive((PrimitiveTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToPrimitive((EntityTypeInfo)source, dest),
            TypeSymbolKind.Complex => ComplexToOther((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Generic => ComplexToOther((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Array => CollectionToOther((ICollectionSymbolInfo)source, dest),
            TypeSymbolKind.Collection => CollectionToPrimitive((CollectionTypeInfo)source, dest),
            _ => OtherToPrimitive(source, dest)
        };
    }
    /// <summary>
    /// 基础类型转化基础类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? PrimitiveToPrimitive(PrimitiveTypeInfo source, PrimitiveTypeInfo dest)
    {
        (_, var converter) = GetConverter(_compilation, source.Symbol, dest.Symbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        return GetCommonConversion(source, dest) ?? GetConverterBySystem(source, dest);
    }
    /// <summary>
    /// 集合转基础类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? CollectionToPrimitive(CollectionTypeInfo source, PrimitiveTypeInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        if (dest.Symbol.IsString())
            return CollectionToString(source);
        return CollectionToOther(source, dest);
    }
    /// <summary>
    /// 其他类型转化为基础类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? OtherToPrimitive(ITypeSymbolInfo source, PrimitiveTypeInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        if (dest.Symbol.IsString())
            return ToString(source);
        return null;
    }
    #endregion
}
