using Hand.Converters;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 转化Enumerable为数组
/// </summary>
public class EnumerableToListConverter(SimpleNameSyntax method)
     : ExtensionMethodConverter(EnumerableConverter.UsingLinq, method)
{
    private static readonly SimpleNameSyntax _methodName = SyntaxFactory.IdentifierName("ToList");
    /// <summary>
    /// 转化Enumerable为数组
    /// </summary>
    public EnumerableToListConverter()
        : this(_methodName)
    {
    }

    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <returns></returns>
    public static EnumerableToListConverter Generic(TypeSyntax argumentType)
        => new(SyntaxGenerator.Generic(_methodName.Identifier, argumentType));

    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly EnumerableToListConverter Instance = new();
}