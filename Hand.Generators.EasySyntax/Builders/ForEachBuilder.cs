using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// foreach
/// </summary>
/// <param name="itemType"></param>
/// <param name="item"></param>
/// <param name="collection"></param>
public class ForEachBuilder(TypeSyntax itemType, SyntaxToken item, ExpressionSyntax collection)
    : ScopeBuilder([])
{
    #region 配置
    private readonly TypeSyntax _itemType = itemType;
    private readonly SyntaxToken _item = item;
    private readonly ExpressionSyntax _collection = collection;
    #endregion
    /// <inheritdoc />
    public override StatementSyntax Build()
        => SyntaxFactory.ForEachStatement(_itemType, _item, _collection, Block(_statements));
}
