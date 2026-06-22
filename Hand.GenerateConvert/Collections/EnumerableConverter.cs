using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Collections;

/// <summary>
/// 转化一个Enumerable到另一个Enumerable
/// </summary>
public class EnumerableConverter(ExpressionSyntax method, IConverter itemConverter)
     : CollectionStaticMethodConverter(method, itemConverter)
{
    #region 配置
    private static readonly SyntaxToken _methodName = SyntaxFactory.Identifier("System.Linq.Enumerable.Select");
    #endregion
    /// <summary>
    /// 转化一个Enumerable到另一个Enumerable
    /// </summary>
    /// <param name="itemConverter"></param>
    public EnumerableConverter(IConverter itemConverter)
        : this(SyntaxFactory.IdentifierName(_methodName), itemConverter)
    {
    }
    /// <summary>
    /// 泛型方法
    /// </summary>
    /// <param name="argumentType">类型参数</param>
    /// <param name="itemConverter"></param>
    /// <returns></returns>
    public static EnumerableConverter Generic(TypeSyntax argumentType, IConverter itemConverter)
        => new(SyntaxGenerator.Generic(_methodName, argumentType), itemConverter);
}
