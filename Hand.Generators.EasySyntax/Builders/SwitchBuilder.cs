using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// switch
/// </summary>
/// <param name="governing"></param>
public class SwitchBuilder(ExpressionSyntax governing)
    : ScopeBuilder([])
{
    #region 配置
    private readonly ExpressionSyntax _governing = governing;
    /// <summary>
    /// 当前控制
    /// </summary>
    public ExpressionSyntax Governing
        => _governing;
    private readonly List<SwitchSection> _sections = [];
    #endregion
    /// <inheritdoc />
    protected internal override void AddCore(StatementSyntax statement)
        => throw new InvalidOperationException("无法直接向switch添加语句，请使用分支添加");
    /// <inheritdoc />
    public override StatementSyntax Build()
    {
        var count = _sections.Count;
        if (count == 0)
            return SyntaxFactory.EmptyStatement();
        var list = new List<SwitchSectionSyntax>();
        foreach (var item in _sections)
            list.Add(item.BuildSection());
        var statement = SyntaxFactory.SwitchStatement(_governing, SyntaxGenerator.List(list));
        return statement;
    }
    /// <summary>
    /// 分支
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public CaseBuilder Case(params ExpressionSyntax[] values)
        => Section(new CaseBuilder(this, values));
    /// <summary>
    /// 模式分支
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public PatternCaseBuilder Case(PatternSyntax pattern)
        => Section(new PatternCaseBuilder(this, pattern));
    /// <summary>
    /// 默认分支
    /// </summary>
    /// <returns></returns>
    public SwitchSection Default()
        => Section(new SwitchSection(this));
    /// <summary>
    /// 添加分支
    /// </summary>
    /// <param name="section"></param>
    internal TSection Section<TSection>(TSection section)
        where TSection : SwitchSection
    {
        _sections.Add(section);
        return section;
    }
}
