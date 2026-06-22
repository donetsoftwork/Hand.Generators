using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// ElseIf
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class ElseIfBuilder<TGrandpa, TParent>(TParent parent, ElseIfBuilder original)
    : IfBuilder<TGrandpa, TParent>(parent, original)
    where TParent : StatementBuilder<TGrandpa>
{
    /// <summary>
    /// ElseIf
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="if"></param>
    /// <param name="condition"></param>
    public ElseIfBuilder(TParent parent, IfBuilder @if, ExpressionSyntax condition)
        : this(parent, new ElseIfBuilder(@if, condition))
    {
    }
}
