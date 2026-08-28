using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [SyntaxFactory.Argument(source)];

    /// <inheritdoc />
    public virtual ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        return GetMethod(generator)
            .Invocation(CreateArguments(generator, source));
    }
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <returns></returns>
    protected virtual ExpressionSyntax GetMethod(SyntaxGenerator generator)
        => _method;
}
