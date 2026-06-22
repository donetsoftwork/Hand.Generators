using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// try
/// </summary>
public class TryBuilder()
    : ScopeBuilder([])
{
    #region 配置
    private readonly List<CatchBuilder> _catches = [];
    private FinallyBuilder? _finally = null;
    #endregion
    /// <summary>
    /// catch
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="when"></param>
    /// <returns></returns>
    public CatchBuilder Catch(CatchDeclarationSyntax? declaration = null, ExpressionSyntax? when = null)
    {
        var @catch = new CatchBuilder(this, declaration, when);
        _catches.Add(@catch);
        return @catch;
    }
    /// <summary>
    /// finally
    /// </summary>
    /// <returns></returns>
    public FinallyBuilder Finally()
        => _finally = new FinallyBuilder(this);
    /// <inheritdoc />
    public override StatementSyntax Build()
    {
        var list = new List<CatchClauseSyntax>();
        foreach (var item in _catches)
            list.Add(item.BuildCatch());

        return SyntaxFactory.TryStatement(SyntaxFactory.Block(_statements), SyntaxGenerator.List(list), _finally?.BuildFinally());
    }
}
