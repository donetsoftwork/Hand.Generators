using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Enums;

/// <summary>
/// 枚举字段
/// </summary>
public class EnumField(string name, string member/*, MemberAccessExpressionSyntax expression*/, LiteralExpressionSyntax under)
    : IEnumField
{    
    #region 配置
    private readonly string _name = name;
    private readonly string _member = member;
    //private readonly MemberAccessExpressionSyntax _expression = expression;
    private readonly LiteralExpressionSyntax _under = under;

    /// <inheritdoc />
    public string Name
        => _name;
    /// <inheritdoc />
    public string Member
        => _member;
    ///// <summary>
    ///// 枚举值
    ///// </summary>
    //public MemberAccessExpressionSyntax Expression
    //    => _expression;
    /// <summary>
    /// 基础值
    /// </summary>
    public LiteralExpressionSyntax Under 
        => _under;
    #endregion
    /// <inheritdoc />
    public MemberAccessExpressionSyntax GetExpression(TypeSyntax enumType)
        => enumType.Access(_name);
    /// <inheritdoc />
    public bool Match(string name)
        => string.Equals(name, _name, StringComparison.OrdinalIgnoreCase‌);
    /// <inheritdoc />
    public bool MatchMember(string member)
        => string.Equals(member, _member, StringComparison.OrdinalIgnoreCase‌);    
}
