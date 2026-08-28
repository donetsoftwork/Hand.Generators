using Hand.Collections;
using Hand.Converters;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;
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
    public IConverter? ToCollection(ITypeSymbolInfo source, CollectionTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Collection => CollectionToCollection((ICollectionSymbolInfo)source, dest),
            TypeSymbolKind.Array => CollectionToCollection((ICollectionSymbolInfo)source, dest),
            TypeSymbolKind.Enum => EnumToCollection((EnumTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToCollection((EntityTypeInfo)source, dest),
            _ => OtherToCollection(source, dest),
        };
    }
    /// <summary>
    /// 集合转化为集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? CollectionToCollection(ICollectionSymbolInfo source, CollectionTypeInfo dest)
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
    public IConverter? OtherToCollection(ITypeSymbolInfo source, ICollectionSymbolInfo dest)
    {
        var destElement = dest.ElementInfo;
        if (source.Original.Equals(destElement.Original, SymbolEqualityComparer.Default))
            return CheckSource(ElementToCollectionConverter.Instance, source.IsNullable, dest);

        var elementConverter = Get(source, destElement);
        if (elementConverter is null)
            return null;
        var converter = new CompatibleConverter(elementConverter, ElementToCollectionConverter.Instance);
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
    public IConverter? CollectionToList(ICollectionSymbolInfo source, ICollectionSymbolInfo dest, bool sourceIsNull, bool sourceIsList)
    {
        var sourceElement = source.ElementInfo;
        var destElement = dest.ElementInfo;
        if (sourceIsList)
        {
            var elementConverter = Get(sourceElement, destElement);
            if (elementConverter is null)
                return null;
            return CheckSource(new ListConverter(elementConverter), sourceIsNull, dest);
        }
        if (sourceElement.Original.IsCompatible(destElement.Original))
        {
            return CheckSource(EnumerableToListConverter.Instance, sourceIsNull, dest);
        }
        else
        {
            var elementConverter = Get(sourceElement, destElement);
            if (elementConverter is null)
                return null;
            var enumerableConverter = new EnumerableConverter(elementConverter);
            var converter = new CompatibleConverter(enumerableConverter, EnumerableToListConverter.Instance);
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
    public IConverter? CollectionToOther(ICollectionSymbolInfo source, ITypeSymbolInfo dest)
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
        var enumerableConverter = new EnumerableConverter(elementConverter);
        var defaultValue = DefaultExpressionBuilder.GetTypedDefault(dest);
        var firstConverter = GetFirstConverter(dest, defaultValue);
        converter = new CompatibleConverter(enumerableConverter, firstConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 获取第一个元素
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static IConverter GetFirstConverter(ITypeSymbolInfo dest, ExpressionSyntax defaultValue)
    {
        var isNullable = dest.IsNullable || !dest.Original.IsValueType;
        if (SyntaxGenerator.FrameworkMajorVersion >= 6)
            return new CollectionFirstOrDefaultConverter(defaultValue);
        else if (isNullable)
            return new CollectionFirstOrCoalesceConverter(isNullable, defaultValue);
        else
            return CollectionFirstOrCoalesceConverter.Generic(dest.Original.ToSyntax().Nullable(), isNullable, defaultValue);
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
