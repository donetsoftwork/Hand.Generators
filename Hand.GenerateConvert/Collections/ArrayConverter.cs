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
     : CollectionStaticMethodConverter(method, itemConverter)
{
    #region 配置
    private static readonly SyntaxToken _methodName = SyntaxFactory.Identifier("System.Array.ConvertAll");
    #endregion
    /// <summary>
    /// 转化一个数组到另一个数组
    /// </summary>
    /// <param name="itemConverter"></param>
    public ArrayConverter(IConverter itemConverter)
        : this(SyntaxFactory.IdentifierName(_methodName), itemConverter)
    {
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
