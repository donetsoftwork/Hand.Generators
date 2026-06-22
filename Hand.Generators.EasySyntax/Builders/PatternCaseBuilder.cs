using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 模式分支
/// </summary>
/// <param name="switch"></param>
/// <param name="pattern"></param>
public class PatternCaseBuilder(SwitchBuilder @switch, PatternSyntax pattern)
    : SwitchSection(@switch)
{

    #region 配置
    private readonly PatternSyntax _pattern = pattern;
    private readonly List<ExpressionSyntax> _conditons = [];
    /// <summary>
    /// 模式
    /// </summary>
    public PatternSyntax Pattern 
        => _pattern;
    #endregion
    /// <summary>
    /// 增加条件
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public PatternCaseBuilder When(ExpressionSyntax condition)
    {
        _conditons.Add(condition);
        return this;
    }
    /// <inheritdoc />
    protected override IEnumerable<SwitchLabelSyntax> GetLabels()
        => [SyntaxFactory.CasePatternSwitchLabel(_pattern, CreateWhen(_conditons), SyntaxFactory.Token(SyntaxKind.ColonToken))];
    /// <summary>
    /// 构造when
    /// </summary>
    /// <param name="conditons"></param>
    /// <returns></returns>
    public static WhenClauseSyntax? CreateWhen(List<ExpressionSyntax> conditons)
    {
        var count = conditons.Count;
        if(count == 0)
            return null;
        var conditon = conditons[0];
        for( var i = 1; i < count; i++)
            conditon = conditon.LogicalOr(conditons[i]);
        return SyntaxFactory.WhenClause(conditon);
    }
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
    /// 默认分支
    /// </summary>
    /// <returns></returns>
    public SwitchSection Default()
        => _switch.Default();
}
