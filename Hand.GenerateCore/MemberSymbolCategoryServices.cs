using Hand.Reflection;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// MemberSymbolCategory扩展方法
/// </summary>
public static partial class GenerateCoreServices
{
    /// <summary>
    /// 是否为基础类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitive(this TypeSymbolKind category)
        => category == TypeSymbolKind.Primitive;
    /// <summary>
    /// 是否为可空类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullable(this TypeSymbolKind category)
        => (category & TypeSymbolKind.Nullable) == TypeSymbolKind.Nullable;
    /// <summary>
    /// 是否为枚举类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnum(this TypeSymbolKind category)
        => (category & TypeSymbolKind.Enum) == TypeSymbolKind.Enum;
    /// <summary>
    /// 是否为集合类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsArray(this TypeSymbolKind category)
        => (category & TypeSymbolKind.Array) == TypeSymbolKind.Array;
    /// <summary>
    /// 是否为集合类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollection(this TypeSymbolKind category)
        => (category & TypeSymbolKind.Collection) == TypeSymbolKind.Collection;
    /// <summary>
    /// 是否为实体类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEntity(this TypeSymbolKind category)
        => (category & TypeSymbolKind.Entity) == TypeSymbolKind.Entity;
    /// <summary>
    /// 是否为复合类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsComplex(this TypeSymbolKind category)
        => (category & TypeSymbolKind.Complex) == TypeSymbolKind.Complex;
}
