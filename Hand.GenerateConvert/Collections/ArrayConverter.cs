using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 转化一个数组到另一个数组
/// </summary>
/// <param name="method"></param>
/// <param name="itemConverter"></param>
public class ArrayConverter(ExpressionSyntax method, IConverter itemConverter)
     : StaticMethodConverter(method)
{
    #region 配置
    private static readonly SyntaxToken _methodName = SyntaxFactory.Identifier("Array.ConvertAll");
    private readonly IConverter _itemConverter = itemConverter;
    /// <summary>
    /// 子元素转化器
    /// </summary>
    public IConverter ItemConverter
        => _itemConverter;
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [SyntaxFactory.Argument(source), EnumerableConverter.GetLambda(generator, _itemConverter)];
    /// <summary>
    /// 转化一个数组到另一个数组
    /// </summary>
    /// <param name="itemConverter"></param>
    public ArrayConverter(IConverter itemConverter)
        : this(SyntaxFactory.IdentifierName(_methodName), itemConverter)
    {
    }
    /// <inheritdoc />
    public override ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        generator.UsingSystem();
        return base.Convert(generator, source);
    }
    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static ArrayConverter Generic(TypeSyntax argumentType, IConverter itemConverter)
        => new(SyntaxGenerator.Generic(_methodName, argumentType), itemConverter);
}
