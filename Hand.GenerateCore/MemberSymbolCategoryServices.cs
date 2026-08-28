using Hand.Reflection;
using Hand.Types;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// TypeSymbolKind扩展方法
/// </summary>
public static partial class GenerateCoreServices
{
    /// <summary>
    /// 是否为基础类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitive(this ITypeSymbolInfo type)
        => type.Kind == TypeSymbolKind.Primitive;
    /// <summary>
    /// 是否为枚举类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnum(this ITypeSymbolInfo type)
        => type.Kind == TypeSymbolKind.Enum;
    /// <summary>
    /// 是否为集合类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsArray(this ITypeSymbolInfo type)
        => type.Kind == TypeSymbolKind.Array;
    /// <summary>
    /// 是否为集合类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollection(this ITypeSymbolInfo type)
        => type.Kind == TypeSymbolKind.Collection;
    /// <summary>
    /// 是否为实体类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEntity(this ITypeSymbolInfo type)
        => type.Kind == TypeSymbolKind.Entity;
    /// <summary>
    /// 是否为复合类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsComplex(this ITypeSymbolInfo type)
        => type.Kind == TypeSymbolKind.Complex;
    /// <summary>
    /// 是否为未知类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUnknow(this ITypeSymbolInfo type)
        => type.Kind == TypeSymbolKind.Unknow;
    
}
