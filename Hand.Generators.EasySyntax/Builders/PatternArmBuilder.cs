using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// Switch表达式条件分支
/// </summary>
/// <param name="switch"></param>
/// <param name="pattern"></param>
/// <param name="result"></param>
public class PatternArmBuilder(PatternSwitchBuilder @switch, PatternSyntax pattern, ExpressionSyntax result)
    : ArmBuilder(@switch, result)
{
    #region 配置
    private readonly PatternSyntax _pattern = pattern;
    private readonly List<ExpressionSyntax> _conditons = [];
    /// <summary>
    /// 模式表达式
    /// </summary>
    public PatternSyntax Pattern
        => _pattern;
    #endregion
    /// <summary>
    /// 增加条件
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public PatternArmBuilder When(ExpressionSyntax condition)
    {
        _conditons.Add(condition);
        return this;
    }
    /// <summary>
    /// 模式分支
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public PatternArmBuilder Case(PatternSyntax pattern, ExpressionSyntax result)
        => _switch.Case(pattern, result);
    /// <summary>
    /// 模式分支
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public PatternArmBuilder Case(ExpressionSyntax pattern, ExpressionSyntax result)
         => _switch.Case(pattern, result);
    /// <summary>
    /// 默认分支
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public DefaultArmBuilder Default(ExpressionSyntax result)
        => _switch.Default(result);
    /// <inheritdoc />
    public override SwitchExpressionArmSyntax BuildArm()
        => SyntaxFactory.SwitchExpressionArm(_pattern, PatternCaseBuilder.CreateWhen(_conditons), _result);
}
