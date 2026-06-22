using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Patterns;

/// <summary>
/// 属性模式构造器
/// </summary>
/// <param name="properties"></param>
/// <param name="type">类型</param>
/// <param name="name">命名</param>
public class PropertyPatternBuilder(List<SubpatternSyntax> properties, TypeSyntax? type, SyntaxToken? name = null)
    : RecursiveBaseBuilder(type, name), INamedPatternCollection
{
    /// <summary>
    /// 属性模式构造器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public PropertyPatternBuilder(TypeSyntax? type, SyntaxToken? name = null)
        : this([], type, name)
    {
    }
    /// <summary>
    /// 位置模式构造器
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="name">命名</param>
    public PropertyPatternBuilder(TypeSyntax? type, string name)
        : this([], type, SyntaxFactory.Identifier(name))
    {
    }
    #region 配置
    private readonly PropertyClauseBuilder _clauseBuilder = new(properties);
    /// <summary>
    /// 属性模式列表
    /// </summary>
    private readonly List<SubpatternSyntax> _properties = properties;
    /// <summary>
    /// 属性模式列表
    /// </summary>
    public IEnumerable<SubpatternSyntax> Properties 
        => _properties;
    #endregion
    #region RecursivePatternBuilder
    /// <inheritdoc />
    public override PositionalPatternClauseSyntax? GetPositional()
        => null;
    /// <inheritdoc />
    public override PropertyPatternClauseSyntax? GetProperty()
        => _clauseBuilder.Build();
    #endregion
    /// <inheritdoc />
    void INamedPatternCollection.AddPattern(NameColonSyntax name, PatternSyntax pattern)
        => _properties.Add(pattern.ToSub(name));
}
