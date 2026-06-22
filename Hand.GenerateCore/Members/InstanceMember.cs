using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Members;

/// <summary>
/// 成员访问
/// </summary>
/// <param name="memberName"></param>
/// <param name="isNullable"></param>
public class InstanceMember(SimpleNameSyntax memberName, bool isNullable = false)
{
    #region 配置
    /// <summary>
    /// 成员名称
    /// </summary>
    protected readonly SimpleNameSyntax _memberName = memberName;
    /// <summary>
    /// 是否nullable
    /// </summary>
    protected readonly bool _isNullable = isNullable;
    /// <summary>
    /// 成员名称
    /// </summary>
    public SimpleNameSyntax MemberName
        => _memberName;
    /// <summary>
    /// 是否nullable
    /// </summary>
    public bool IsNullable
        => _isNullable;
    #endregion

    /// <summary>
    /// 获取成员访问表达式
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public virtual ExpressionSyntax GetMember(ExpressionSyntax owner)
        => GetMember(owner, _isNullable, _memberName);
    /// <summary>
    /// 获取成员访问表达式
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="isNullable"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static ExpressionSyntax GetMember(ExpressionSyntax owner, bool isNullable, SimpleNameSyntax memberName)
        => isNullable ? owner.ConditionalAccess(memberName) : owner.Access(memberName);
}
