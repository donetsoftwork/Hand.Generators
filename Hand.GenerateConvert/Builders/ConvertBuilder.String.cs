using Hand.Collections;
using Hand.Converters;
using Hand.Converters.Collections;
using Hand.Converters.Members;
using Hand.Converters.System;
using Hand.Reflection;
using Hand.Syntax;
using Hand.Types;

namespace Hand.Builders;

/// <summary>
/// 字符串转化构造器
/// </summary>
public partial class ConvertBuilder
{
    /// <summary>
    /// 转化为字符串
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public ISyntaxConverter ToString(ITypeSymbolInfo source)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Array => ArrayToToString((ArrayTypeInfo)source),
            TypeSymbolKind.Collection => CollectionToString((CollectionTypeInfo)source),
            TypeSymbolKind.Entity => EntityToString((EntityPropertyTypeInfo)source),
            _ => ToStringConverter.Instance,
        };
    }
    /// <summary>
    /// 数组转化为字符串
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ISyntaxConverter ArrayToToString(ArrayTypeInfo source)
    {
        var elementInfo = source.ElementInfo;
        if (elementInfo.Kind == TypeSymbolKind.Primitive && elementInfo.Original.IsChar())
            return NewStringConverter.Instance;
        return ToStringConverter.Instance;
    }
    /// <summary>
    /// 集合转化为字符串
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ISyntaxConverter CollectionToString(CollectionTypeInfo source)
    {
        var elementInfo = source.ElementInfo;
        if (elementInfo.Kind == TypeSymbolKind.Primitive && elementInfo.Original.IsChar())
        {
            return new CompositeConverter(LinqConverter.ToArray(), NewStringConverter.Instance);
        }
        return ToStringConverter.Instance;
    }
    /// <summary>
    /// 实体属性转化为字符串
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public ISyntaxConverter EntityToString(EntityPropertyTypeInfo source)
    {
        var elementConverter = ToString(source.ElementInfo);
        return new CompositeConverter(MemberConverter.Original, elementConverter);
    }
    /// <summary>
    /// 从字符串转化为
    /// </summary>
    /// <param name="dest"></param>
    /// <returns></returns>
    public ISyntaxConverter? FromString(ITypeSymbolInfo dest)
    {
        return null;
    }
}
