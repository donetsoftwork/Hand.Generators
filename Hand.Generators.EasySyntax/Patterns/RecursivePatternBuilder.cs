using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Patterns;

/// <summary>
/// 递归模式构造器
/// </summary>
/// <param name="positional"></param>
/// <param name="property"></param>
/// <param name="type"></param>
/// <param name="name"></param>
public class RecursivePatternBuilder(PositionalClauseBuilder positional, PropertyClauseBuilder property, TypeSyntax? type, SyntaxToken? name = null)
    : RecursivePatternBuilder<PositionalClauseBuilder>(positional, property, type, name), IPatternCollection, INamedPatternCollection
{
    /// <summary>
    /// 递归模式构造器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public RecursivePatternBuilder(TypeSyntax? type, SyntaxToken? name = null)
        : this(new([]), new([]), type, name)
    {
    }
    /// <summary>
    /// 递归模式构造器
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    public RecursivePatternBuilder(TypeSyntax? type, string name)
        : this(new([]), new([]), type, SyntaxFactory.Identifier(name))
    {
    }
    /// <inheritdoc />
    void IPatternCollection.AddPattern(PatternSyntax pattern)
        => _positional.Add(pattern);
    /// <inheritdoc />
    void INamedPatternCollection.AddPattern(NameColonSyntax name, PatternSyntax pattern)
        => _property.Add(name, pattern);
}
