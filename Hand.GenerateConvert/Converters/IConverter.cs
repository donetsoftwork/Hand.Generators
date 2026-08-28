using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 转化器接口
/// </summary>
public interface IConverter
{
    /// <summary>
    /// 转化方法
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source);
    ///// <summary>
    ///// 获取可空转化器
    ///// </summary>
    ///// <param name="defaultExpression">默认值</param>
    ///// <returns></returns>
    //IConverter Nullable(ExpressionSyntax? defaultExpression);
}
