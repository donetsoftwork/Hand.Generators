using Hand.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 表达式
/// </summary>
public partial class SyntaxGenerator
{
    #region Literal
    /// <summary>
    /// null
    /// </summary>
    public static LiteralExpressionSyntax NullLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
    /// <summary>
    /// default
    /// </summary>
    public static LiteralExpressionSyntax DefaultLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
    /// <summary>
    /// true
    /// </summary>
    public static LiteralExpressionSyntax TrueLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression);
    /// <summary>
    /// false
    /// </summary>
    public static LiteralExpressionSyntax FalseLiteral => SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
    /// <summary>
    /// this
    /// </summary>
    public static ThisExpressionSyntax ThisExpression => SyntaxFactory.ThisExpression();
    /// <summary>
    /// this
    /// </summary>
    public static BaseExpressionSyntax BaseExpression => SyntaxFactory.BaseExpression();
    /// <summary>
    /// bool字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(bool value)
        => value ? TrueLiteral : FalseLiteral;
    /// <summary>
    /// int字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(int value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// uint字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(uint value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// long字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(long value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// ulong字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(ulong value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// float字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(float value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// double字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(double value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// decimal字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(decimal value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// string字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(string value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// char字面量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LiteralExpressionSyntax Literal(char value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal(value));
    /// <summary>
    /// 基础类型转化为字面量表达式
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static LiteralExpressionSyntax Literal(SpecialType type, object value)
    {
        return type switch
        {
            SpecialType.System_Boolean => Literal((bool)value),
            SpecialType.System_Int16 => Literal((short)value),
            SpecialType.System_UInt16 => Literal((ushort)value),
            SpecialType.System_Int32 => Literal((int)value),
            SpecialType.System_UInt32 => Literal((uint)value),
            SpecialType.System_Int64 => Literal((long)value),
            SpecialType.System_UInt64 => Literal((ulong)value),
            SpecialType.System_String => Literal((string)value),
            SpecialType.System_Char => Literal((char)value),
            SpecialType.System_Decimal => Literal((decimal)value),
            SpecialType.System_Double => Literal((double)value),
            SpecialType.System_Single => Literal((float)value),
            _ => throw new ArgumentException("字面量类型不支持"),
        };
    }
    #endregion
    #region Collection
    /// <summary>
    /// 集合表达式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CollectionExpressionSyntax Collection(params ExpressionSyntax[] items)
        => SyntaxFactory.CollectionExpression(SyntaxFactory.SeparatedList(Array.ConvertAll<ExpressionSyntax, CollectionElementSyntax>(items, static item => SyntaxFactory.ExpressionElement(item))));
    #endregion
    /// <summary>
    /// 元组表达式
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TupleExpressionSyntax Tuple(params ExpressionSyntax[] items)
        => SyntaxFactory.TupleExpression(SyntaxFactory.SeparatedList(Array.ConvertAll(items, static item => SyntaxFactory.Argument(item))));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InitializerExpressionSyntax Initializer(params IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>(items));
    /// <summary>
    /// 表达式方法体
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrowExpressionClauseSyntax ExpressionBody(ExpressionSyntax expression)
        => SyntaxFactory.ArrowExpressionClause(SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken), expression);
    #region New
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(SeparatedSyntaxList<ArgumentSyntax> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<ExpressionSyntax> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<SyntaxToken> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<SyntaxToken> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<string> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<string> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(), Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImplicitObjectCreationExpressionSyntax New(IEnumerable<ExpressionSyntax> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ImplicitObjectCreationExpression(ArgumentList(arguments), Initializer(items));
    #endregion
    #region Interpolation
    /// <summary>
    /// 开始构造插值表达式
    /// </summary>
    /// <param name="start">String/VerbatimString/SingleLineRawString/MultiLineRawString</param>
    /// <param name="end">String/MultiLineRawString</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InterpolationBuilder Interpolation(SyntaxKind start, SyntaxKind end)
        => new(start, end);
    /// <summary>
    /// 开始构造插值表达式
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InterpolationBuilder Interpolation()
        => new(SyntaxKind.InterpolatedStringStartToken, SyntaxKind.InterpolatedStringEndToken);
    #endregion
}
