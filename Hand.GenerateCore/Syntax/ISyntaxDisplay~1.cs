using Microsoft.CodeAnalysis.CSharp;

namespace Hand.Syntax;

/// <summary>
/// 语法展示
/// </summary>
public interface ISyntaxDisplay<out TSyntax>
    where TSyntax : CSharpSyntaxNode
{
    /// <summary>
    /// 展示
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    TSyntax Display(SyntaxGenerator generator);
}
