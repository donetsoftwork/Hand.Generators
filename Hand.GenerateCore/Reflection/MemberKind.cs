namespace Hand.Reflection;

/// <summary>
/// 成员类型
/// </summary>
public enum MemberKind
{
    /// <summary>
    /// 属性
    /// </summary>
    Property = 0,
    /// <summary>
    /// 字段
    /// </summary>
    Field = 1,
    /// <summary>
    /// 参数
    /// </summary>
    Parameter = 2,
    /// <summary>
    /// 属性定义
    /// </summary>
    PropertyDeclaration = 3,
    /// <summary>
    /// 字段定义
    /// </summary>
    FieldDeclaration = 4,
    /// <summary>
    /// 参数定义
    /// </summary>
    ParameterDeclaration = 5
}
