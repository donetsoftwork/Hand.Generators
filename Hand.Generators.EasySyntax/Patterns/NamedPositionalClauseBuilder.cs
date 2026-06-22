using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Patterns;

/// <summary>
/// 命名位置模式构造器
/// </summary>
/// <param name="positions"></param>
public class NamedPositionalClauseBuilder(List<SubpatternSyntax> positions)
    : PositionalBaseBuilder(positions), INamedPatternCollection
{
    /// <inheritdoc />
    void INamedPatternCollection.AddPattern(NameColonSyntax name, PatternSyntax pattern)
        => _positions.Add(pattern.ToSub(name));
}
