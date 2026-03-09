using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// while
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="condition"></param>
public class WhileBuilder<TGrandpa, TParent>(TParent parent, ExpressionSyntax condition)
    : BlockBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly ExpressionSyntax _condition = condition;
    #endregion
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        var statement = SyntaxFactory.WhileStatement(_condition, Block(_statements));
        _parent.AddCore(statement);
        return _parent;
    }
}
