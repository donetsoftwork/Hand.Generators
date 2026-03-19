using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 分支
/// </summary>
/// <typeparam name="TGrandpa"></typeparam>
/// <typeparam name="TParent"></typeparam>
/// <param name="switch"></param>
/// <param name="label"></param>
public class SwitchSection<TGrandpa, TParent>(SwitchBuilder<TGrandpa, TParent> @switch, SwitchLabelSyntax label)
    : ScopeBuilder<TGrandpa, TParent>(@switch.Parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    /// <summary>
    /// Switch节点
    /// </summary>
    protected readonly SwitchBuilder<TGrandpa, TParent> _switch = @switch;
    private readonly SwitchLabelSyntax _label = label;
    private bool _isReturn = false;
    /// <summary>
    /// Switch节点
    /// </summary>
    public SwitchBuilder<TGrandpa, TParent> Switch
        => _switch;
    /// <summary>
    /// 标签
    /// </summary>
    public SwitchLabelSyntax Label 
        => _label;
    #endregion
    /// <inheritdoc />
    protected internal override void AddCore(StatementSyntax statement)
    {
        if (_isReturn)
            return;
        base.AddCore(statement);
        _isReturn = statement.IsKind(SyntaxKind.ReturnStatement);
    }
    /// <summary>
    /// 构建分支
    /// </summary>
    /// <returns></returns>
    public SwitchSectionSyntax BuildSection()
        => _isReturn ? 
        SyntaxFactory.SwitchSection(SyntaxFactory.SingletonList(_label), SyntaxGenerator.List(_statements)) : 
        SyntaxFactory.SwitchSection(SyntaxFactory.SingletonList(_label), SyntaxGenerator.List([.. _statements, SyntaxFactory.BreakStatement()]));
    /// <inheritdoc />
    protected internal override TParent BuildCore()
        => _switch.BuildCore();
}
