using Hand.Converters;
using Hand.Converters.Collections;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 集合类型转化
/// </summary>
public partial class ConvertBuilder
{
    #region ToCollection
    /// <summary>
    /// 转化为集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ToCollection(ITypeSymbolInfo source, CollectionTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Collection => CollectionToCollection((ICollectionTypeInfo)source, dest),
            TypeSymbolKind.Array => CollectionToCollection((ICollectionTypeInfo)source, dest),
            TypeSymbolKind.Enum => EnumToCollection((EnumTypeInfo)source, dest),
            TypeSymbolKind.Enumeration => EnumerationToCollection((EnumerationTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToCollection((EntityPropertyTypeInfo)source, dest),
            TypeSymbolKind.Void => null,
            _ => OtherToCollection(source, dest),
        };
    }
    /// <summary>
    /// 集合转化为集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? CollectionToCollection(ICollectionTypeInfo source, CollectionTypeInfo dest)
    {
        var listType = _compilation.GetListSymbol();
        if (listType is null)
            return null;
        var destSymbol = dest.Symbol;
        if (destSymbol.IsGenericType(listType)
            || destSymbol.IsGenericType(SpecialType.System_Collections_Generic_IList_T)
            || destSymbol.IsGenericType(SpecialType.System_Collections_Generic_IReadOnlyList_T)
            || destSymbol.IsGenericType(SpecialType.System_Collections_Generic_ICollection_T)
            || destSymbol.IsGenericType(SpecialType.System_Collections_Generic_IReadOnlyCollection_T)
            || destSymbol.IsGenericType(SpecialType.System_Collections_Generic_IEnumerable_T))
            return CollectionToList(source, dest, source.IsNullable, IsList(source.Symbol));

        return null;

        bool IsList(ITypeSymbol symbol)
            => symbol is INamedTypeSymbol namedType && namedType.IsGenericType(listType);
    }
    /// <summary>
    /// 其他类型转集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? OtherToCollection(ITypeSymbolInfo source, ICollectionTypeInfo dest)
    {
        var destElement = dest.ElementInfo;
        if (source.Original.Equals(destElement.Original, SymbolEqualityComparer.Default))
            return CheckSource(ElementToCollectionConverter.Instance, source.IsNullable, dest);

        var elementConverter = Get(source, destElement);
        if (elementConverter is null)
            return null;
        var converter = new CompositeConverter(elementConverter, ElementToCollectionConverter.Instance);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 集合转化为列表
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="sourceIsNull"></param>
    /// <param name="sourceIsList"></param>
    /// <returns></returns>
    public ISyntaxConverter? CollectionToList(ICollectionTypeInfo source, ICollectionTypeInfo dest, bool sourceIsNull, bool sourceIsList)
    {
        var sourceElement = source.ElementInfo;
        var destElement = dest.ElementInfo;
        if (sourceIsList)
        {
            var elementConverter = Get(sourceElement, destElement);
            if (elementConverter is null)
                return null;
            return CheckSource(ListConverter.Create(elementConverter), sourceIsNull, dest);
        }
        if (sourceElement.Original.IsCompatible(destElement.Original))
        {
            return CheckSource(LinqConverter.ToList(), sourceIsNull, dest);
        }
        else
        {
            var elementConverter = Get(sourceElement, destElement);
            if (elementConverter is null)
                return null;
            var selectConverter = ItemLinqConverter.Select(elementConverter);
            var converter = new CompositeConverter(selectConverter, LinqConverter.ToList());
            return CheckSource(converter, sourceIsNull, dest);
        }
    }
    #endregion
    #region FromCollection
    /// <summary>
    /// 集合转化其他类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? CollectionToOther(ICollectionTypeInfo source, ITypeSymbolInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var sourceElement = source.ElementInfo;
        if (sourceElement.Original.Equals(dest.Original, SymbolEqualityComparer.Default))
            return CheckSource(GetFirstConverter(dest, DefaultExpressionBuilder.GetTypedDefault(dest)), source.IsNullable, dest);

        var elementConverter = Get(sourceElement, dest);
        if (elementConverter is null)
            return null;
        var selectConverter = ItemLinqConverter.Select(elementConverter);
        var defaultValue = DefaultExpressionBuilder.GetTypedDefault(dest);
        var firstConverter = GetFirstConverter(dest, defaultValue);
        converter = new CompositeConverter(selectConverter, firstConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 获取第一个元素
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static ISyntaxConverter GetFirstConverter(ITypeSymbolInfo dest, ExpressionSyntax defaultValue)
    {
        var isNullable = dest.IsNullable || !dest.Original.IsValueType;
        if (isNullable && defaultValue.IsKind(SyntaxKind.DefaultExpression))
            return FirstOrDefaultConverter.Create();
        if (SyntaxGenerator.FrameworkMajorVersion >= 6)
            return FirstOrDefaultValueConverter.Create(defaultValue);
        else
            return FirstOrCoalesceConverter.Create(defaultValue);
    }
    #endregion
    ///// <summary>
    ///// 单个转数组或集合
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //public IConverter? ElementToCollection(ITypeSymbolInfo source, ICollectionSymbolInfo dest)
    //{
    //    var destElement = dest.ElementInfo;
    //    if (source.Original.Equals(destElement.Original, SymbolEqualityComparer.Default))
    //        return CheckSource(ElementToCollectionConverter.Instance, source.IsNullable, dest);

    //    var elementConverter = Get(source, destElement);
    //    if (elementConverter is null)
    //        return null;
    //    var converter = new CompatibleConverter(elementConverter, ElementToCollectionConverter.Instance);
    //    return CheckSource(converter, source.IsNullable, dest);
    //}
    ///// <summary>
    ///// 集合转化为列表
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="dest"></param>
    ///// <param name="sourceIsNull"></param>
    ///// <returns></returns>
    //public IConverter? CollectionToEnumerable(ICollectionSymbolInfo source, ICollectionSymbolInfo dest, bool sourceIsNull)
    //{
    //    var sourceElement = source.ElementInfo;
    //    var destElement = dest.ElementInfo;
    //    if (sourceElement.Original.Equals(destElement.Original, SymbolEqualityComparer.Default))
    //        return new PassConverter(DefaultExpressionBuilder.GetDefault(dest, _compilation), sourceIsNull);

    //    var elementConverter = Get(sourceElement, destElement);
    //    if (elementConverter is null)
    //        return null;
    //    var enumerableConverter = new EnumerableConverter(elementConverter);
    //    return CheckSource(enumerableConverter, sourceIsNull, dest);
    //}
}
