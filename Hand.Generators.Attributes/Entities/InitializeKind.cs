namespace Hand.Entities;

/// <summary>
/// 初始化类型
/// </summary>
[Flags]
public enum InitializeKind
{
    /// <summary>
    /// 属性
    /// </summary>
    Property = 1,
    /// <summary>
    /// 字段
    /// </summary>
    Field = 2,
    /// <summary>
    /// 初始化
    /// </summary>
    Init = 4,
    /// <summary>
    /// 构造函数
    /// </summary>
    Constructor = 8,
}
