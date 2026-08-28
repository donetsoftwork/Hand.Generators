using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 获取集合第一个或默认值
/// 调用FirstOrDefault&lt;TSource&gt;(IEnumerable&lt;TSource&gt;, TSource)
/// .NET支持版本 6, 7, 8, 9, 10, 11
/// </summary>
/// <param name="method"></param>
/// <param name="defaultValue"></param>
public class CollectionFirstOrDefaultConverter(SimpleNameSyntax method, ArgumentSyntax defaultValue)
     : ExtensionMethodConverter(EnumerableConverter.UsingLinq, method)
{
    #region 配置
    private readonly ArgumentSyntax _defaultValue = defaultValue;
    private static readonly SimpleNameSyntax _methodName = SyntaxFactory.IdentifierName("FirstOrDefault");
    #endregion
    /// <summary>
    /// 转化Enumerable为数组
    /// </summary>
    /// <param name="defaultValue"></param>
    public CollectionFirstOrDefaultConverter(ExpressionSyntax defaultValue)
        : this(_methodName, SyntaxFactory.Argument(defaultValue))
    {
    }
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [_defaultValue];
    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static CollectionFirstOrDefaultConverter Generic(TypeSyntax argumentType, ExpressionSyntax defaultValue)
        => new(SyntaxGenerator.Generic(_methodName.Identifier, argumentType), SyntaxFactory.Argument(defaultValue));
}
