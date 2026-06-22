using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// finally
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="try"></param>
/// <param name="original"></param>
public class FinallyBuilder<TGrandpa, TParent>(TryBuilder<TGrandpa, TParent> @try, FinallyBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(@try.Parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    /// <summary>
    /// try节点
    /// </summary>
    private readonly TryBuilder<TGrandpa, TParent> _try = @try;
    private readonly FinallyBuilder _original = original;
    /// <summary>
    /// try节点
    /// </summary>
    public TryBuilder<TGrandpa, TParent> Try
        => _try;
    /// <summary>
    /// 原始finally
    /// </summary>
    public FinallyBuilder Original
        => _original;
    #endregion
    /// <summary>
    /// 构建finally
    /// </summary>
    /// <returns></returns>
    public FinallyClauseSyntax BuildFinally()
        => _original.BuildFinally();
    /// <inheritdoc />
    protected internal override TParent BuildCore()
        => _try.BuildCore();
}
