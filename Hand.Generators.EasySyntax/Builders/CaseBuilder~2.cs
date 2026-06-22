using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 条件分支
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="switch"></param>
/// <param name="original"></param>
public class CaseBuilder<TGrandpa, TParent>(SwitchBuilder<TGrandpa, TParent> @switch, CaseBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(@switch.Parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    /// <summary>
    /// switch节点
    /// </summary>
    private readonly SwitchBuilder<TGrandpa, TParent> _switch = @switch;
    private readonly CaseBuilder _original = original;
    /// <summary>
    /// switch节点
    /// </summary>
    public SwitchBuilder<TGrandpa, TParent> Switch
        => _switch;
    /// <summary>
    /// 原始分支
    /// </summary>
    public CaseBuilder Original
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override void AddCore(StatementSyntax statement)
        => _original.AddCore(statement);
    /// <inheritdoc />
    protected internal override TParent BuildCore()
        => _switch.BuildCore();
    /// <summary>
    /// 增加条件
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="when"></param>
    /// <returns></returns>
    public CaseBuilder<TGrandpa, TParent> When(PatternSyntax pattern, WhenClauseSyntax? when = null)
    {
        _original.When(pattern, when);
        return this;
    }
    /// <summary>
    /// 条件分支(新增)
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public CaseBuilder<TGrandpa, TParent> Case(params ExpressionSyntax[] values)
        => _switch.Case(values);
    /// <summary>
    /// 默认分支
    /// </summary>
    /// <returns></returns>
    public SwitchSection<TGrandpa, TParent> Default()
        => new(_switch, _original.Default());
}
