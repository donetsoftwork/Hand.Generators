using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 常量扩展方法
/// </summary>
public static partial class ReflectionServices
{
    /// <summary>
    /// 获取常量值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="constant"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TValue GetValue<TValue>(this TypedConstant constant, TValue defaultValue = default!)
    {
        return constant.Kind switch
        {
            TypedConstantKind.Primitive => GetPrimitive(constant, defaultValue),
            TypedConstantKind.Enum => GetEnum(constant, defaultValue),
            TypedConstantKind.Type => GetTypeAdapt(constant, defaultValue),
            TypedConstantKind.Array => GetArrayAdapt(constant, defaultValue),
            _ => defaultValue,
        };
    }
    /// <summary>
    /// 适配类型符号
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="constant"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private static TValue GetTypeAdapt<TValue>(this TypedConstant constant, TValue defaultValue)
    {
        if(defaultValue is null)
            return (TValue)GetTypeSymbol(constant);
        if (defaultValue is INamedTypeSymbol symbol)
            return (TValue)GetTypeSymbol(constant, symbol);
        return defaultValue;
    }
    private static readonly MethodInfo _getArrayMethod = typeof(ReflectionServices).GetMethod("GetValues")!;
    /// <summary>
    /// 适配数组
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="constant"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private static TValue GetArrayAdapt<TValue>(this TypedConstant constant, TValue defaultValue)
    {
        var arrayType = typeof(TValue);
        if(arrayType.IsArray && arrayType.GetElementType() is Type valueType)
            return (TValue)_getArrayMethod.MakeGenericMethod(valueType).Invoke(null, [constant])!;
        return defaultValue;
    }
    /// <summary>
    /// 获取类型符号
    /// </summary>
    /// <param name="constant"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static INamedTypeSymbol GetTypeSymbol(this TypedConstant constant, INamedTypeSymbol defaultValue = default!)
    {
        if (constant.Value is INamedTypeSymbol symbol)
            return symbol;
        return defaultValue;
    }
    /// <summary>
    /// 基础类型转化为字面量表达式
    /// </summary>
    /// <param name="primitive"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TValue GetPrimitive<TValue>(this TypedConstant primitive, TValue defaultValue = default!)
    {
        if (primitive.Value is TValue value)
            return value;
        return defaultValue;
    }
    /// <summary>
    /// 基础类型转化为字面量表达式
    /// </summary>
    /// <param name="constant"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TEnum GetEnum<TEnum>(this TypedConstant constant, TEnum defaultValue = default!)
    {
        var value = constant.Value;
        if(value is null)
            return defaultValue;
        return (TEnum)Enum.ToObject(typeof(TEnum), constant.Value!);
    }
    /// <summary>
    /// 获取数组
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="constant"></param>
    /// <returns></returns>
    public static TValue[] GetValues<TValue>(this TypedConstant constant)
    {
        var values = constant.Values;
        var items = new TValue[values.Length];
        var i = 0;
        foreach (var value in values)
            items[i++] = GetValue<TValue>(value);
        return items;
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
            TypedConstantKind.Type => TypeToExpression(GetTypeSymbol(constant)),
            TypedConstantKind.Array => ArrayToExpression(constant.Values),
            _ => throw new ArgumentException("错误类型不支持"),
        };
    }
    /// <summary>
    /// 类型常量转化为表达式
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ExpressionSyntax TypeToExpression(this INamedTypeSymbol @type)
        => SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(type.Name));
    /// <summary>
    /// 枚举常量转化为表达式
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ExpressionSyntax EnumToExpression(this TypedConstant @enum)
    {
        var type = @enum.Type ?? throw new ArgumentException("缺少枚举类型");
        var value = @enum.Value;
        if (value is IComparable comparable)
        {
            var field = SymbolReflection.GetEnumField(type, comparable)
                ?? throw new ArgumentException($"类型: {type.Name},无效的枚举值: {value}");
            var typeName = SyntaxFactory.IdentifierName(type.Name);
            return typeName.Access(field.Name);
        }
        throw new ArgumentException("枚举值无效: {value}");
    }
    /// <summary>
    /// 转化数组常量为表达式数组
    /// </summary>
    /// <param name="constants"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ExpressionSyntax[] ToExpressions(this TypedConstant[] constants)
        => Array.ConvertAll(constants, static constant => ToExpression(constant));
    /// <summary>
    /// 转化数组常量为表达式
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CollectionExpressionSyntax ArrayToExpression(this TypedConstant array)
        => SyntaxGenerator.Collection(ToExpressions([.. array.Values]));
    /// <summary>
    /// 转化数组常量为表达式
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CollectionExpressionSyntax ArrayToExpression(this ImmutableArray<TypedConstant> array)
        => SyntaxGenerator.Collection(ToExpressions([.. array]));
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
            SpecialType.System_Boolean => SyntaxGenerator.Literal(GetPrimitive<bool>(primitive)),
            SpecialType.System_Int16 => SyntaxGenerator.Literal(GetPrimitive<short>(primitive)),
            SpecialType.System_UInt16 => SyntaxGenerator.Literal(GetPrimitive<ushort>(primitive)),
            SpecialType.System_Int32 => SyntaxGenerator.Literal(GetPrimitive<int>(primitive)),
            SpecialType.System_UInt32 => SyntaxGenerator.Literal(GetPrimitive<uint>(primitive)),
            SpecialType.System_Int64 => SyntaxGenerator.Literal(GetPrimitive<long>(primitive)),
            SpecialType.System_UInt64 => SyntaxGenerator.Literal(GetPrimitive<ulong>(primitive)),
            SpecialType.System_String => SyntaxGenerator.Literal(GetPrimitive<string>(primitive)),
            SpecialType.System_Char => SyntaxGenerator.Literal(GetPrimitive<char>(primitive)),
            SpecialType.System_Decimal => SyntaxGenerator.Literal(GetPrimitive<decimal>(primitive)),
            SpecialType.System_Double => SyntaxGenerator.Literal(GetPrimitive<double>(primitive)),
            SpecialType.System_Single => SyntaxGenerator.Literal(GetPrimitive<float>(primitive)),
            _ => throw new ArgumentException("字面量类型不支持"),
        };
    }
}
