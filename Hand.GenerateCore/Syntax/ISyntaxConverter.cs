using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Syntax;

/// <summary>
/// 语法转化器接口
/// </summary>
public interface ISyntaxConverter
{
    /// <summary>
    /// 转化
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source);
}
