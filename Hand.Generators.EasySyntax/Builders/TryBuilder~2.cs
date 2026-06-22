using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// try
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class TryBuilder<TGrandpa, TParent>(TParent parent, TryBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly TryBuilder _original = original;
    /// <summary>
    /// 原始try
    /// </summary>
    public TryBuilder Original
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.Build());
        return _parent;
    }
    /// <summary>
    /// catch
    /// </summary>
    /// <param name="declaration"></param>
    /// <param name="when"></param>
    /// <returns></returns>
    public CatchBuilder<TGrandpa, TParent> Catch(CatchDeclarationSyntax? declaration = null, ExpressionSyntax? when = null)
        => new(this, _original.Catch(declaration, when));
    /// <summary>
    /// finally
    /// </summary>
    /// <returns></returns>
    public FinallyBuilder<TGrandpa, TParent> Finally()
        => new(this, _original.Finally());
}