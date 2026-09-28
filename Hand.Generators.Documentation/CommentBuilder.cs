using Hand.Creational;
using Hand.Members;
using System.Collections.Generic;

namespace Hand.Documentation;

/// <summary>
/// 注册构造器
/// </summary>
public class CommentBuilder
    : IMemberBuilder<Comment>
{
    private readonly Comment _original = new();
    /// <summary>
    /// 原始对象
    /// </summary>
    public Comment Original
        => _original;
    /// <inheritdoc />
    public Comment Build()
        => _original;
    /// <inheritdoc />
    public void Save<TMember>(string name, TMember value)
    {
        switch (name)
        {
            case "summary":
                if (value is string summaryValue)
                    _original.Summary = summaryValue;
                break;
            case "typeparam":
                if (value is Dictionary<string, string> typeparamValue)
                    _original.TypeParams = typeparamValue;
                break;
            case "param":
                if (value is Dictionary<string, string> paramValue)
                    _original.Params = paramValue;
                break;
            case "returns":
                if (value is string returnsValue)
                    _original.Returns = returnsValue;
                break;
        }
    }
    /// <summary>
    /// 工厂模式
    /// </summary>
    public static readonly ICreator<CommentBuilder> Creater = new DefaultCreater<CommentBuilder>();
}
