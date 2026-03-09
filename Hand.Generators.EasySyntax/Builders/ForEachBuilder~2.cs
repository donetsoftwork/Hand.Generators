using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// foreach
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="itemType"></param>
/// <param name="item"></param>
/// <param name="collection"></param>
public class ForEachBuilder<TGrandpa, TParent>(TParent parent, TypeSyntax itemType, SyntaxToken item, ExpressionSyntax collection)
    : BlockBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly TypeSyntax _itemType = itemType;
    private readonly SyntaxToken _item = item;
    private readonly ExpressionSyntax _collection = collection;
    #endregion

    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        var statement = SyntaxFactory.ForEachStatement(_itemType, _item, _collection, Block(_statements));
        _parent.AddCore(statement);
        return _parent;
    }
}
