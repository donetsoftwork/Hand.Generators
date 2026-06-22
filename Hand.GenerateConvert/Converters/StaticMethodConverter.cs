using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Converters;

/// <summary>
/// 使用静态方法转化
/// </summary>
/// <param name="method"></param>
public class StaticMethodConverter(ExpressionSyntax method)
    : IConverter
{
    /// <summary>
    /// 使用静态方法转化
    /// </summary>
    /// <param name="methodName"></param>
    public StaticMethodConverter(string methodName)
        : this(SyntaxFactory.IdentifierName(methodName))
    {
    }
    #region 配置
    private readonly ExpressionSyntax _method = method;
    /// <summary>
    /// 方法
    /// </summary>
    public ExpressionSyntax Method 
        => _method;
    #endregion
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual IEnumerable<ExpressionSyntax> CreateArguments(ExpressionSyntax source)
        => [source];

    /// <inheritdoc />
    public virtual ExpressionSyntax Convert(ExpressionSyntax source)
    {
        return GetMethod()
            .Invocation(CreateArguments(source));
    }
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <returns></returns>
    protected virtual ExpressionSyntax GetMethod()
        => _method;
}
