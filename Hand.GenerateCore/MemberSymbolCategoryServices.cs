using Hand.Members;
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
    public static bool IsPrimitive(this MemberSymbolCategory category)
        => category == MemberSymbolCategory.Primitive;
    /// <summary>
    /// 是否为可空类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullable(this MemberSymbolCategory category)
        => (category & MemberSymbolCategory.Nullable) == MemberSymbolCategory.Nullable;
    /// <summary>
    /// 是否为枚举类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnum(this MemberSymbolCategory category)
        => (category & MemberSymbolCategory.Enum) == MemberSymbolCategory.Enum;
    /// <summary>
    /// 是否为集合类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsArray(this MemberSymbolCategory category)
        => (category & MemberSymbolCategory.Array) == MemberSymbolCategory.Array;
    /// <summary>
    /// 是否为集合类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollection(this MemberSymbolCategory category)
        => (category & MemberSymbolCategory.Collection) == MemberSymbolCategory.Collection;
    /// <summary>
    /// 是否为实体类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEntity(this MemberSymbolCategory category)
        => (category & MemberSymbolCategory.Entity) == MemberSymbolCategory.Entity;
    /// <summary>
    /// 是否为复合类型
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsComplex(this MemberSymbolCategory category)
        => (category & MemberSymbolCategory.Complex) == MemberSymbolCategory.Complex;
}
