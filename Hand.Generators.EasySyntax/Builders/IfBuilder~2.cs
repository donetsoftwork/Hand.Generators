using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// If
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class IfBuilder<TGrandpa, TParent>(TParent parent, IfBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly IfBuilder _original = original;
    /// <summary>
    /// 原始If
    /// </summary>
    public IfBuilder Original 
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.BuildCurrent());
        return _parent;
    }
    /// <summary>
    /// ElseIf
    /// </summary>
    /// <returns></returns>
    public ElseIfBuilder<TGrandpa, TParent> ElseIf(ExpressionSyntax @if)
        => new(_parent, _original, @if);
    /// <summary>
    /// Else
    /// </summary>
    /// <returns></returns>
    public ElseBuilder<TGrandpa, TParent> Else()
        => new(_parent, _original.Else());
}
