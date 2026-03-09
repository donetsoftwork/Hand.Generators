using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// lock
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="expression"></param>
public class LockBuilder<TGrandpa, TParent>(TParent parent, ExpressionSyntax expression)
    : BlockBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly ExpressionSyntax _expression = expression;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        var statement = SyntaxFactory.LockStatement(_expression, Block(_statements));
        _parent.AddCore(statement);
        return _parent;
    }
}
