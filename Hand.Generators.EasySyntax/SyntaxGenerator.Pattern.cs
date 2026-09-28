using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 模式匹配
/// </summary>
public partial class SyntaxGenerator
{
    /// <summary>
    /// Null模式
    /// </summary>
    public static ConstantPatternSyntax NullPattern
        => SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression));
    /// <summary>
    /// NotNull模式
    /// </summary>
    public static UnaryPatternSyntax NotNullPattern
        => SyntaxFactory.UnaryPattern(NullPattern);
    #region RelationalPatternSyntax
    #region GreaterThanPattern
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterThanPattern(int number)
        => GreaterThanPattern(Literal(number));
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterThanPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.GreaterThanToken), number);
    #endregion
    #region GreaterOrEqualPattern
    /// <summary>
    /// >=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterOrEqualPattern(int number)
        => GreaterOrEqualPattern(Literal(number));
    /// <summary>
    /// >=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax GreaterOrEqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.GreaterThanEqualsToken), number);
    #endregion
    #region LessThanPattern
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessThanPattern(int number)
        => LessThanPattern(Literal(number));
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessThanPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.LessThanToken), number);
    #endregion
    #region LessOrEqualPattern
    /// <summary>
    /// >
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessOrEqualPattern(int number)
        => LessOrEqualPattern(Literal(number));
    /// <summary>
    /// >=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax LessOrEqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.LessThanEqualsToken), number);
    #endregion
    #region EqualPattern
    /// <summary>
    /// !=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax EqualPattern(int number)
        => EqualPattern(Literal(number));
    /// <summary>
    /// ==
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax EqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken), number);
    #endregion
    #region NotEqualPattern
    /// <summary>
    /// ==
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax NotEqualPattern(int number)
        => NotEqualPattern(Literal(number));
    /// <summary>
    /// !=
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RelationalPatternSyntax NotEqualPattern(ExpressionSyntax number)
        => SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken), number);
    #endregion
    #endregion
    #region VarPattern
    /// <summary>
    /// var模式
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern()
        => SyntaxFactory.VarPattern(SyntaxFactory.DiscardDesignation());
    /// <summary>
    /// var模式
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern(SyntaxToken variableName)
        => SyntaxFactory.VarPattern(SyntaxFactory.SingleVariableDesignation(variableName));
    /// <summary>
    /// var模式
    /// </summary>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern(string variableName)
        => SyntaxFactory.VarPattern(SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier(variableName)));
    /// <summary>
    /// var括号模式
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern(params SeparatedSyntaxList<VariableDesignationSyntax> variables)
        => SyntaxFactory.VarPattern(SyntaxFactory.ParenthesizedVariableDesignation(variables));
    /// <summary>
    /// var括号模式
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern(params IEnumerable<SyntaxToken> variables)
        => SyntaxFactory.VarPattern(ParenthesizedVariable(variables));
    /// <summary>
    /// var括号模式
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VarPatternSyntax VarPattern(params IEnumerable<string> variables)
        => SyntaxFactory.VarPattern(ParenthesizedVariable(variables));
    #endregion
    /// <summary>
    /// or模式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static PatternSyntax OrPattern(params ExpressionSyntax[] items)
    {
        var count = items.Length;
        if (count == 0)
            throw new ArgumentOutOfRangeException(nameof(items));
        PatternSyntax pattern = SyntaxFactory.ConstantPattern(items[0]);
        for (var i = 1; i < count; i++)
            pattern = pattern.Or(SyntaxFactory.ConstantPattern(items[i]));
        return pattern;
    }
    /// <summary>
    /// and模式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static PatternSyntax AndPattern(params ExpressionSyntax[] items)
    {
        var count = items.Length;
        if (count == 0)
            throw new ArgumentOutOfRangeException(nameof(items));
        PatternSyntax pattern = SyntaxFactory.ConstantPattern(items[0]);
        for (var i = 1; i < count; i++)
            pattern = pattern.And(SyntaxFactory.ConstantPattern(items[i]));
        return pattern;
    }
}
