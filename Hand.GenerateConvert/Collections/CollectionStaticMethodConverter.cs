using Hand.Converters;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Collections;

/// <summary>
/// 集合静态方法转化
/// </summary>
/// <param name="method"></param>
/// <param name="itemConverter"></param>
public class CollectionStaticMethodConverter(ExpressionSyntax method, IConverter itemConverter)
     : StaticMethodConverter(method)
{
    #region 配置
    private readonly IConverter _itemConverter = itemConverter;
    /// <summary>
    /// 子元素转化器
    /// </summary>
    public IConverter ItemConverter
        => _itemConverter;
    #endregion
    /// <inheritdoc />
    protected override IEnumerable<ExpressionSyntax> CreateArguments(ExpressionSyntax source)
    {
        var item = SyntaxFactory.IdentifierName("item");
        var lambda = SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(item.Identifier), _itemConverter.Convert(item));
        return [source, lambda];
    }
}