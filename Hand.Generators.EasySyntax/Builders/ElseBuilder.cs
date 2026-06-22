using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// Else
/// </summary>
/// <param name="if"></param>
public class ElseBuilder(IfBuilder @if)
    : ScopeBuilder([])
{
    #region 配置
    private readonly IfBuilder _if = @if;
    /// <summary>
    /// If
    /// </summary>
    public IfBuilder If
        => _if;
    #endregion
    /// <summary>
    /// 构造当前语句
    /// </summary>
    /// <returns></returns>
    protected internal IfStatementSyntax BuildCurrent()
    {
        var statement = Concat(_statements);
        if (statement is null)
            return _if.BuildCurrent();
        return _if.WithElse(SyntaxFactory.ElseClause(statement));
    }
    /// <inheritdoc />
    public override StatementSyntax Build()
        => BuildCurrent();
}
