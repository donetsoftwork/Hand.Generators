using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 反射
/// </summary>
public static class SymbolReflection
{
    #region IsGenericType
    /// <summary>
    /// 是否泛型定义
    /// </summary>
    /// <param name="type"></param>
    /// <param name="definitionType">泛型</param>
    /// <returns></returns>
    public static bool IsGenericType(INamedTypeSymbol type, INamedTypeSymbol definitionType)
        => type.IsGenericType && SymbolTypeDescriptor.CheckEquals(definitionType, type.ConstructedFrom);
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
    /// 获取成员
    /// </summary>
    /// <typeparam name="TMember"></typeparam>
    /// <param name="owner"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static IEnumerable<TMember> GetMembers<TMember>(INamespaceOrTypeSymbol owner, SymbolKind kind)
         where TMember : ISymbol
    {
        foreach (var item in owner.GetMembers())
        {
            // 忽略编译器自动生成的成员
            if (item.IsImplicitlyDeclared)
                continue;
            if (item.Kind == kind && item is TMember member)
                yield return member;
        }
    }
    /// <summary>
    /// 获取属性
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<IPropertySymbol> GetProperties(ITypeSymbol type)
        => GetMembers<IPropertySymbol>(type, SymbolKind.Property);
    /// <summary>
    /// 获取字段
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<IFieldSymbol> GetFields(ITypeSymbol type)
        => GetMembers<IFieldSymbol>(type, SymbolKind.Field);
    /// <summary>
    /// 获取枚举字段
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IFieldSymbol? GetEnumField(ITypeSymbol enumType, object value)
    {
        foreach (var field in GetFields(enumType))
        {
            if (field.IsStatic && field.HasConstantValue && Equals(field.ConstantValue, value))
                return field;
        }
        return null;
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
