using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// case
/// </summary>
/// <param name="switch"></param>
/// <param name="labels"></param>
public class CaseBuilder(SwitchBuilder @switch, List<SwitchLabelSyntax> labels)
    : SwitchSection(@switch)
{
    /// <summary>
    /// case
    /// </summary>
    /// <param name="switch"></param>
    /// <param name="values"></param>
    public CaseBuilder(SwitchBuilder @switch, params ExpressionSyntax[] values)
        : this(@switch, CreateLabels(values))
    {
    }
    #region 配置
    private readonly List<SwitchLabelSyntax>  _labels = labels;
    /// <summary>
    /// 分支
    /// </summary>
    public IEnumerable<SwitchLabelSyntax> Labels
        => _labels;
    #endregion
    /// <inheritdoc />
    protected override SyntaxList<SwitchLabelSyntax> GetLabels()
        => SyntaxFactory.List(_labels);
    /// <summary>
    /// 条件分支(新增)
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public CaseBuilder Case(params ExpressionSyntax[] values)
        => _switch.Case(values);
    /// <summary>
    /// 模式分支
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public PatternCaseBuilder Case(PatternSyntax pattern)
        => _switch.Case(pattern);
    /// <summary>
    /// 增加条件
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="when"></param>
    /// <returns></returns>
    public CaseBuilder When(PatternSyntax pattern, WhenClauseSyntax? when = null)
    {
        _labels.Add(SyntaxFactory.CasePatternSwitchLabel(pattern, when, SyntaxFactory.Token(SyntaxKind.ColonToken)));
        return this;
    }
    /// <summary>
    /// 默认分支
    /// </summary>
    /// <returns></returns>
    public SwitchSection Default()
        => _switch.Default();
    /// <summary>
    /// 构造标签
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<SwitchLabelSyntax> CreateLabels(params ExpressionSyntax[] values)
    {
        var labels = new List<SwitchLabelSyntax>(values.Length);
        foreach (var value in values)
            labels.Add(SyntaxFactory.CaseSwitchLabel(value));
        return labels;
    }
}
