using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Patterns;

/// <summary>
/// 位置模式构造器
/// </summary>
/// <param name="positions"></param>
public class PositionalClauseBuilder(List<SubpatternSyntax> positions)
    : PositionalBaseBuilder(positions), IPatternCollection
{
    /// <inheritdoc />
    void IPatternCollection.AddPattern(PatternSyntax pattern)
        => _positions.Add(pattern.ToSub());
}
