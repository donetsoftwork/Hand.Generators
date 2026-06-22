using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// catch
/// </summary>
/// <param name="try"></param>
/// <param name="declaration"></param>
/// <param name="when"></param>
public class CatchBuilder(TryBuilder @try, CatchDeclarationSyntax? declaration, ExpressionSyntax? when)
    : ScopeBuilder([])
{
    #region 配置
    /// <summary>
    /// try节点
    /// </summary>
    protected readonly TryBuilder _try = @try;
    private readonly CatchDeclarationSyntax? _declaration = declaration;
    private readonly ExpressionSyntax? _when = when;

    /// <summary>
    /// try节点
    /// </summary>
    public TryBuilder Try
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
    /// catch
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="when"></param>
    /// <returns></returns>
    public CatchBuilder Catch(CatchDeclarationSyntax? declaration = null, ExpressionSyntax? when = null)
        => _try.Catch(declaration, when);
    /// <summary>
    /// finally
    /// </summary>
    /// <returns></returns>
    public FinallyBuilder Finally()
        => _try.Finally();
    /// <summary>
    /// 构建catch
    /// </summary>
    /// <returns></returns>
    public CatchClauseSyntax BuildCatch()
    {
        var statement = SyntaxFactory.Block(_statements);
        if (_declaration is null)
            return SyntaxFactory.CatchClause(null, null, statement);
        var filter = _when is null ? null : SyntaxFactory.CatchFilterClause(_when);
        return SyntaxFactory.CatchClause(_declaration, filter, statement);
    }
    /// <inheritdoc />
    public override StatementSyntax Build()
        => _try.Build();
}
