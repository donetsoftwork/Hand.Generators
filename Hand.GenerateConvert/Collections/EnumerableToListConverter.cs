using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 转化Enumerable为数组
/// </summary>
public class EnumerableToListConverter(ExpressionSyntax method)
     : StaticMethodConverter(method)
{
    private static readonly SyntaxToken _methodName = SyntaxFactory.Identifier("System.Linq.Enumerable.ToList");
    /// <summary>
    /// 转化Enumerable为数组
    /// </summary>
    public EnumerableToListConverter()
        : this(SyntaxFactory.IdentifierName(_methodName))
    {
    }

    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <returns></returns>
    public static EnumerableToListConverter Generic(TypeSyntax argumentType)
        => new(SyntaxGenerator.Generic(_methodName, argumentType));
}