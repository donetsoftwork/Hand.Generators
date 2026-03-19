using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 插值表达式构造器
/// </summary>
/// <param name="start"></param>
/// <param name="end"></param>
public class InterpolationBuilder(SyntaxKind start, SyntaxKind end)
{
    #region 配置
    private readonly SyntaxKind _start = start;
    private readonly SyntaxKind _end = end;
    private readonly List<InterpolatedStringContentSyntax> _contents = [];
    /// <summary>
    /// 开头
    /// </summary>
    public SyntaxKind Start 
        => _start;
    /// <summary>
    /// 结尾
    /// </summary>
    public SyntaxKind End 
        => _end;
    /// <summary>
    /// 内容
    /// </summary>
    public IReadOnlyCollection<InterpolatedStringContentSyntax> Contents
        => _contents;
    #endregion
    /// <summary>
    /// 添加内容
    /// </summary>
    /// <param name="content"></param>
    public InterpolationBuilder Add(InterpolatedStringContentSyntax content)
    {
        _contents.Add(content);
        return this;
    }
    /// <summary>
    /// 添加字符串片段
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public InterpolationBuilder Add(string text)
        => Add(InterpolatedStringText(text));
    /// <summary>
    /// 添加表达式
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public InterpolationBuilder Add(ExpressionSyntax expression)
    {
//        var interpolation = SyntaxFactory.InterpolatedStringExpression(
//    SyntaxFactory.SingletonSeparatedList<InterpolatedStringContentSyntax>(
//        SyntaxFactory.Interpolation(expression)
//    )
//);
        return Add(SyntaxFactory.Interpolation(expression));
    }
    /// <summary>
    /// 添加格式化表达式
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public InterpolationBuilder Add(ExpressionSyntax expression, string format)
    {
        var interpolationFormat = SyntaxFactory.InterpolationFormatClause(
            SyntaxFactory.Token(SyntaxKind.ColonToken),
            SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.InterpolatedStringTextToken, SymbolDisplay.FormatLiteral(format, false), format, SyntaxTriviaList.Empty));
        return Add(SyntaxFactory.Interpolation(expression, default, interpolationFormat));
    }
    /// <summary>
    /// 构造插值表达式
    /// </summary>
    /// <returns></returns>
    public InterpolatedStringExpressionSyntax Build()
        => SyntaxFactory.InterpolatedStringExpression(SyntaxFactory.Token(_start), SyntaxGenerator.List(_contents), SyntaxFactory.Token(_end));
    /// <summary>
    /// 插值字符串片段
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static InterpolatedStringTextSyntax InterpolatedStringText(string text)
    {
        return SyntaxFactory.InterpolatedStringText(
            SyntaxFactory.Token(
                SyntaxTriviaList.Empty,
                SyntaxKind.InterpolatedStringTextToken,
                SymbolDisplay.FormatLiteral(text, quote: false),
                text,
                SyntaxTriviaList.Empty));
    }
}
