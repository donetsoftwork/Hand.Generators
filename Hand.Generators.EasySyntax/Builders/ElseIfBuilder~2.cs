using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// ElseIf
/// </summary>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="if"></param>
/// <param name="condition"></param>
public class ElseIfBuilder<TGrandpa, TParent>(TParent parent, IfBuilder<TGrandpa, TParent> @if, ExpressionSyntax condition)
    : IfBuilder<TGrandpa, TParent>(parent, condition)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly IfBuilder<TGrandpa, TParent> _if = @if;
    /// <summary>
    /// if
    /// </summary>
    public IfBuilder<TGrandpa, TParent> If
        => _if;
    #endregion
    /// <inheritdoc />
    protected override IfStatementSyntax BuildCurrent(List<StatementSyntax> statements)
    {
        var @if = _if.BuildCurrent();
        var elseif = SyntaxFactory.ElseClause(base.BuildCurrent(statements));
        return @if.WithElse(elseif);
    }
}
