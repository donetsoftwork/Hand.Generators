using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// If
/// </summary>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="condition"></param>
public class IfBuilder<TGrandpa, TParent>(TParent parent, ExpressionSyntax condition)
    : ScopeBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    protected readonly ExpressionSyntax _condition = condition;
    /// <summary>
    /// 当前条件
    /// </summary>
    public ExpressionSyntax Condition 
        => _condition;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(BuildCurrent());
        return _parent;
    }
    /// <summary>
    /// 构造当前语句
    /// </summary>
    /// <param name="statements"></param>
    /// <returns></returns>
    protected virtual IfStatementSyntax BuildCurrent(List<StatementSyntax> statements)
        => SyntaxFactory.IfStatement(_condition, Concat(statements) ?? SyntaxFactory.EmptyStatement());
    /// <summary>
    /// 构造当前语句
    /// </summary>
    /// <returns></returns>
    protected internal IfStatementSyntax BuildCurrent()
        => BuildCurrent(_statements);
    /// <summary>
    /// ElseIf
    /// </summary>
    /// <returns></returns>
    public ElseIfBuilder<TGrandpa, TParent> ElseIf(ExpressionSyntax @if)
        => new(_parent, this, @if);
    /// <summary>
    /// Else
    /// </summary>
    /// <returns></returns>
    public ElseBuilder<TGrandpa, TParent> Else()
        => new(_parent, this);
}
