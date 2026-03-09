using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// try
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
public class TryBuilder<TGrandpa, TParent>(TParent parent)
    : BlockBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly List<CatchBuilder<TGrandpa, TParent>> _catches = [];
    private FinallyBuilder<TGrandpa, TParent>? _finally = null;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        var list = new List<CatchClauseSyntax>();
        foreach (var item in _catches)
            list.Add(item.BuildCatch());

        var statement = SyntaxFactory.TryStatement(SyntaxFactory.Block(_statements), SyntaxGenerator.List(list), _finally?.BuildFinally());
        _parent.AddCore(statement);
        return _parent;
    }
    /// <summary>
    /// Catch
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="when"></param>
    /// <returns></returns>
    public CatchBuilder<TGrandpa, TParent> Catch(CatchDeclarationSyntax? declaration = null, ExpressionSyntax? when = null)
    {
        var @catch = new CatchBuilder<TGrandpa, TParent>(this, declaration, when);
        _catches.Add(@catch);
        return @catch;
    }
    /// <summary>
    /// Finally
    /// </summary>
    /// <returns></returns>
    public FinallyBuilder<TGrandpa, TParent> Finally()
        => _finally = new FinallyBuilder<TGrandpa, TParent>(this);
}