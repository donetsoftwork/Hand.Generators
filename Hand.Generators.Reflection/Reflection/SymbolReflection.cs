using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand.Reflection;

/// <summary>
/// 反射
/// </summary>
public static class SymbolReflection
{
    #region GetMembers
    /// <summary>
    /// 获取本身及基类非私有成员
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<ISymbol> GetMembersWithBase(INamedTypeSymbol type)
    {
        foreach (var item in type.GetMembers())
            yield return item;
        var baseType = type.BaseType;
        if (baseType is not null)
        {
            foreach (var item in GeNonPrivateMembersWithBase(baseType))
                yield return item;
        }
    }
    /// <summary>
    /// 获取本身及基类非私有成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static IEnumerable<ISymbol> GetMembersWithBase(INamedTypeSymbol type, INamedTypeSymbol baseType)
    {
        foreach (var item in type.GetMembers())
            yield return item;
        foreach (var item in GeNonPrivateMembersWithBase(baseType))
            yield return item;
    }
    /// <summary>
    /// 获取本身及基类非私有成员
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<ISymbol> GeNonPrivateMembersWithBase(INamedTypeSymbol type)
    {
        foreach (var item in type.GetMembers())
        {
            if (item.DeclaredAccessibility != Accessibility.Private)
                yield return item;
        }
        var baseType = type.BaseType;
        if (baseType is not null)
        {
            foreach (var item in GeNonPrivateMembersWithBase(baseType))
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
            if (item.Kind == kind && item is TMember member)
                yield return member;
        }
    }
    /// <summary>
    /// 获取本身及基类成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static IEnumerable<TMember> GetMembersWithBase<TMember>(INamedTypeSymbol type, SymbolKind kind)
        where TMember : ISymbol
    {
        foreach (var item in GetMembersWithBase(type))
        {
            if (item.Kind == kind && item is TMember member)
                yield return member;
        }
    }
    /// <summary>
    /// 获取本身及基类成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static IEnumerable<TMember> GetMembersWithBase<TMember>(INamedTypeSymbol type, INamedTypeSymbol baseType, SymbolKind kind)
        where TMember : ISymbol
    {
        foreach (var item in GetMembersWithBase(type, baseType))
        {
            if (item.Kind == kind && item is TMember member)
                yield return member;
        }
    }
    /// <summary>
    /// 获取本身及基类公开成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static IEnumerable<TMember> GetPublicMembersWithBase<TMember>(INamedTypeSymbol type, SymbolKind kind)
        where TMember : ISymbol
    {
        foreach (var item in GetMembersWithBase(type))
        {
            if (item.Kind == kind && item.DeclaredAccessibility == Accessibility.Public && item is TMember member)
                yield return member;
        }
    }
    #endregion
    #region GetProperties
    /// <summary>
    /// 获取属性
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IPropertySymbol> GetProperties(ITypeSymbol type)
        => GetMembers<IPropertySymbol>(type, SymbolKind.Property);
    /// <summary>
    /// 获取本身及基类属性
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IPropertySymbol> GetPropertiesWithBase(INamedTypeSymbol type)
         => GetMembersWithBase<IPropertySymbol>(type, SymbolKind.Property);
    /// <summary>
    /// 获取本身及基类属性
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static IEnumerable<IPropertySymbol> GetPropertiesWithBase(INamedTypeSymbol type, INamedTypeSymbol baseType)
         => GetMembersWithBase<IPropertySymbol>(type, baseType, SymbolKind.Property);
    /// <summary>
    /// 获取本类及基类的Public属性
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IPropertySymbol> GetPublicPropertiesWithBase(INamedTypeSymbol type)
        => GetPublicMembersWithBase<IPropertySymbol>(type, SymbolKind.Property);
    #endregion
    #region GetFields
    /// <summary>
    /// 获取字段
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IFieldSymbol> GetFields(ITypeSymbol type)
        => GetMembers<IFieldSymbol>(type, SymbolKind.Field);
    /// <summary>
    /// 获取本身及基类字段
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IFieldSymbol> GetFieldsWithBase(INamedTypeSymbol type)
        => GetMembersWithBase<IFieldSymbol>(type, SymbolKind.Field);
    /// <summary>
    /// 获取本身及基类字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static IEnumerable<IFieldSymbol> GetFieldsWithBase(INamedTypeSymbol type, INamedTypeSymbol baseType)
        => GetMembersWithBase<IFieldSymbol>(type, baseType, SymbolKind.Field);
    /// <summary>
    /// 获取本类及基类的Public字段
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IFieldSymbol> GetPublicFieldsWithBase(INamedTypeSymbol type)
        => GetPublicMembersWithBase<IFieldSymbol>(type, SymbolKind.Field);
    /// <summary>
    /// 获取枚举字段
    /// </summary>
    /// <param name="enumType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IFieldSymbol? GetEnumField(ITypeSymbol enumType, IComparable value)
    {
        foreach (var field in GetFields(enumType))
        {
            if (field.IsStatic && field.HasConstantValue && value.CompareTo(field.ConstantValue) == 0)
                return field;
        }
        return null;
    }
    #endregion
    /// <summary>
    /// 获取构造函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skipImplicitly"></param>
    /// <returns></returns>
    public static IEnumerable<IMethodSymbol> GetConstructors(INamedTypeSymbol type, bool skipImplicitly = true)
    {
        foreach (var constructor in type.InstanceConstructors)
        {
            // 是否忽略编译器自动生成的成员
            if (skipImplicitly && constructor.IsImplicitlyDeclared)
                continue;
            yield return constructor;
        }
    }
    #region GetMethods
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IMethodSymbol> GetMethods(ITypeSymbol type)
        => GetMembers<IMethodSymbol>(type, SymbolKind.Method);
    /// <summary>
    /// 获取本类及基类方法
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IMethodSymbol> GetMethodsWithBase(INamedTypeSymbol type)
        => GetMembersWithBase<IMethodSymbol>(type, SymbolKind.Method);
    /// <summary>
    /// 获取本类及基类的Public方法
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IMethodSymbol> GetPublicMethodsWithBase(INamedTypeSymbol type)
        => GetPublicMembersWithBase<IMethodSymbol>(type, SymbolKind.Method);
    #endregion
    /// <summary>
    /// 按参数类型匹配
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="parameterTypes"></param>
    /// <returns></returns>
    public static bool MatchParameterType(ImmutableArray<IParameterSymbol> parameters, ITypeSymbol[] parameterTypes)
    {
        var typeCount = parameterTypes.Length;
        if (parameters.Length != typeCount)
            return false;
        for (var i = 0; i < typeCount; i++)
        {
            if (!SymbolEqualityComparer.Default.Equals(parameterTypes[i], parameters[i].Type))
                return false;
        }
        return true;
    }
    /// <summary>
    /// 按单一参数类型匹配
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public static bool MatchSingle(ImmutableArray<IParameterSymbol> parameters, ITypeSymbol parameterType)
    {
        if (parameters.Length != 1)
            return false;
        return SymbolEqualityComparer.Default.Equals(parameterType, parameters[0].Type);
    }
    /// <summary>
    /// 按单一参数类型匹配
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="parameterType"></param>
    /// <returns></returns>
    public static bool MatchFirst(ImmutableArray<IParameterSymbol> parameters, ITypeSymbol parameterType)
    {
        if (parameters.Length == 0)
            return false;
        return SymbolEqualityComparer.Default.Equals(parameterType, parameters[0].Type);
    }
    /// <summary>
    /// 是否存在空构造函数
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static IMethodSymbol? GetEmptyConstructor(INamedTypeSymbol symbol)
        => GetMethods(symbol)
        .FirstOrDefault(IsEmptyConstructor);
    /// <summary>
    /// 是否为空构造函数
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsEmptyConstructor(IMethodSymbol method)
        => method.MethodKind == MethodKind.Constructor
        && method.Parameters.Length == 0
        && method.DeclaredAccessibility == Accessibility.Public;
    /// <summary>
    /// 检查原始类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? CheckOriginalSymbol(Compilation compilation, ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedTypeSymbol)
            return GetOriginalSymbol(compilation, namedTypeSymbol) ?? namedTypeSymbol;
        return null;
    }
    /// <summary>
    /// 检查原始类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static INamedTypeSymbol CheckOriginalSymbol(Compilation compilation, INamedTypeSymbol type)
        => GetOriginalSymbol(compilation, type) ?? type;
    /// <summary>
    /// 获取原始类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetOriginalSymbol(Compilation compilation, INamedTypeSymbol type)
    {
        var entityId = compilation.GetTypeByMetadataName("Hand.Primitives.IEntityId");
        if (entityId is null)
            return null;
        if (type.IsInterface(entityId))
            return compilation.GetSpecialType(SpecialType.System_Int64);
        var entityProperty = compilation.GetTypeByMetadataName("Hand.Primitives.IEntityProperty`1");
        if (entityProperty is null)
            return null;
        var @interface = type.GetGenericCloseInterfaces(entityProperty)
            .FirstOrDefault();
        if (@interface is null)
            return null;
        if (@interface.TypeArguments.FirstOrDefault() is INamedTypeSymbol original)
            return original;
        return null;
    }
    ///// <summary>
    ///// 判断是否需要检查null
    ///// </summary>
    ///// <param name="declareType"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static bool CheckNullable(INamedTypeSymbol declareType)
    //    => declareType.IsGenericType || !declareType.IsValueType;
}
