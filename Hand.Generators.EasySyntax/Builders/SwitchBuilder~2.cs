using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// Switch
/// </summary>
/// <typeparam name="TParent"></typeparam>
/// <param name="parent"></param>
/// <param name="governing"></param>
public class SwitchBuilder<TGrandpa, TParent>(TParent parent, ExpressionSyntax governing)
    : ScopeBuilder<TGrandpa, TParent>(parent)
    where TParent : StatementBuilder<TGrandpa>
{
    #region 配置
    private readonly ExpressionSyntax _governing = governing;
    /// <summary>
    /// 当前控制
    /// </summary>
    public ExpressionSyntax Governing
        => _governing;
    private readonly List<SwitchSection<TGrandpa, TParent>> _sections = [];
    #endregion
    /// <summary>
    /// 分支
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public CaseBuilder<TGrandpa, TParent> Case(ExpressionSyntax value)
        => Section(new CaseBuilder<TGrandpa, TParent>(this, value));
    /// <inheritdoc />
    protected internal override TParent BuildCore()
    {
        var count = _sections.Count;
        if (count == 0)
            return _parent;
        var list = new List<SwitchSectionSyntax>();
        foreach (var item in _sections)
            list.Add(item.BuildSection());
        var statement = SyntaxFactory.SwitchStatement(_governing, SyntaxGenerator.List(list));
        _parent.AddCore(statement);        
        return _parent;
    }
    /// <summary>
    /// 添加分支
    /// </summary>
    /// <param name="section"></param>
    internal TSection Section<TSection>(TSection section)
        where TSection: SwitchSection<TGrandpa, TParent>
    {
        _sections.Add(section);
        return section;
    }
}
