using Hand.Convert;
using Hand.Creational;
using Hand.Members;
using Hand.ParseXml;
using Hand.ParseXml.Move;
using Hand.ParseXml.Nodes;
using Microsoft.CodeAnalysis;
using System.Xml;

namespace Hand.Documentation;

/// <summary>
/// 注释解析器
/// </summary>
public class CommentParser()
    : EntityParser<Comment>(HandXml.Default, CommentBuilder.Creater, null, true)
{
    #region 配置
    private static readonly ContentReader _content = new(string.Empty);
    private static readonly IndexAttributeReader _name = new(0);
    private static readonly MoveToParser<string> _summaryParser = new("summary", _content, string.Empty);

    /// <summary>
    /// Summary解析器
    /// </summary>
    public static IParser<XmlReader, string> SummaryParser
        => _summaryParser;
    #endregion
    /// <inheritdoc />
    public override void ReadAttributes(IMemberStore entity, XmlReader reader) { }
    /// <inheritdoc />
    public override void ReadItem(IMemberBuilder<Comment> entity, XmlReader reader)
    {
        if (entity is CommentBuilder builder)
            ReadItem(builder.Original, reader);
        else
            base.ReadItem(entity, reader);
    }
    /// <summary>
    /// 使用自定义构造器
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="reader"></param>
    public static void ReadItem(Comment entity, XmlReader reader)
    {
        switch (reader.LocalName)
        {
            case "summary":
                if (_content.TryParse(reader, out var summaryResult))
                    entity.Summary = summaryResult;
                break;
            case "typeparam":
                if (_name.TryParse(reader, out var typeparamName) &&  _content.TryParse(reader, out var typeparamResult))
                    entity.AddTypeParam(typeparamName, typeparamResult);
                break;
            case "param":
                if (_name.TryParse(reader, out var paramName) && _content.TryParse(reader, out var paramResult))
                    entity.AddParam(paramName, paramResult);
                break;
            case "returns":
                if (_content.TryParse(reader, out var returnsResult))
                    entity.Returns = returnsResult;
                break;
        }
    }
    /// <summary>
    /// 单列
    /// </summary>
    public static readonly CommentParser Instance = new();
    /// <summary>
    /// 获取注释
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static Comment Comment(ISymbol symbol)
    {
        var xml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml))
            return new();
        return Instance.Get(xml!);
    }
    /// <summary>
    /// 获取备注
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetSummary(ISymbol symbol, string defaultValue = "")
    {
        var xml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml))
            return defaultValue;
        if(_summaryParser.TryParse(xml!, out var summary))
            return summary.Trim();
        return defaultValue;
    }
}
