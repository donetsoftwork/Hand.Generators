using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 转化一个Enumerable到另一个Enumerable
/// </summary>
/// <param name="method"></param>
/// <param name="itemConverter"></param>
/// <param name="isNullable"></param>
public class EnumerableConverter(SimpleNameSyntax method, IConverter itemConverter, bool isNullable = false)
     : ExtensionMethodConverter(UsingLinq, method, isNullable)
{
    #region 配置
    /// <summary>
    /// using System.Linq
    /// </summary>
    public static readonly UsingDirectiveSyntax UsingLinq = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System.Linq"));
    /// <summary>
    /// 子元素转化器
    /// </summary>
    protected readonly IConverter _itemConverter = itemConverter;
    /// <summary>
    /// 子元素转化器
    /// </summary>
    public IConverter ItemConverter
        => _itemConverter;
    private static readonly SimpleNameSyntax _methodName = SyntaxFactory.IdentifierName("Select");
    #endregion
    /// <summary>
    /// 转化一个Enumerable到另一个Enumerable
    /// </summary>
    /// <param name="itemConverter"></param>
    public EnumerableConverter(IConverter itemConverter)
        : this(_methodName, itemConverter)
    {
    }
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [GetLambda(generator, _itemConverter)];
    /// <summary>
    /// 获取lambda表达式参数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ArgumentSyntax GetLambda(SyntaxGenerator generator, IConverter itemConverter)
    {
        var item = SyntaxFactory.IdentifierName("item");
        var lambda = SyntaxFactory.SimpleLambdaExpression(SyntaxFactory.Parameter(item.Identifier), itemConverter.Convert(generator, item));
        return SyntaxFactory.Argument(lambda);
    }
    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static EnumerableConverter Generic(TypeSyntax argumentType, IConverter itemConverter)
        => new(SyntaxGenerator.Generic(_methodName.Identifier, argumentType), itemConverter);
}
