using Hand.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 文档
/// </summary>
public partial class SyntaxGenerator
{
    #region Comment
    private static readonly char[] _lineSeparator = ['\r', '\n'];
    /// <summary>
    /// 增加换行
    /// </summary>
    /// <param name="continueComment"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static XmlTextSyntax XmlNewLine(bool continueComment)
        => SyntaxFactory.XmlText(SyntaxFactory.XmlTextNewLine("\r\n", continueComment));
    /// <summary>
    /// 处理摘要内容
    /// </summary>
    /// <param name="summary"></param>
    /// <returns></returns>
    private static List<XmlTextSyntax> CheckSummaryContents(string summary)
    {
        if (string.IsNullOrEmpty(summary))
            return [];
        return [XmlNewLine(true), SyntaxFactory.XmlText(summary), XmlNewLine(true)];
    }
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="params"></param>
    private static void AddTypeParam(List<XmlNodeSyntax> elements, Dictionary<string, string> @params)
    {
        foreach (var nv in @params)
        {
            elements.Add(XmlNewLine(true));
            elements.Add(CreateTypeParamElement(nv.Key, nv.Value));
        }
    }
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="params"></param>
    private static void AddParam(List<XmlNodeSyntax> elements, Dictionary<string, string> @params)
    {
        foreach (var nv in @params)
        {
            elements.Add(XmlNewLine(true));
            elements.Add(CreateParamElement(nv.Key, nv.Value));
        }
    }
    private static void AddReturns(List<XmlNodeSyntax> elements, string returns)
    {
        if (string.IsNullOrWhiteSpace(returns))
            return;
        elements.Add(XmlNewLine(true));
        elements.Add(SyntaxFactory.XmlReturnsElement(SyntaxFactory.XmlText(returns)));
    }
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static XmlElementSyntax CreateParamElement(string name, string value)
        => SyntaxFactory.XmlParamElement(name, SyntaxFactory.XmlText(value));
    /// <summary>
    /// 构造类型参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static XmlElementSyntax CreateTypeParamElement(string name, string value)
    {
        XmlElementSyntax element = SyntaxFactory.XmlElement("typeparam", SyntaxFactory.SingletonList<XmlNodeSyntax>(SyntaxFactory.XmlText(value)));
        return element.WithStartTag(element.StartTag.AddAttributes(SyntaxFactory.XmlNameAttribute(name)));
    }
    /// <summary>
    /// 处理摘要内容
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    private static List<XmlTextSyntax> CheckSummaryContents(string[] lines)
    {
        var count = lines.Length;
        switch (count)
        {
            case 0: return [];
            case 1: return CheckSummaryContents(lines[0]);
        }
        var contents = new List<XmlTextSyntax>(count + 2)
        {
            XmlNewLine(true)
        };
        foreach (var summary in lines)
            contents.Add(SyntaxFactory.XmlText(summary));
        contents.Add(XmlNewLine(true));
        return contents;
    }
    /// <summary>
    /// 构造摘要备注
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
    public static XmlElementSyntax? CreateSummary(params string[] lines)
    {
        var texts = CheckSummaryContents(lines);
        if (texts.Count == 0)
            return null;
        return SyntaxFactory.XmlSummaryElement(SyntaxFactory.List<XmlNodeSyntax>(texts));
    }
    /// <summary>
    /// 构造摘要文档
    /// </summary>
    /// <param name="summary"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DocumentationCommentTriviaSyntax CreateDocumentation(XmlElementSyntax summary)
        => SyntaxFactory.DocumentationComment(summary, XmlNewLine(false));
    /// <summary>
    /// 构造摘要文档
    /// </summary>
    /// <param name="summaryLines"></param>
    /// <returns></returns>
    public static DocumentationCommentTriviaSyntax? CreateDocumentation(params string[] summaryLines)
    {
        var summary = CreateSummary(summaryLines);
        if (summary is null)
            return null;
        return SyntaxFactory.DocumentationComment(summary, XmlNewLine(false));
    }
    /// <summary>
    /// 构造注释文档
    /// </summary>
    /// <param name="comment"></param>
    /// <returns></returns>
    public static DocumentationCommentTriviaSyntax? CreateDocumentation(Comment comment)
    {
        var summary = CreateSummary(comment.Summary.Split(_lineSeparator, StringSplitOptions.RemoveEmptyEntries));
        if (summary is null)
            return null;
        var typeParams = comment.TypeParams;
        var @params = comment.Params;
        var elements = new List<XmlNodeSyntax>(((typeParams.Count + @params.Count) << 1) + 3) { summary };
        AddTypeParam(elements, typeParams);
        AddParam(elements, @params);
        AddReturns(elements, comment.Returns);
        return SyntaxFactory.DocumentationComment([.. elements, XmlNewLine(false)]);
    }
    #endregion
}
