namespace Hand.Reflection;

/// <summary>
/// 类型类别
/// </summary>
public enum TypeSymbolKind
{
    /// <summary>
    /// 未知类型
    /// </summary>
    Unknow = 0,
    /// <summary>
    /// 参数
    /// </summary>
    Parameter = 1,
    /// <summary>
    /// 基础类型
    /// </summary>
    Primitive = 2,
    /// <summary>
    /// 枚举类型
    /// </summary>
    Enum = 3,
    /// <summary>
    /// 数组类型
    /// </summary>
    Array = 4,
    /// <summary>
    /// 集合类型
    /// </summary>
    Collection = 5,
    /// <summary>
    /// 泛型
    /// </summary>
    Generic = 6,
    /// <summary>
    /// 复合类型
    /// </summary>
    Complex = 7,
    /// <summary>
    /// 实体属性类型(Hand.Models.IEntityProperty)
    /// </summary>
    Entity = 8,
}
