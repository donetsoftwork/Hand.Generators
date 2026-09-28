using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Enums.Fields;

/// <summary>
/// 枚举字段
/// </summary>
public class EnumField(string name, string member, LiteralExpressionSyntax under, string summary)
    : IEnumField
{    
    #region 配置
    private readonly string _name = name;
    private readonly string _member = member;
    private readonly LiteralExpressionSyntax _under = under;


    /// <inheritdoc />
    public string Name
        => _name;
    /// <inheritdoc />
    public string Member
        => _member;
    /// <inheritdoc />
    public LiteralExpressionSyntax Under 
        => _under;
    /// <inheritdoc />
    public string Summary { get; } = summary;
    #endregion
    /// <inheritdoc />
    public ExpressionSyntax GetExpression(TypeSyntax enumType)
        => enumType.Access(_name);
    /// <inheritdoc />
    public bool Match(string name)
        => string.Equals(name, _name, StringComparison.OrdinalIgnoreCase‌);
    /// <inheritdoc />
    public bool MatchMember(string member)
        => string.Equals(member, _member, StringComparison.OrdinalIgnoreCase‌);    
}
