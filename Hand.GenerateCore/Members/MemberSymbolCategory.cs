using System;

namespace Hand.Members;

/// <summary>
/// 成员类别
/// </summary>
[Flags]
public enum MemberSymbolCategory
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
    /// 实体类型
    /// </summary>
    Entity = 16,
    /// <summary>
    /// 复合类型
    /// </summary>
    Complex = 32,
}
