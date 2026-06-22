using Hand.Converters;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand;

/// <summary>
/// 转化服务
/// </summary>
public static class ConvertServices
{
    /// <summary>
    /// 检查空值并提供默认值
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="defaultExpression"></param>
    /// <returns></returns>
    public static ExpressionSyntax CheckNull(this ExpressionSyntax expression, ExpressionSyntax? defaultExpression)
    {
        if (defaultExpression is null)
            return expression;
        return expression.NullCoalesce(defaultExpression);
    }
    /// <summary>
    /// 检查空值并提供默认值
    /// </summary>
    /// <param name="converter"></param>
    /// <param name="defaultExpression"></param>
    /// <returns></returns>
    public static IConverter CheckNull(this IConverter converter, ExpressionSyntax? defaultExpression)
    {
        if (defaultExpression is null)
            return converter;
        return new NullableConverter(converter, defaultExpression);
    }
}
