namespace Hand.Members;

/// <summary>
/// 成员类型
/// </summary>
public enum MemberKind
{
    /// <summary>
    /// 未知类型
    /// </summary>
    Unknown,
    /// <summary>
    /// 属性
    /// </summary>
    Property,
    /// <summary>
    /// 字段
    /// </summary>
    Field,
    /// <summary>
    /// 方法
    /// </summary>
    Method,
    /// <summary>
    /// 构造函数
    /// </summary>
    Constructor,
    /// <summary>
    /// 属性定义
    /// </summary>
    PropertyDeclaration,
    /// <summary>
    /// 字段定义
    /// </summary>
    FieldDeclaration,
    /// <summary>
    /// 方法定义
    /// </summary>
    MethodDeclaration,
    /// <summary>
    /// 构造函数定义
    /// </summary>
    ConstructorDeclaration,
    /// <summary>
    /// 参数
    /// </summary>
    Parameter,
    /// <summary>
    /// 参数定义
    /// </summary>
    ParameterDeclaration,
    /// <summary>
    /// 记录参数
    /// </summary>
    RecordParameterDeclaration,
}
