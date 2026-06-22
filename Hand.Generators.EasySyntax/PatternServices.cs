using Hand.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 模式扩展方法
/// </summary>
public static partial class GenerateServices
{
    /// <summary>
    /// 常量模式
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstantPatternSyntax ToPattern(this ExpressionSyntax expression)
        => SyntaxFactory.ConstantPattern(expression);
    #region VariablePattern
    /// <summary>
    /// 变量模式
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeclarationPatternSyntax VariablePattern(this TypeSyntax type, VariableDesignationSyntax variable)
        => SyntaxFactory.DeclarationPattern(type, variable);
    /// <summary>
    /// 变量模式
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeclarationPatternSyntax VariablePattern(this TypeSyntax type, SyntaxToken variableName)
        => SyntaxFactory.DeclarationPattern(type, SyntaxFactory.SingleVariableDesignation(variableName));
    /// <summary>
    /// 变量模式
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeclarationPatternSyntax VariablePattern(this TypeSyntax type, string variableName)
        => SyntaxFactory.DeclarationPattern(type, SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(variableName)));
    #endregion
    /// <summary>
    /// 类型弃元模式
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DeclarationPatternSyntax DiscardPattern(this TypeSyntax type)
        => SyntaxFactory.DeclarationPattern(type, SyntaxFactory.DiscardDesignation());
    /// <summary>
    /// 否定模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UnaryPatternSyntax Not(this PatternSyntax pattern)
        => SyntaxFactory.UnaryPattern(pattern);
    /// <summary>
    /// 括号模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParenthesizedPatternSyntax Parenthesized(this PatternSyntax pattern)
        => SyntaxFactory.ParenthesizedPattern(pattern);
    #region Slice
    /// <summary>
    /// 切片(展开)
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SlicePatternSyntax Slice(this PatternSyntax pattern)
        => SyntaxFactory.SlicePattern(pattern);
    /// <summary>
    /// 切片(展开)
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SlicePatternSyntax Slice(this ExpressionSyntax pattern)
        => SyntaxFactory.SlicePattern(SyntaxFactory.ConstantPattern(pattern));
    #endregion
    #region ToSub
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSub(this PatternSyntax pattern, NameColonSyntax? name = null)
        => SyntaxFactory.Subpattern(name, pattern);
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSub(this PatternSyntax pattern, IdentifierNameSyntax name)
        => SyntaxFactory.Subpattern(SyntaxFactory.NameColon(name), pattern);
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSub(this PatternSyntax pattern, SyntaxToken name)
        => SyntaxFactory.Subpattern(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(name)), pattern);
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSub(this PatternSyntax pattern, string name)
        => SyntaxFactory.Subpattern(SyntaxFactory.NameColon(name), pattern);
    #endregion
    #region ToSubpattern
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSubpattern(this ExpressionSyntax pattern, NameColonSyntax? name = null)
        => SyntaxFactory.Subpattern(name, SyntaxFactory.ConstantPattern(pattern));
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSubpattern(this ExpressionSyntax pattern, IdentifierNameSyntax name)
        => SyntaxFactory.Subpattern(SyntaxFactory.NameColon(name), SyntaxFactory.ConstantPattern(pattern));
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSubpattern(this ExpressionSyntax pattern, SyntaxToken name)
        => SyntaxFactory.Subpattern(SyntaxFactory.NameColon(SyntaxFactory.IdentifierName(name)), SyntaxFactory.ConstantPattern(pattern));
    /// <summary>
    /// 作为子模式
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SubpatternSyntax ToSubpattern(this ExpressionSyntax pattern, string name)
        => SyntaxFactory.Subpattern(SyntaxFactory.NameColon(name), SyntaxFactory.ConstantPattern(pattern));
    #endregion
    /// <summary>
    /// or模式
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BinaryPatternSyntax Or(this PatternSyntax left, PatternSyntax right)
        => SyntaxFactory.BinaryPattern(SyntaxKind.OrPattern, left, SyntaxFactory.Token(SyntaxKind.OrKeyword), right);
    /// <summary>
    /// or模式
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BinaryPatternSyntax And(this PatternSyntax left, PatternSyntax right)
        => SyntaxFactory.BinaryPattern(SyntaxKind.AndPattern, left, SyntaxFactory.Token(SyntaxKind.AndKeyword), right);
    /// <summary>
    /// or模式
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BinaryPatternSyntax OrPattern(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryPattern(SyntaxKind.OrPattern, SyntaxFactory.ConstantPattern(left), SyntaxFactory.Token(SyntaxKind.OrKeyword), SyntaxFactory.ConstantPattern(right));
    /// <summary>
    /// and模式
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BinaryPatternSyntax AndPattern(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryPattern(SyntaxKind.AndPattern, SyntaxFactory.ConstantPattern(left), SyntaxFactory.Token(SyntaxKind.AndKeyword), SyntaxFactory.ConstantPattern(right));
    #region Add
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static TBuilder Add<TBuilder>(this TBuilder builder, PatternSyntax pattern)
        where TBuilder : IPatternCollection
    {
        builder.AddPattern(pattern);
        return builder;
    }
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TBuilder Add<TBuilder>(this TBuilder builder, ExpressionSyntax pattern)
        where TBuilder : IPatternCollection
        => Add(builder, pattern.ToPattern());
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static TBuilder Add<TBuilder>(this TBuilder builder, NameColonSyntax name, PatternSyntax pattern)
        where TBuilder : INamedPatternCollection
    {
        builder.AddPattern(name, pattern);
        return builder;
    }
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TBuilder Add<TBuilder>(this TBuilder builder, IdentifierNameSyntax name, PatternSyntax pattern)
        where TBuilder : INamedPatternCollection
        => Add(builder, SyntaxFactory.NameColon(name), pattern);
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TBuilder Add<TBuilder>(this TBuilder builder, string name, PatternSyntax pattern)
        where TBuilder : INamedPatternCollection
        => Add(builder, SyntaxFactory.NameColon(name), pattern);
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TBuilder Add<TBuilder>(this TBuilder builder, NameColonSyntax name, ExpressionSyntax pattern)
        where TBuilder : INamedPatternCollection
        => Add(builder, name, pattern.ToPattern());
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TBuilder Add<TBuilder>(this TBuilder builder, IdentifierNameSyntax name, ExpressionSyntax pattern)
        where TBuilder : INamedPatternCollection
        => Add(builder, SyntaxFactory.NameColon(name), pattern.ToPattern());
    /// <summary>
    /// 添加模式
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TBuilder Add<TBuilder>(this TBuilder builder, string name, ExpressionSyntax pattern)
        where TBuilder : INamedPatternCollection
        => Add(builder, SyntaxFactory.NameColon(name), pattern.ToPattern());
    #endregion
}
