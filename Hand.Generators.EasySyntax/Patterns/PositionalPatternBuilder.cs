using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Patterns;

/// <summary>
/// 位置模式构造器
/// </summary>
/// <param name="positions"></param>
/// <param name="type">类型</param>
/// <param name="name">命名</param>
public class PositionalPatternBuilder(List<SubpatternSyntax> positions, TypeSyntax? type, SyntaxToken? name = null)
    : RecursiveBaseBuilder(type, name), IPatternCollection
{
    /// <summary>
    /// 位置模式构造器
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">命名</param>
    public PositionalPatternBuilder(TypeSyntax? type, SyntaxToken? name = null)
        : this([], type, name)
    {
    }
    /// <summary>
    /// 位置模式构造器
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">命名</param>
    public PositionalPatternBuilder(TypeSyntax? type, string name)
        : this([], type, SyntaxFactory.Identifier(name))
    {
    }
    #region 配置
    private readonly PositionalClauseBuilder _clauseBuilder = new(positions);
    /// <summary>
    /// 位置模式列表
    /// </summary>
    private readonly List<SubpatternSyntax> _positions = positions;
    /// <summary>
    /// 位置模式列表
    /// </summary>
    public IEnumerable<SubpatternSyntax> Positions
        => _positions;
    #endregion
    #region RecursivePatternBuilder
    /// <inheritdoc />
    public override PositionalPatternClauseSyntax? GetPositional()
        => _clauseBuilder.Build();
    /// <inheritdoc />
    public override PropertyPatternClauseSyntax? GetProperty()
        => null;
    #endregion

    /// <inheritdoc />
    void IPatternCollection.AddPattern(PatternSyntax pattern)
        => _positions.Add(pattern.ToSub());
}
