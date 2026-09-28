using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 预定义类型扩展方法
/// </summary>
public static partial class GenerateServices
{
    /// <summary>
    /// 是bool类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBool(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsBool(predefined);
    /// <summary>
    /// 是bool类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBool(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.BoolKeyword);
    /// <summary>
    /// 是byte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsByte(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsByte(predefined);
    /// <summary>
    /// 是byte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsByte(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ByteKeyword);
    /// <summary>
    /// 是sbyte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSByte(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsSByte(predefined);
    /// <summary>
    /// 是sbyte类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSByte(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.SByteKeyword);
    /// <summary>
    /// 是Int类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsInt(predefined);
    /// <summary>
    /// 是Int类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.IntKeyword);
    /// <summary>
    /// 是uint类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUInt(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsUInt(predefined);
    /// <summary>
    /// 是uint类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUInt(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.UIntKeyword);
    /// <summary>
    /// 是short类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsShort(predefined);
    /// <summary>
    /// 是short类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ShortKeyword);
    /// <summary>
    /// 是UShort类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUShort(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsUShort(predefined);
    /// <summary>
    /// 是UShort类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUShort(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.UShortKeyword);
    /// <summary>
    /// 是long类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLong(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsLong(predefined);
    /// <summary>
    /// 是long类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLong(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.LongKeyword);
    /// <summary>
    /// 是ulong类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsULong(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsULong(predefined);
    /// <summary>
    /// 是ulong类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsULong(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ULongKeyword);
    /// <summary>
    /// 是float类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFloat(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsFloat(predefined);
    /// <summary>
    /// 是float类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFloat(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.FloatKeyword);
    /// <summary>
    /// 是double类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDouble(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsDouble(predefined);
    /// <summary>
    /// 是double类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDouble(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.DoubleKeyword);
    /// <summary>
    /// 是decimal类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDecimal(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsDecimal(predefined);
    /// <summary>
    /// 是decimal类型 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDecimal(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.DecimalKeyword);
    /// <summary>
    /// 是string类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsString(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsString(predefined);
    /// <summary>
    /// 是string类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsString(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.StringKeyword);
    /// <summary>
    /// 是char类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsChar(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsChar(predefined);
    /// <summary>
    /// 是char类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsChar(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.CharKeyword);
    /// <summary>
    /// 是Object类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsObject(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsObject(predefined);
    /// <summary>
    /// 是Object类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsObject(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.ObjectKeyword);
    /// <summary>
    /// 是Void类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this TypeSyntax type)
        => type is PredefinedTypeSyntax predefined && IsVoid(predefined);
    /// <summary>
    /// 是Void类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this PredefinedTypeSyntax type)
        => type.Keyword.IsKind(SyntaxKind.VoidKeyword);
    /// <summary>
    /// 空类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NullableTypeSyntax Nullable(this TypeSyntax type)
        => SyntaxFactory.NullableType(type);
    /// <summary>
    /// 空类型
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax Nullable(this TypeSyntax type, bool isNullable)
        => isNullable ? SyntaxFactory.NullableType(type) : type;
    /// <summary>
    /// typeof
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeOfExpressionSyntax TypeOf(this TypeSyntax type)
        => SyntaxFactory.TypeOfExpression(type);
    /// <summary>
    /// 类型转换
    /// </summary>
    /// <param name="type"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CastExpressionSyntax Cast(this TypeSyntax type, ExpressionSyntax expression)
        => SyntaxFactory.CastExpression(type, expression);
    /// <summary>
    /// 数组
    /// </summary>
    /// <param name="type"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayTypeSyntax Array(this TypeSyntax type, int rank = 1)
        => Array(type, CheckOmittedArraySize(rank));
    /// <summary>
    /// 数组
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sizes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayTypeSyntax Array(this TypeSyntax type, params SeparatedSyntaxList<ExpressionSyntax> sizes)
        => SyntaxFactory.ArrayType(type, SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(sizes)));
    #region New
    /// <summary>
    /// 初始化数组
    /// </summary>
    /// <param name="type"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCreationExpressionSyntax New(this ArrayTypeSyntax type, params SeparatedSyntaxList<ExpressionSyntax> elements)
        => SyntaxFactory.ArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, elements));
    /// <summary>
    /// 初始化空数组
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCreationExpressionSyntax Empty(this ArrayTypeSyntax type)
        => SyntaxFactory.ArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type, default);
    /// <summary>
    /// 初始化空数组
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCreationExpressionSyntax EmptyArray(this TypeSyntax type)
        => SyntaxFactory.ArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), type.Array(SyntaxGenerator.Literal(0)), default);
    /// <summary>
    /// 初始化数组
    /// </summary>
    /// <param name="type"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayCreationExpressionSyntax NewArray(this TypeSyntax type, params SeparatedSyntaxList<ExpressionSyntax> elements)
        => New(Array(type, 1), elements);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxFactory.ArgumentList(), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, SeparatedSyntaxList<ArgumentSyntax> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxFactory.ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<ExpressionSyntax> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<SyntaxToken> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxFactory.ArgumentList(), SyntaxGenerator.Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="initializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<string> arguments, InitializerExpressionSyntax? initializer = null)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), initializer);
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, SeparatedSyntaxList<ArgumentSyntax> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxFactory.ArgumentList(arguments), SyntaxGenerator.Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<ExpressionSyntax> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), SyntaxGenerator.Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<SyntaxToken> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), SyntaxGenerator.Initializer(items));
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arguments"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObjectCreationExpressionSyntax New(this TypeSyntax type, IEnumerable<string> arguments, IEnumerable<AssignmentExpressionSyntax> items)
        => SyntaxFactory.ObjectCreationExpression(type, SyntaxGenerator.ArgumentList(arguments), SyntaxGenerator.Initializer(items));
    #endregion
    /// <summary>
    /// 默认值
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DefaultExpressionSyntax Default(this TypeSyntax type)
        => SyntaxFactory.DefaultExpression(type);
    #region Attribute
    /// <summary>
    /// 构造Attribute标记
    /// </summary>
    /// <param name="attributeName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeSyntax Attribute(this NameSyntax attributeName)
        => SyntaxFactory.Attribute(attributeName);
    /// <summary>
    /// 构造Attribute标记
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeSyntax Attribute(this NameSyntax attributeName, SeparatedSyntaxList<AttributeArgumentSyntax> arguments)
        => SyntaxFactory.Attribute(attributeName, SyntaxFactory.AttributeArgumentList(arguments));
    /// <summary>
    /// 构造Attribute标记
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AttributeSyntax Attribute(this NameSyntax attributeName, IEnumerable<ExpressionSyntax> arguments)
        => Attribute(attributeName, [..arguments.Select(static item => item.ToAttributeArgument())]);
    #endregion
    #region Throw
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, params SeparatedSyntaxList<ExpressionSyntax> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, IEnumerable<ExpressionSyntax> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, IEnumerable<SyntaxToken> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="exceptionType"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThrowExpressionSyntax Throw(this TypeSyntax exceptionType, IEnumerable<string> arguments)
        => SyntaxFactory.ThrowExpression(New(exceptionType, arguments));
    #endregion
    /// <summary>
    /// 数组长度参数处理
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public static SeparatedSyntaxList<ExpressionSyntax> CheckOmittedArraySize(int rank = 1)
    {
        if (rank > 1)
            return SyntaxFactory.SeparatedList<ExpressionSyntax>(Enumerable.Repeat(SyntaxFactory.OmittedArraySizeExpression(), rank));
        return SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression());
    }
    /// <summary>
    /// 指针
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax Pointer(this TypeSyntax type)
        => SyntaxFactory.PointerType(type);
    /// <summary>
    /// 全局命名样式
    /// </summary>
    private static readonly SymbolDisplayFormat _globalStyle = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included‌);
    /// <summary>
    /// 最简模式(不包含命名空间)
    /// </summary>
    private static readonly SymbolDisplayFormat _miniStyle = SymbolDisplayFormat.MinimallyQualifiedFormat;
    /// <summary>
    /// 自动包含命名
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax ToDisplayName(this ITypeSymbol symbol)
        => SyntaxFactory.ParseTypeName(symbol.ToDisplayString());
    /// <summary>
    /// 全局命名
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax ToGlobalName(this ITypeSymbol symbol)
        => SyntaxFactory.ParseTypeName(symbol.ToDisplayString(_globalStyle));
    /// <summary>
    /// 最简命名
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToMiniName(this ITypeSymbol symbol)
        => SyntaxFactory.IdentifierName(symbol.ToDisplayString(_miniStyle));
    #region IsNullable
    /// <summary>
    /// 是否可空类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullable(this INamedTypeSymbol type)
        => type.NullableAnnotation == NullableAnnotation.Annotated || IsGenericType(type, SpecialType.System_Nullable_T);
    #endregion
    /// <summary>
    /// 是否泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGenericType(this INamedTypeSymbol type, SpecialType genericType)
        => type.IsGenericType && type.ConstructedFrom.SpecialType == genericType;
    /// <summary>
    /// 判断是否包含泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static bool HasGenericType(this INamedTypeSymbol type, SpecialType genericType)
    {
        if (IsGenericType(type, genericType))
            return true;
        foreach (var subType in type.Interfaces)
        {
            if (IsGenericType(subType, genericType))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 获取泛型闭合接口
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetGenericCloseInterfaces(this INamedTypeSymbol type, SpecialType genericType)
    {
        if (IsGenericType(type, genericType))
        {
            yield return type;
            yield break;
        }
        var interfaces = type.Interfaces;
        foreach (var item in interfaces)
        {
            if (IsGenericType(item, genericType))
                yield return item;
        }
    }
    /// <summary>
    /// 确认Nullable类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static NullableTypeSyntax CheckNullable(this TypeSyntax type)
    {
        if (type is NullableTypeSyntax nullable)
            return nullable;
        return SyntaxFactory.NullableType(type);
    }
    /// <summary>
    /// 类型符号转化为类型语法
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax ToSyntax(this INamedTypeSymbol symbol, bool nullable)
        => nullable ? CheckNullable(ToSyntax(symbol)) : ToSyntax(symbol);
    /// <summary>
    /// 类型符号转化为类型语法
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeSyntax ToSyntax(this INamedTypeSymbol symbol)
    {
        if (symbol.IsGenericType && symbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
            return SyntaxFactory.NullableType(ToSyntax(symbol.TypeArguments[0]));
        if (symbol.NullableAnnotation == NullableAnnotation.Annotated)
            return SyntaxFactory.NullableType(ToSyntax(symbol.ConstructedFrom));
        return ToSyntaxCore(symbol);
    }
    /// <summary>
    /// IArrayTypeSymbol转TypeSyntax
    /// </summary>
    /// <param name="arraySymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax ToSyntax(this IArrayTypeSymbol arraySymbol)
        => ToSyntax(arraySymbol.ElementType).Array();
    /// <summary>
    /// 类型符号转化为类型语法(不处理Nullable)
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeSyntax ToSyntax(this ITypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol namedType)
            return ToSyntax(namedType);
        if (symbol is IArrayTypeSymbol arraySymbol)
            return ToSyntax(arraySymbol);
        return ToDisplayName(symbol);
    }
    /// <summary>
    /// 类型符号转化为类型语法(不处理Nullable)
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TypeSyntax ToSyntaxCore(this INamedTypeSymbol symbol)
    {
        return symbol.SpecialType switch
        {
            SpecialType.System_Boolean => SyntaxGenerator.BoolType,
            SpecialType.System_Byte => SyntaxGenerator.ByteType,
            SpecialType.System_SByte => SyntaxGenerator.SByteType,
            SpecialType.System_Int16 => SyntaxGenerator.ShortType,
            SpecialType.System_UInt16 => SyntaxGenerator.UShortType,
            SpecialType.System_Int32 => SyntaxGenerator.IntType,
            SpecialType.System_UInt32 => SyntaxGenerator.UIntType,
            SpecialType.System_Int64 => SyntaxGenerator.LongType,
            SpecialType.System_UInt64 => SyntaxGenerator.ULongType,
            SpecialType.System_Single => SyntaxGenerator.FloatType,
            SpecialType.System_Double => SyntaxGenerator.DoubleType,
            SpecialType.System_Decimal => SyntaxGenerator.DecimalType,
            SpecialType.System_String => SyntaxGenerator.StringType,
            SpecialType.System_Char => SyntaxGenerator.CharType,
            SpecialType.System_DateTime => SyntaxGenerator.DateTimeType,
            SpecialType.System_Object => SyntaxGenerator.ObjectType,
            SpecialType.System_ValueType => SyntaxFactory.IdentifierName("ValueType").Qualify("System"),
            SpecialType.System_Void => SyntaxGenerator.VoidType,  
            SpecialType.System_IntPtr => SyntaxFactory.IdentifierName("IntPtr").Qualify("System"),
            SpecialType.System_UIntPtr => SyntaxFactory.IdentifierName("UIntPtr").Qualify("System"),
            SpecialType.System_Array => SyntaxFactory.IdentifierName("Array").Qualify("System"),
            SpecialType.System_Delegate => SyntaxFactory.IdentifierName("Delegate").Qualify("System"),
            SpecialType.System_MulticastDelegate => SyntaxFactory.IdentifierName("MulticastDelegate").Qualify("System"),
            SpecialType.System_Enum => SyntaxFactory.IdentifierName("Enum").Qualify("System"),
            SpecialType.System_IDisposable => SyntaxFactory.IdentifierName("IDisposable").Qualify("System"),
            SpecialType.System_IAsyncResult => SyntaxFactory.IdentifierName("IAsyncResult").Qualify("System"),
            SpecialType.System_Collections_IEnumerable => SyntaxFactory.IdentifierName("IEnumerable").Qualifies("System", "Collections"),
            SpecialType.System_Collections_IEnumerator => SyntaxFactory.IdentifierName("IEnumerator").Qualifies("System", "Collections"),
            SpecialType.System_AsyncCallback => SyntaxFactory.IdentifierName("AsyncCallback").Qualify("System"),
            SpecialType.System_ArgIterator => SyntaxFactory.IdentifierName("ArgIterator").Qualify("System"),
            SpecialType.System_TypedReference => SyntaxFactory.IdentifierName("TypedReference").Qualify("System"),
            SpecialType.System_RuntimeArgumentHandle => SyntaxFactory.IdentifierName("RuntimeArgumentHandle").Qualify("System"),
            SpecialType.System_RuntimeFieldHandle => SyntaxFactory.IdentifierName("RuntimeFieldHandle").Qualify("System"),
            SpecialType.System_RuntimeMethodHandle => SyntaxFactory.IdentifierName("RuntimeMethodHandle").Qualify("System"),
            SpecialType.System_RuntimeTypeHandle => SyntaxFactory.IdentifierName("RuntimeTypeHandle").Qualify("System"),
            SpecialType.System_Runtime_CompilerServices_IsVolatile => SyntaxFactory.IdentifierName("IsVolatile").Qualifies("System", "Runtime", "CompilerServices"),
            SpecialType.System_Runtime_CompilerServices_RuntimeFeature => SyntaxFactory.IdentifierName(".RuntimeFeature").Qualifies("System", "Runtime", "CompilerServices"),
            SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute => SyntaxFactory.IdentifierName("PreserveBaseOverridesAttribute").Qualifies("System", "Runtime", "CompilerServices"),
            SpecialType.System_Runtime_CompilerServices_InlineArrayAttribute => SyntaxFactory.IdentifierName("InlineArrayAttribute").Qualifies("System", "Runtime", "CompilerServices"),
            SpecialType.None => ToDisplayName(symbol),
            SpecialType.System_Nullable_T => SyntaxGenerator.OmitGeneric("Nullable").Qualify("System"),
            SpecialType.System_Collections_Generic_IEnumerable_T => SyntaxGenerator.OmitGeneric("IEnumerable").Qualifies("System", "Collections", "Generic"),
            SpecialType.System_Collections_Generic_IEnumerator_T => SyntaxGenerator.OmitGeneric("IEnumerator").Qualifies("System", "Collections", "Generic"),
            SpecialType.System_Collections_Generic_IList_T => SyntaxGenerator.OmitGeneric("IList").Qualifies("System", "Collections", "Generic"),
            SpecialType.System_Collections_Generic_ICollection_T => SyntaxGenerator.OmitGeneric("ICollection").Qualifies("System", "Collections", "Generic"),
            SpecialType.System_Collections_Generic_IReadOnlyList_T => SyntaxGenerator.OmitGeneric("IReadOnlyList").Qualifies("System", "Collections", "Generic"),
            SpecialType.System_Collections_Generic_IReadOnlyCollection_T => SyntaxGenerator.OmitGeneric("IReadOnlyCollection").Qualifies("System", "Collections", "Generic"),
            _ => throw new NotSupportedException(),
        };
    }
    /// <summary>
    /// 生成器特性
    /// </summary>
    /// <param name="generatorType"></param>
    /// <param name="namespaces"></param>
    /// <returns></returns>
    public static AttributeSyntax ToGeneratedCodeAttribute(this Type generatorType, List<string> namespaces)
    {
        namespaces.Add("System.CodeDom.Compiler");
        return SyntaxFactory.IdentifierName("GeneratedCode")
            .Attribute([
                SyntaxGenerator.Literal(generatorType.FullName!),
                SyntaxGenerator.Literal(generatorType.Assembly.GetName().Version!.ToString())]
        );
    }
}
