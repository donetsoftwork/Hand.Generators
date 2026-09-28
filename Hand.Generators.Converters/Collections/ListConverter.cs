using Hand.Converters.Methods;
using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 转化一个List到另一个List
/// </summary>
/// <param name="method"></param>
/// <param name="itemConverter"></param>
public class ListConverter(ISyntaxDisplay<SimpleNameSyntax> method, ISyntaxConverter itemConverter)
     : MethodConverter(method)
{
    #region 配置
    private static readonly SyntaxWrapper<SimpleNameSyntax> _methodName = new(SyntaxFactory.IdentifierName("ConvertAll"));
    private readonly ISyntaxConverter _itemConverter = itemConverter;
    /// <summary>
    /// 子元素转化器
    /// </summary>
    public ISyntaxConverter ItemConverter
        => _itemConverter;
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [ItemLinqConverter.GetLambda(generator, _itemConverter)];

    #region Create
    /// <summary>
    /// 构造数组转化器
    /// </summary>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ListConverter Create(ISyntaxConverter itemConverter)
        => new(_methodName, itemConverter);
    /// <summary>
    /// 构造泛型数组转化器
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ListConverter Create(ISyntaxDisplay<TypeSyntax> argumentType, ISyntaxConverter itemConverter)
        => new(new GenericDisplay(_methodName.Original.Identifier, argumentType), itemConverter);
    #endregion
}
