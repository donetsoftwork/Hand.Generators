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
public class NamedPositionalPatternBuilder(List<SubpatternSyntax> positions, TypeSyntax? type, SyntaxToken? name = null)
    : RecursiveBaseBuilder(type, name), INamedPatternCollection
{
    /// <summary>
    /// 位置模式构造器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public NamedPositionalPatternBuilder(TypeSyntax? type, SyntaxToken? name = null)
        : this([], type, name)
    {
    }
    /// <summary>
    /// 位置模式构造器
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">命名</param>
    public NamedPositionalPatternBuilder(TypeSyntax? type, string name)
        : this([], type, SyntaxFactory.Identifier(name))
    {
    }
    #region 配置
    private readonly NamedPositionalClauseBuilder _clauseBuilder = new(positions);
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
    void INamedPatternCollection.AddPattern(NameColonSyntax name, PatternSyntax pattern)
        => _positions.Add(pattern.ToSub(name));
}
