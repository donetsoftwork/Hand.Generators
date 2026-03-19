using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 反射
/// </summary>
public static class SymbolReflection
{
    #region IsNullable
    /// <summary>
    /// 是否可空类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullable(INamedTypeSymbol type)
        => IsGenericType(type, SpecialType.System_Nullable_T);
    #endregion
    #region IsGenericType
    /// <summary>
    /// 是否泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="definitionType">泛型</param>
    /// <returns></returns>
    public static bool IsGenericType(INamedTypeSymbol type, INamedTypeSymbol definitionType)
        => type.IsGenericType && SymbolTypeDescriptor.CheckEquals(definitionType, type.ConstructedFrom);
    /// <summary>
    /// 是否泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static bool IsGenericType(INamedTypeSymbol type, SpecialType genericType)
        => type.IsGenericType && type.ConstructedFrom.SpecialType == genericType;
    #endregion
    #region HasGenericType
    /// <summary>
    /// 判断是否包含泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="definitionType"></param>
    /// <returns></returns>
    public static bool HasGenericType(INamedTypeSymbol type, INamedTypeSymbol definitionType)
    {
        if (IsGenericType(type, definitionType))
            return true;
        foreach (var subType in type.Interfaces)
        {
            if (IsGenericType(subType, definitionType))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 判断是否包含泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static bool HasGenericType(INamedTypeSymbol type, SpecialType genericType)
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
    #endregion
    #region GetGenericCloseInterfaces
    /// <summary>
    /// 获取泛型闭合接口
    /// </summary>
    /// <param name="type"></param>
    /// <param name="definitionType"></param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetGenericCloseInterfaces(INamedTypeSymbol type, INamedTypeSymbol definitionType)
    {
        if (IsGenericType(type, definitionType))
        {
            yield return type;
            yield break;
        }
        var interfaces = type.Interfaces;
        foreach (var item in interfaces)
        {
            if (IsGenericType(item, definitionType))
                yield return item;
        }
    }
    /// <summary>
    /// 获取泛型闭合接口
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetGenericCloseInterfaces(INamedTypeSymbol type, SpecialType genericType)
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
    #endregion
    /// <summary>
    /// 判断是否需要检查null
    /// </summary>
    /// <param name="declareType"></param>
    /// <returns></returns>
    public static bool CheckNullable(INamedTypeSymbol declareType)
    {
        return declareType.IsGenericType || !declareType.IsValueType;
    }
}
