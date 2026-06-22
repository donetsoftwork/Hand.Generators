using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 模式switch构造器
/// </summary>
/// <param name="governing"></param>
public class PatternSwitchBuilder(ExpressionSyntax governing)
{
    #region 配置
    private readonly ExpressionSyntax _governing = governing;
    /// <summary>
    /// 当前控制
    /// </summary>
    public ExpressionSyntax Governing
        => _governing;
    private readonly List<ArmBuilder> _arms = [];
    #endregion
    /// <summary>
    /// 模式分支
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public PatternArmBuilder Case(PatternSyntax pattern, ExpressionSyntax result)
        => Arm(new PatternArmBuilder(this, pattern, result));
    /// <summary>
    /// 模式分支
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public PatternArmBuilder Case(ExpressionSyntax pattern, ExpressionSyntax result)
        => Case(pattern.ToPattern(), result);
    /// <summary>
    /// 默认分支
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public DefaultArmBuilder Default(ExpressionSyntax result)
        => Arm(new DefaultArmBuilder(this, result));
    /// <summary>
    /// 构造switch表达式
    /// </summary>
    /// <returns></returns>
    public SwitchExpressionSyntax Build()
        => SyntaxFactory.SwitchExpression(_governing, SyntaxFactory.SeparatedList(_arms.Select(builder => builder.BuildArm())));
    /// <summary>
    /// 添加分支
    /// </summary>
    /// <param name="arm"></param>
    internal TArm Arm<TArm>(TArm arm)
        where TArm : ArmBuilder
    {
        _arms.Add(arm);
        return arm;
    }
}
