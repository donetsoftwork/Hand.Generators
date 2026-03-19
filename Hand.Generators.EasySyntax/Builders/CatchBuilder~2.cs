using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// catch
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="try"></param>
/// <param name="declaration"></param>
/// <param name="when"></param>
public class CatchBuilder<TGrandpa, TParent>(TryBuilder<TGrandpa, TParent> @try, CatchDeclarationSyntax? declaration, ExpressionSyntax? when)
    : ScopeBuilder<TGrandpa, TParent>(@try.Parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    /// <summary>
    /// try节点
    /// </summary>
    protected readonly TryBuilder<TGrandpa, TParent> _try = @try;
    private readonly CatchDeclarationSyntax? _declaration = declaration;
    private readonly ExpressionSyntax? _when = when;

    /// <summary>
    /// try节点
    /// </summary>
    public TryBuilder<TGrandpa, TParent> Try
        => _try;
    /// <summary>
    /// 异常变量声明
    /// </summary>
    public CatchDeclarationSyntax? Declaration
        => _declaration;
    /// <summary>
    /// 过滤条件
    /// </summary>
    public ExpressionSyntax? When 
        => _when;
    #endregion
    /// <summary>
    /// 构建分支
    /// </summary>
    /// <returns></returns>
    public CatchClauseSyntax BuildCatch()
    {
        var statement = SyntaxFactory.Block(_statements);
        if(_declaration is null)
            return SyntaxFactory.CatchClause(null, null, statement);
        var filter = _when is null ? null : SyntaxFactory.CatchFilterClause(_when);
        return SyntaxFactory.CatchClause(_declaration, filter, statement);
    }
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