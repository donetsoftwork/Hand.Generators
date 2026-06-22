using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// finally
/// </summary>
/// <param name="try"></param>
public class FinallyBuilder(TryBuilder @try)
    : ScopeBuilder([])
{
    #region 配置
    /// <summary>
    /// try节点
    /// </summary>
    protected readonly TryBuilder _try = @try;
    /// <summary>
    /// try节点
    /// </summary>
    public TryBuilder Try
        => _try;
    #endregion
    /// <summary>
    /// 构建分支
    /// </summary>
    /// <returns></returns>
    public FinallyClauseSyntax BuildFinally()
        => SyntaxFactory.FinallyClause(SyntaxFactory.Block(_statements));
    /// <inheritdoc />
    public override StatementSyntax Build()
        => _try.Build();
}
