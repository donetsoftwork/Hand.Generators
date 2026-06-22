using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Patterns;

/// <summary>
/// 位置模式构造器
/// </summary>
/// <param name="positions"></param>
public abstract class PositionalBaseBuilder(List<SubpatternSyntax> positions)
{
    #region 配置
    /// <summary>
    /// 位置模式列表
    /// </summary>
    protected readonly List<SubpatternSyntax> _positions = positions;
    /// <summary>
    /// 位置模式列表
    /// </summary>
    public IEnumerable<SubpatternSyntax> Positions
        => _positions;
    #endregion
    /// <summary>
    /// 构造位置模式
    /// </summary>
    /// <returns></returns>
    public PositionalPatternClauseSyntax? Build()
    {
        if (_positions.Count == 0)
            return null;
        return SyntaxFactory.PositionalPatternClause(SyntaxFactory.SeparatedList(_positions));
    }
}
