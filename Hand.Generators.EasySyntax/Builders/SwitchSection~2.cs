using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 分支
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="switch"></param>
/// <param name="original"></param>
public class SwitchSection<TGrandpa, TParent>(SwitchBuilder<TGrandpa, TParent> @switch, SwitchSection original)
    : ScopeBuilder<TGrandpa, TParent>(@switch.Parent, original._statements)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    /// <summary>
    /// switch节点
    /// </summary>
    private readonly SwitchBuilder<TGrandpa, TParent> _switch = @switch;
    private readonly SwitchSection _original = original;
    /// <summary>
    /// switch节点
    /// </summary>
    public SwitchBuilder<TGrandpa, TParent> Switch
        => _switch;
    /// <summary>
    /// 原始分支
    /// </summary>
    public SwitchSection Original
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override void AddCore(StatementSyntax statement)
        => _original.AddCore(statement);
    /// <inheritdoc />
    protected internal override TParent BuildCore()
        => _switch.BuildCore();
}
