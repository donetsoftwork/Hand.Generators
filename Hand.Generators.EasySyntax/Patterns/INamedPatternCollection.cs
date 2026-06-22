using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Patterns;

/// <summary>
/// 命名模式集合
/// </summary>
public interface INamedPatternCollection
{
    /// <summary>
    /// 添加命名模式
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    void AddPattern(NameColonSyntax name, PatternSyntax pattern);
}
