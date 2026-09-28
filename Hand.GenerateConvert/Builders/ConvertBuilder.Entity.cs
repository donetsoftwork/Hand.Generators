using Hand.Converters;
using Hand.Converters.Members;
using Hand.Reflection;
using Hand.Syntax;
using Hand.Types;
using Microsoft.CodeAnalysis;

namespace Hand.Builders;

/// <summary>
/// 实体属性转化
/// </summary>
public partial class ConvertBuilder
{
    #region ToEntity
    /// <summary>
    /// 转化为实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ToEntity(ITypeSymbolInfo source, EntityPropertyTypeInfo dest)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Enum => EnumToEntity((EnumTypeInfo)source, dest),
            TypeSymbolKind.Enumeration => EnumerationToEntity((EnumerationTypeInfo)source, dest),
            TypeSymbolKind.Primitive => PrimitiveToEntity((PrimitiveTypeInfo)source, dest),
            TypeSymbolKind.Entity => EntityToEntity((EntityPropertyTypeInfo)source, dest),
            TypeSymbolKind.Complex => ComplexToEntity((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Generic => ComplexToEntity((ComplexTypeInfo)source, dest),
            TypeSymbolKind.Void => null,
            _ => OtherToEntity(source, dest),
        };
    }
    /// <summary>
    /// 实体属性转化实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EntityToEntity(EntityPropertyTypeInfo source, EntityPropertyTypeInfo dest)
    {
        (_, var converter) = GetConverter(_compilation, source.Symbol, dest.Symbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var sourceElement = source.ElementInfo;
        var destElement = dest.ElementInfo;
        var elementConverter = Get(sourceElement, destElement);
        if (elementConverter is null)
            return null;
        converter = ConstructorBySingle(destElement.Symbol, dest);
        if (converter is null)
            return null;
        // 先获取Original,再转为子类型, 再转为实体属性
        return CheckSource(new CompositeConverter(new CompositeConverter(MemberConverter.Original, elementConverter), converter), source.IsNullable, dest);
    }    
    /// <summary>
    /// 基础类型转化实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? PrimitiveToEntity(PrimitiveTypeInfo source, EntityPropertyTypeInfo dest)
    {
        var sourceSymbol = source.Symbol;
        var destSymbol = dest.Symbol;
        (_, var converter) = GetConverter(_compilation, sourceSymbol, destSymbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        var element = dest.ElementInfo;
        var elementSymbol = element.Symbol;
        converter = ConstructorBySingle(elementSymbol, dest);
        if (converter is null)
            return null;
        if (sourceSymbol.Equals(elementSymbol, SymbolEqualityComparer.Default))
            return CheckSource(converter, source.IsNullable, dest);
        var elementConverter = Get(source, element);
        if (elementConverter is null)
            return null;
        // 先转为子类型, 再转为实体属性
        return CheckSource(new CompositeConverter(elementConverter, converter), source.IsNullable, dest);
    }
    /// <summary>
    /// 复杂类型转实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? ComplexToEntity(ComplexTypeInfo source, EntityPropertyTypeInfo dest)
    {
        var converter = ComplexToOther(source, dest);
        if (converter is not null)
            return converter;
        var element = dest.ElementInfo;
        var elementConverter = ComplexToOther(source, element);
        if (elementConverter is null)
            return null;
        var entityConverter = ConstructorBySingle(element.Symbol, dest);
        if (entityConverter is null)
            return null;
        // 先转为子类型, 再转为实体属性
        converter = new CompositeConverter(elementConverter, entityConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 其他类型转实体属性
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? OtherToEntity(ITypeSymbolInfo source, EntityPropertyTypeInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var elementInfo = dest.ElementInfo;
        // 尝试系统转化
        var elementConverter = GetCommonConversion(source, elementInfo);
        if (elementConverter is null)
            return null;
        var entityConverter = ConstructorBySingle(elementInfo.Original, dest);
        if (entityConverter is null)
            return null;
        // 先转为子类型, 再转为实体属性
        converter = new CompositeConverter(elementConverter, entityConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    #endregion
    #region FromEntity
    /// <summary>
    /// 实体属性转化基础类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EntityToPrimitive(EntityPropertyTypeInfo source, PrimitiveTypeInfo dest)
    {
        (_, var converter) = GetConverter(_compilation, source.Symbol, dest.Symbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        var elementInfo = source.ElementInfo;
        var destSymbol = dest.Symbol;
        if (elementInfo.Symbol.Equals(destSymbol, SymbolEqualityComparer.Default))
            return MemberConverter.Original;
        if (destSymbol.IsString())
            return EntityToString(source);
        var elementConverter = Get(elementInfo, dest);
        if (elementConverter is not null)
            return new CompositeConverter(MemberConverter.Original, elementConverter);
        return null;
    }
    /// <summary>
    /// 实体转集合
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EntityToCollection(EntityPropertyTypeInfo source, ICollectionTypeInfo dest)
    {
        var converter = OtherToCollection(source, dest);
        if (converter is not null)
            return converter;
        var element = source.ElementInfo;
        var elementConverter = OtherToCollection(element, dest);
        if (elementConverter is null)
            return null;
        // 先转为子类型, 再转为集合
        converter = new CompositeConverter(MemberConverter.Original, elementConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 枚举转化为复杂类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EntityToComplex(EntityPropertyTypeInfo source, ComplexTypeInfo dest)
    {
        (_, var converter) = GetConverter(_compilation, source.Symbol, dest.Symbol);
        if (converter is not null)
            return CheckSource(converter, source.IsNullable, dest);
        converter = OtherToComplex(source, dest);
        if (converter is not null)
            return converter;
        var element = source.ElementInfo;
        var elementConverter = OtherToComplex(element, dest);
        if (elementConverter is null)
            return null;
        // 先转为子类型, 再转为复杂类型
        converter = new CompositeConverter(MemberConverter.Original, elementConverter);
        return CheckSource(converter, source.IsNullable, dest);
    }
    /// <summary>
    /// 实体属性转未知类型
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? EntityToUnknow(EntityPropertyTypeInfo source, ITypeSymbolInfo dest)
    {
        var converter = GetCommonConversion(source, dest);
        if (converter is not null)
            return converter;
        var elementInfo = source.ElementInfo;
        // 尝试系统转化
        var elementConverter = GetCommonConversion(elementInfo, dest);
        if (elementConverter is null)
            return null;
        //var entityConverter = ConstructorBySingle(elementInfo.Symbol, dest);
        //if (entityConverter is null)
        //    return null;
        //// 先转为子类型, 再转为未知类型
        //converter = new CompatibleConverter(entityConverter, elementConverter);
        //return CheckSource(converter, source.IsNullable, dest);
        return null;
    }
    #endregion
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //public static IConverter? ElementToEntity(EntityTypeInfo dest)
    //    => ConstructorBySingle(dest.Symbol, dest.Element);
    ///// <summary>
    ///// 从实体属性转化
    ///// </summary>
    ///// <param name="sourceInfo"></param>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //public IConverter? FromEntity(EntityTypeInfo sourceInfo, ITypeSymbolInfo dest)
    //{
    //    if (sourceInfo.Element.Equals(dest.Symbol, SymbolEqualityComparer.Default))
    //        return MemberConverter.Original;
    //    var elementConverter = Get(sourceInfo.ElementInfo, dest);
    //    if (elementConverter is not null)
    //        return new CompatibleConverter(MemberConverter.Original, elementConverter);
    //    return null;
    //}
    ///// <summary>
    ///// 实体转数组
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //public IConverter? EntityToArray(EntityTypeInfo source, ArrayTypeInfo dest)
    //{
    //    var converter = OtherToArray(source, dest);
    //    if (converter is not null)
    //        return converter;
    //    var element = source.ElementInfo;
    //    var elementConverter = OtherToArray(element, dest);
    //    if (elementConverter is null)
    //        return null;
    //    // 先转为子类型, 再转为数组
    //    converter = new CompatibleConverter(MemberConverter.Original, elementConverter);
    //    return CheckSource(converter, source.IsNullable, dest);
    //}
}
