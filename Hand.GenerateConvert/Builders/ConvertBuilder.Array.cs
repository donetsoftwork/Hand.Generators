using Hand.Collections;
using Hand.Converters;
using Hand.Converters.Collections;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;

namespace Hand.Builders;

/// <summary>
/// 数组类型转化
/// </summary>
public partial class ConvertBuilder
{
    #region ToArray
    /// <summary>
    /// 转化为数组
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ToArray(ITypeSymbolInfo source, ArrayTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Array => ArrayToArray((ArrayTypeInfo)source, dest),
            TypeSymbolKind.Collection => CollectionToArray((CollectionTypeInfo)source, dest),
            TypeSymbolKind.Enum => EnumToCollection((EnumTypeInfo)source, dest),
            TypeSymbolKind.Enumeration => EnumerationToCollection((EnumerationTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToCollection((EntityPropertyTypeInfo)source, dest),
            TypeSymbolKind.Void => null,
            _ => OtherToCollection(source, dest),
        };
    }
    /// <summary>
    /// 数组转化为数组
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ArrayToArray(ArrayTypeInfo source, ArrayTypeInfo dest)
    {
        var elementConverter = Get(source.ElementInfo, dest.ElementInfo);
        if (elementConverter is null)
            return null;
        return CheckSource(ArrayConverter.Create(elementConverter), source.IsNullable, dest);
    }
    /// <summary>
    /// 集合转化为数组
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? CollectionToArray(CollectionTypeInfo source, ArrayTypeInfo dest)
    {
        var sourceElement = source.ElementInfo;
        var destElement = dest.ElementInfo;
        if (sourceElement.Original.Equals(destElement.Original, SymbolEqualityComparer.Default))
        {
            return CheckSource(LinqConverter.ToArray(), source.IsNullable, dest);
        }
        else
        {
            var elementConverter = Get(sourceElement, destElement);
            if (elementConverter is null)
                return null;
            var selectConverter = ItemLinqConverter.Select(elementConverter);
            var converter = new CompositeConverter(selectConverter, LinqConverter.ToArray());
            return CheckSource(converter, source.IsNullable, dest);
        }
    }
    #endregion
}
