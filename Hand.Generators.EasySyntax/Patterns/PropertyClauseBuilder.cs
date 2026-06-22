using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Patterns;

/// <summary>
/// 属性模式构造器
/// </summary>
/// <param name="properties"></param>
public class PropertyClauseBuilder(List<SubpatternSyntax> properties)
    : INamedPatternCollection
{
    #region 配置
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
    /// <inheritdoc />
    void INamedPatternCollection.AddPattern(NameColonSyntax name, PatternSyntax pattern)
        => _properties.Add(pattern.ToSub(name));
    /// <summary>
    /// 构造位置模式
    /// </summary>
    /// <returns></returns>
    public PropertyPatternClauseSyntax? Build()
    {
        if (_properties.Count == 0)
            return null;
        return SyntaxFactory.PropertyPatternClause(SyntaxFactory.SeparatedList(_properties));
    }
}
