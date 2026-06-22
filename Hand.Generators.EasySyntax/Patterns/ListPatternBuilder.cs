using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Patterns;

/// <summary>
/// 构造列表模式
/// </summary>
/// <param name="name"></param>
public class ListPatternBuilder(SyntaxToken? name = null)
    : IPatternCollection
{
    #region 配置
    private readonly VariableDesignationSyntax? _designation = name is null ? null : SyntaxFactory.SingleVariableDesignation(name.Value);
    /// <summary>
    /// 位置模式列表
    /// </summary>
    private readonly List<PatternSyntax> _patterns = [];

    /// <summary>
    /// 模式变量
    /// </summary>
    public VariableDesignationSyntax? Designation
        => _designation;
    /// <summary>
    /// 模式列表
    /// </summary>
    public IEnumerable<PatternSyntax> Patterns
        => _patterns;
    #endregion
    ///// <summary>
    ///// 添加模式
    ///// </summary>
    ///// <param name="pattern"></param>
    ///// <returns></returns>
    //public ListPatternBuilder Add(PatternSyntax pattern)
    //{
    //    _patterns.Add(pattern);
    //    return this;
    //}
    ///// <summary>
    ///// 添加模式
    ///// </summary>
    ///// <param name="pattern"></param>
    ///// <returns></returns>
    //public ListPatternBuilder Add(ExpressionSyntax pattern)
    //    => Add(pattern.ToPattern());
    /// <inheritdoc />
    void IPatternCollection.AddPattern(PatternSyntax pattern)
        => _patterns.Add(pattern);
    /// <summary>
    /// 构造列表模式
    /// </summary>
    /// <returns></returns>
    public ListPatternSyntax Build()
        => SyntaxFactory.ListPattern(SyntaxFactory.SeparatedList(_patterns), _designation);
}
