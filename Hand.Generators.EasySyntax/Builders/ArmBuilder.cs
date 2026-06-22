using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// switch表达式分支
/// </summary>
/// <param name="switch"></param>
/// <param name="result"></param>
public class ArmBuilder(PatternSwitchBuilder @switch, ExpressionSyntax result)
{
    #region 配置
    /// <summary>
    /// switch
    /// </summary>
    protected readonly PatternSwitchBuilder _switch = @switch;
    /// <summary>
    /// 结果表达式
    /// </summary>
    protected readonly ExpressionSyntax _result = result;
    /// <summary>
    /// switch
    /// </summary>
    public PatternSwitchBuilder Switch
        => _switch;
    /// <summary>
    /// 结果表达式
    /// </summary>
    public ExpressionSyntax Result
        => _result;
    #endregion
    /// <summary>
    /// 构建分支
    /// </summary>
    /// <returns></returns>
    public virtual SwitchExpressionArmSyntax BuildArm()
        => SyntaxFactory.SwitchExpressionArm(SyntaxFactory.DiscardPattern(), _result);
}
