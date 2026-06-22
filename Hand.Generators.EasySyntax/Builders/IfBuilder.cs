using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// If
/// </summary>
/// <param name="condition"></param>
public class IfBuilder(ExpressionSyntax condition)
    : ScopeBuilder([])
{
    #region 配置
    /// <summary>
    /// 当前条件
    /// </summary>
    protected readonly ExpressionSyntax _condition = condition;
    /// <summary>
    /// 当前条件
    /// </summary>
    public ExpressionSyntax Condition
        => _condition;
    #endregion
    /// <summary>
    /// 构造当前语句
    /// </summary>
    /// <returns></returns>
    protected internal IfStatementSyntax BuildCurrent()
        => WithElse(null);
    /// <summary>
    /// 构建else
    /// </summary>
    /// <param name="else"></param>
    /// <returns></returns>
    protected internal virtual IfStatementSyntax WithElse(ElseClauseSyntax? @else)
        => SyntaxFactory.IfStatement(_condition, Concat(_statements) ?? SyntaxFactory.EmptyStatement(), @else);
    /// <inheritdoc />
    public override StatementSyntax Build()
         => WithElse(null);
    /// <summary>
    /// ElseIf
    /// </summary>
    /// <returns></returns>
    public ElseIfBuilder ElseIf(ExpressionSyntax @if)
        => new(this, @if);
    /// <summary>
    /// Else
    /// </summary>
    /// <returns></returns>
    public ElseBuilder Else()
        => new(this);
}
