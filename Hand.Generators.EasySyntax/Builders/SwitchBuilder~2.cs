using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// Switch
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="original"></param>
public class SwitchBuilder<TGrandpa, TParent>(TParent parent, SwitchBuilder original)
    : ScopeBuilder<TGrandpa, TParent>(parent, [])
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly SwitchBuilder _original = original;
    /// <summary>
    /// 原始分支
    /// </summary>
    public SwitchBuilder Original
        => _original;
    #endregion
    /// <inheritdoc />
    protected internal override void AddCore(StatementSyntax statement)
        => _original.AddCore(statement);
    /// <summary>
    /// 分支
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public CaseBuilder<TGrandpa, TParent> Case(params ExpressionSyntax[] values)
        => new(this, _original.Case(values));
    /// <summary>
    /// 默认分支
    /// </summary>
    /// <returns></returns>
    public SwitchSection<TGrandpa, TParent> Default()
        => new(this, _original.Default());
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        _parent.AddCore(_original.Build());
        return _parent;
    }
}
