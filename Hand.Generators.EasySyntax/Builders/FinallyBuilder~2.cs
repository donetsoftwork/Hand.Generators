using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// Finally
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="try"></param>
public class FinallyBuilder<TGrandpa, TParent>(TryBuilder<TGrandpa, TParent> @try)
    : ScopeBuilder<TGrandpa, TParent>(@try.Parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    /// <summary>
    /// try节点
    /// </summary>
    protected readonly TryBuilder<TGrandpa, TParent> _try = @try;
    /// <summary>
    /// try节点
    /// </summary>
    public TryBuilder<TGrandpa, TParent> Try
        => _try;
    #endregion
    /// <summary>
    /// 构建分支
    /// </summary>
    /// <returns></returns>
    public FinallyClauseSyntax BuildFinally()
        => SyntaxFactory.FinallyClause(SyntaxFactory.Block(_statements));
    /// <inheritdoc />
    protected internal override TParent BuildCore()
        => _try.BuildCore();
}
