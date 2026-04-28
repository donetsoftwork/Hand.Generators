using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Linq;
using Hand.Symbols;

namespace Hand;

/// <summary>
/// 常量扩展方法
/// </summary>
public static partial class GenerateCoreServices
{
    /// <summary>
    /// 获取常量值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="constant"></param>
    /// <returns></returns>
    public static TValue GetValue<TValue>(this TypedConstant constant)
    {
        if (constant.Value is TValue value)
            return value;
        return default!;
    }
    /// <summary>
    /// 获取集合常量值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="constant"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TValue> GetValues<TValue>(this TypedConstant constant)
    {
        return constant.Values
            .Select(item => item.Value)
            .OfType<TValue>();
    }
    /// <summary>
    /// 转化常量为表达式
    /// </summary>
    /// <param name="constant"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ExpressionSyntax ToExpression(this TypedConstant constant)
    {
        if (constant.IsNull)
            return SyntaxGenerator.NullLiteral;
        return constant.Kind switch
        {
            TypedConstantKind.Primitive => PrimitiveToLiteral(constant),
            TypedConstantKind.Enum => EnumToExpression(constant),
            TypedConstantKind.Type => TypeToExpression(constant.GetValue<INamedTypeSymbol>()),
            TypedConstantKind.Array => ArrayToExpression(constant.Values),
            _ => throw new ArgumentException("错误类型不支持"),
        };
    }
    /// <summary>
    /// 类型常量转化为表达式
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ExpressionSyntax TypeToExpression(this INamedTypeSymbol @type)
    {
        return SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(type.Name));
    }
    /// <summary>
    /// 枚举常量转化为表达式
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ExpressionSyntax EnumToExpression(this TypedConstant @enum)
    {
        var type = @enum.Type
            ?? throw new ArgumentException("缺少枚举类型");
        var value = @enum.Value
            ?? throw new ArgumentException("枚举值无效");
        var field = SymbolReflection.GetEnumField(type, value)
            ?? throw new ArgumentException($"类型: {type.Name},无效的枚举值: {value}");
        var typeName = SyntaxFactory.IdentifierName(type.Name);
        return typeName.Access(field.Name);
    }
    /// <summary>
    /// 转化数组常量为表达式数组
    /// </summary>
    /// <param name="constants"></param>
    /// <returns></returns>
    public static ExpressionSyntax[] ToExpressions(this ImmutableArray<TypedConstant> constants)
    {
        var count = constants.Length;
        var items = new ExpressionSyntax[count];
        for (var i = 0; i < count; i++)
            items[i] = ToExpression(constants[i]);
        return items;
    }
    /// <summary>
    /// 转化数组常量为表达式
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CollectionExpressionSyntax ArrayToExpression(this TypedConstant array)
        => SyntaxGenerator.Collection(ToExpressions(array.Values));
    /// <summary>
    /// 转化数组常量为表达式
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CollectionExpressionSyntax ArrayToExpression(this ImmutableArray<TypedConstant> array)
        => SyntaxGenerator.Collection(ToExpressions(array));
    /// <summary>
    /// 基础类型转化为字面量表达式
    /// </summary>
    /// <param name="primitive"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static LiteralExpressionSyntax PrimitiveToLiteral(this TypedConstant primitive)
    {
        var type = primitive.Type ?? throw new ArgumentException("Type is null");
        return type.SpecialType switch
        {
            SpecialType.System_Boolean => SyntaxGenerator.Literal(GetValue<bool>(primitive)),
            SpecialType.System_Int16 => SyntaxGenerator.Literal(GetValue<short>(primitive)),
            SpecialType.System_UInt16 => SyntaxGenerator.Literal(GetValue<ushort>(primitive)),
            SpecialType.System_Int32 => SyntaxGenerator.Literal(GetValue<int>(primitive)),
            SpecialType.System_UInt32 => SyntaxGenerator.Literal(GetValue<uint>(primitive)),
            SpecialType.System_Int64 => SyntaxGenerator.Literal(GetValue<long>(primitive)),
            SpecialType.System_UInt64 => SyntaxGenerator.Literal(GetValue<ulong>(primitive)),
            SpecialType.System_String => SyntaxGenerator.Literal(GetValue<string>(primitive)),
            SpecialType.System_Char => SyntaxGenerator.Literal(GetValue<char>(primitive)),
            SpecialType.System_Decimal => SyntaxGenerator.Literal(GetValue<decimal>(primitive)),
            SpecialType.System_Double => SyntaxGenerator.Literal(GetValue<double>(primitive)),
            SpecialType.System_Single => SyntaxGenerator.Literal(GetValue<float>(primitive)),
            _ => throw new ArgumentException("字面量类型不支持"),
        };
    }
}
