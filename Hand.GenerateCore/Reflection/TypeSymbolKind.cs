using System;

namespace Hand.Reflection;

/// <summary>
/// 类型类别
/// </summary>
[Flags]
public enum TypeSymbolKind
{
    /// <summary>
    /// 基础类型
    /// </summary>
    Primitive = 0,
    /// <summary>
    /// 可空类型
    /// </summary>
    Nullable = 1,
    /// <summary>
    /// 枚举类型
    /// </summary>
    Enum = 2,
    /// <summary>
    /// 数组类型
    /// </summary>
    Array = 4,
    /// <summary>
    /// 集合类型
    /// </summary>
    Collection = 8,
    /// <summary>
    /// 复合类型
    /// </summary>
    Complex = 16,
    /// <summary>
    /// 实体属性类型(Hand.Models.IEntityProperty)
    /// </summary>
    Entity = 32,
}
