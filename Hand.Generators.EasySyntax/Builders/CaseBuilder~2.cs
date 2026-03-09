using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 条件分支
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="switch"></param>
/// <param name="value"></param>
public class CaseBuilder<TGrandpa, TParent>(SwitchBuilder<TGrandpa, TParent> @switch, ExpressionSyntax value)
    : SwitchSection<TGrandpa, TParent>(@switch, SyntaxFactory.CaseSwitchLabel(value))
     where TParent : StatementBuilder<TGrandpa>
{
    /// <summary>
    /// 条件分支(新增)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CaseBuilder<TGrandpa, TParent> Case(ExpressionSyntax value)
        => _switch.Case(value);
    /// <summary>
    /// 默认分支
    /// </summary>
    /// <returns></returns>
    public SwitchSection<TGrandpa, TParent> Default()
        => _switch.Section(new SwitchSection<TGrandpa, TParent>(_switch, SyntaxFactory.DefaultSwitchLabel()));
}
