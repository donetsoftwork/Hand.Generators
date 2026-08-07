using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Patterns;

/// <summary>
/// 模式集合
/// </summary>
public interface IPatternCollection
{
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    void AddPattern(PatternSyntax pattern); 
}
