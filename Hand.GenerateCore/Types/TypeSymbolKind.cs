namespace Hand.Types;

/// <summary>
/// 类型类别
/// </summary>
public enum TypeSymbolKind
{
    /// <summary>
    /// 未知类型
    /// </summary>
    Unknow,
    /// <summary>
    /// void类型
    /// </summary>
    Void,
    /// <summary>
    /// 参数
    /// </summary>
    Parameter,
    /// <summary>
    /// 基础类型
    /// </summary>
    Primitive,
    /// <summary>
    /// 枚举类型
    /// </summary>
    Enum,
    /// <summary>
    /// 复合类型
    /// </summary>
    Complex,
    /// <summary>
    /// 实体属性类型(Hand.Primitives.IEntityProperty)
    /// </summary>
    Entity,
    /// <summary>
    /// 枚举类
    /// </summary>
    Enumeration,
    /// <summary>
    /// 泛型
    /// </summary>
    Generic,
    /// <summary>
    /// 数组类型
    /// </summary>
    Array,
    /// <summary>
    /// 集合类型
    /// </summary>
    Collection,
}
