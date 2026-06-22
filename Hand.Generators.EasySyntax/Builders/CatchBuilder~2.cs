using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// catch
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="try"></param>
/// <param name="original"></param>
public class CatchBuilder<TGrandpa, TParent>(TryBuilder<TGrandpa, TParent> @try, CatchBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(@try.Parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    /// <summary>
    /// try节点
    /// </summary>
    private readonly TryBuilder<TGrandpa, TParent> _try = @try;
    private readonly CatchBuilder _original = original;
    /// <summary>
    /// try节点
    /// </summary>
    public TryBuilder<TGrandpa, TParent> Try
        => _try;
    /// <summary>
    /// 原始catch
    /// </summary>
    public CatchBuilder Original
        => _original;
    #endregion
    /// <summary>
    /// 构建分支
    /// </summary>
    /// <returns></returns>
    public CatchClauseSyntax BuildCatch()
        => _original.BuildCatch();
    /// <inheritdoc />
    protected internal override TParent BuildCore()
        => _try.BuildCore();
    /// <summary>
    /// Catch
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="when"></param>
    /// <returns></returns>
    public CatchBuilder<TGrandpa, TParent> Catch(CatchDeclarationSyntax? declaration = null, ExpressionSyntax? when = null)
        => _try.Catch(declaration, when);
    /// <summary>
    /// Finally
    /// </summary>
    /// <returns></returns>
    public FinallyBuilder<TGrandpa, TParent> Finally()
        => _try.Finally();
}