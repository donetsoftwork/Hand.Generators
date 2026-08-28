using Hand.Collections;
using Hand.Converters;
using Hand.Reflection;
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
    public IConverter ToString(ITypeSymbolInfo source)
    {
        return source.Kind switch
        {
            TypeSymbolKind.Array => ArrayToToString((ArrayTypeInfo)source),
            TypeSymbolKind.Collection => CollectionToString((CollectionTypeInfo)source),
            TypeSymbolKind.Entity => EntityToString((EntityTypeInfo)source),
            _ => ToStringConverter.Instance,
        };
    }
    /// <summary>
    /// 数组转化为字符串
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IConverter ArrayToToString(ArrayTypeInfo source)
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
    public static IConverter CollectionToString(CollectionTypeInfo source)
    {
        var elementInfo = source.ElementInfo;
        if (elementInfo.Kind == TypeSymbolKind.Primitive && elementInfo.Original.IsChar())
        {
            return new CompatibleConverter(EnumerableToArrayConverter.Instance, NewStringConverter.Instance);
        }
        return ToStringConverter.Instance;
    }
    /// <summary>
    /// 实体属性转化为字符串
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public IConverter EntityToString(EntityTypeInfo source)
    {
        var elementConverter = ToString(source.ElementInfo);
        return new CompatibleConverter(MemberConverter.Original, elementConverter);
    }
    /// <summary>
    /// 从字符串转化为
    /// </summary>
    /// <param name="dest"></param>
    /// <returns></returns>
    public IConverter? FromString(ITypeSymbolInfo dest)
    {
        return null;
    }
}
