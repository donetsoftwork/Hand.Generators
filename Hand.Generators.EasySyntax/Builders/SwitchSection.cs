using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// switch分支
/// </summary>
/// <param name="switch"></param>
public class SwitchSection(SwitchBuilder @switch)
    : ScopeBuilder([])
{
    #region 配置
    /// <summary>
    /// Switch节点
    /// </summary>
    protected readonly SwitchBuilder _switch = @switch;
    private bool _isReturn = false;
    /// <summary>
    /// Switch节点
    /// </summary>
    public SwitchBuilder Switch
        => _switch;
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
    /// 获取标签
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<SwitchLabelSyntax> GetLabels()
        => [SyntaxFactory.DefaultSwitchLabel()];
    /// <summary>
    /// 构建分支
    /// </summary>
    /// <returns></returns>
    public SwitchSectionSyntax BuildSection()
        => _isReturn ?
        SyntaxFactory.SwitchSection(SyntaxFactory.List(GetLabels()), SyntaxGenerator.List(_statements)) :
        SyntaxFactory.SwitchSection(SyntaxFactory.List(GetLabels()), SyntaxGenerator.List([.. _statements, SyntaxFactory.BreakStatement()]));
    /// <inheritdoc />
    public override StatementSyntax Build()
        => _switch.Build();
}
