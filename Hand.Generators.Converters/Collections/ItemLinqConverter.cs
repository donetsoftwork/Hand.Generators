using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 转化一个Enumerable到另一个Enumerable
/// </summary>
/// <param name="method"></param>
/// <param name="itemConverter"></param>
public class ItemLinqConverter(ISyntaxDisplay<SimpleNameSyntax> method, ISyntaxConverter itemConverter)
     : LinqConverter(method)
{
    #region 配置
    private static readonly SyntaxWrapper<SimpleNameSyntax> _select = new(SyntaxFactory.IdentifierName("Select"));

    /// <summary>
    /// 子元素转化器
    /// </summary>
    protected readonly ISyntaxConverter _itemConverter = itemConverter;
    /// <summary>
    /// 子元素转化器
    /// </summary>
    public ISyntaxConverter ItemConverter
        => _itemConverter;
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [GetLambda(generator, _itemConverter)];
    /// <summary>
    /// 获取lambda表达式参数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ArgumentSyntax GetLambda(SyntaxGenerator generator, ISyntaxConverter itemConverter)
    {
        var item = SyntaxFactory.IdentifierName("item");
        var lambda = SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(item.Identifier), itemConverter.Convert(generator, item));
        return SyntaxFactory.Argument(lambda);
    }

    #region Select
    /// <summary>
    /// Select
    /// </summary>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ItemLinqConverter Select(ISyntaxConverter itemConverter)
        => new(_select, itemConverter);
    /// <summary>
    /// 泛型Select
    /// </summary>
    /// <param name="argumentType"></param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ItemLinqConverter Select(ISyntaxDisplay<TypeSyntax> argumentType, ISyntaxConverter itemConverter)
        => new(new GenericDisplay(_select.Original.Identifier, argumentType), itemConverter);
    #endregion
    #region ToList
    /// <summary>
    /// ToList
    /// </summary>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ItemLinqConverter ToList(ISyntaxConverter itemConverter)
        => new(_toList, itemConverter);
    /// <summary>
    /// 泛型ToList
    /// </summary>
    /// <param name="argumentType"></param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ItemLinqConverter ToList(ISyntaxDisplay<TypeSyntax> argumentType, ISyntaxConverter itemConverter)
        => new(new GenericDisplay(_toList.Original.Identifier, argumentType), itemConverter);
    #endregion
    #region ToArray
    /// <summary>
    /// ToArray
    /// </summary>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ItemLinqConverter ToArray(ISyntaxConverter itemConverter)
        => new(_toArray, itemConverter);
    /// <summary>
    /// 泛型ToArray
    /// </summary>
    /// <param name="argumentType"></param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ItemLinqConverter ToArray(ISyntaxDisplay<TypeSyntax> argumentType, ISyntaxConverter itemConverter)
        => new(new GenericDisplay(_toArray.Original.Identifier, argumentType), itemConverter);
    #endregion
}
