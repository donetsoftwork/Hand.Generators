using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// ElseIf
/// </summary>
/// <param name="if"></param>
/// <param name="condition"></param>
public class ElseIfBuilder(IfBuilder @if, ExpressionSyntax condition)
    : IfBuilder(condition)
{
    #region 配置
    private readonly IfBuilder _if = @if;
    /// <summary>
    /// if
    /// </summary>
    public IfBuilder If
        => _if;
    #endregion
    /// <inheritdoc />
    protected internal override IfStatementSyntax WithElse(ElseClauseSyntax? @else)
    {
        var elseif = SyntaxFactory.ElseClause(base.WithElse(@else));
        return _if.WithElse(elseif);
    }
}
