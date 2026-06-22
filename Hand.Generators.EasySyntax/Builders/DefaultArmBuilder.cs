using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 默认分支
/// </summary>
/// <param name="switch"></param>
/// <param name="result"></param>
public class DefaultArmBuilder(PatternSwitchBuilder @switch, ExpressionSyntax result)
    : ArmBuilder(@switch, result)
{
    /// <summary>
    /// 构造switch表达式
    /// </summary>
    /// <returns></returns>
    public SwitchExpressionSyntax Build()
        => _switch.Build();
}
