using Hand.Converters;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;

namespace Hand.Builders;

/// <summary>
/// 未知类型转化
/// </summary>
public partial class ConvertBuilder
{
    /// <summary>
    /// 转化为未知类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? ToUnknow(ITypeSymbolInfo source, ITypeSymbolInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Primitive => PrimitiveToUnknow((PrimitiveTypeInfo)source, dest),
            TypeSymbolKind.Enum => EnumToUnknow((EnumTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToUnknow((EntityTypeInfo)source, dest),
            _ => GetCommonConversion(source, dest),
        };
    }
    /// <summary>
    /// 基础类型转未知类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? PrimitiveToUnknow(PrimitiveTypeInfo source, ITypeSymbolInfo dest)
    {
        IConverter? converter;
        if (dest.Symbol is INamedTypeSymbol namedType)
        {
            (_, converter) = GetConverter(_compilation, source.Symbol, namedType);
            if (converter is not null)
                return CheckSource(converter, source.IsNullable, dest);
        }
        converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        return null;
    }
}
