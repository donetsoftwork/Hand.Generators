using Hand.Members;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand.Symbols;

/// <summary>
/// 反射
/// </summary>
public static class SymbolReflection
{
    #region GetMembers
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
    /// 获取本身及基类成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static IEnumerable<TMember> GetMembersWithBase<TMember>(INamedTypeSymbol type, SymbolKind kind)
        where TMember : ISymbol
    {
        foreach (var item in GetMembers<TMember>(type, kind))
            yield return item;
        var baseType = type.BaseType;
        if (baseType is not null)
        {
            foreach (var item in GetNotPrivateMembersWithBase<TMember>(baseType, kind))
                yield return item;
        }
    }
    /// <summary>
    /// 获取本身及基类非私有成员
    /// </summary>
    /// <param name="type"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static IEnumerable<TMember> GetNotPrivateMembersWithBase<TMember>(INamedTypeSymbol type, SymbolKind kind)
        where TMember : ISymbol
    {
        foreach (var item in type.GetMembers())
        {
            // 忽略编译器自动生成的成员
            if (item.IsImplicitlyDeclared)
                continue;
            // 剔除Private
            if (item.Kind == kind && item.DeclaredAccessibility != Accessibility.Private && item is TMember member)
                yield return member;
        }
        var baseType = type.BaseType;
        if (baseType is not null)
        {
            foreach (var item in GetNotPrivateMembersWithBase<TMember>(baseType, kind))
                yield return item;
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
        foreach (var item in type.GetMembers())
        {
            // 忽略编译器自动生成的成员
            if (item.IsImplicitlyDeclared)
                continue;
            // 获取Public成员
            if (item.Kind == kind && item.DeclaredAccessibility == Accessibility.Public && item is TMember member)
                yield return member;
        }
        var baseType = type.BaseType;
        if (baseType is not null)
        {
            foreach (var item in GetPublicMembersWithBase<TMember>(baseType, kind))
                yield return item;
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
    {
        ITypeSymbol? baseType0 = type.BaseType;
        if (SymbolTypeDescriptor.CheckEquals(baseType, baseType0))
            return GetMembersWithBase<IPropertySymbol>(type, SymbolKind.Property);
        var properties = GetMembers<IPropertySymbol>(type, SymbolKind.Property);
        // 父类非私有属性
        var baseProperties = GetNotPrivateMembersWithBase<IPropertySymbol>(baseType, SymbolKind.Property);
        return properties.Concat(baseProperties);
    }
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
    {
        ITypeSymbol? baseType0 = type.BaseType;
        if (SymbolTypeDescriptor.CheckEquals(baseType, baseType0))
            return GetMembersWithBase<IFieldSymbol>(type, SymbolKind.Field);
        var fields = GetMembers<IFieldSymbol>(type, SymbolKind.Field);
        // 父类非私有字段
        var baseFields = GetNotPrivateMembersWithBase<IFieldSymbol>(baseType, SymbolKind.Field);
        return fields.Concat(baseFields);
    }
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
    /// 获取构造函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isImplicitlyDeclared"></param>
    /// <returns></returns>
    public static IEnumerable<IMethodSymbol> GetConstructors(INamedTypeSymbol type, bool isImplicitlyDeclared = true)
    {
        foreach (var item in type.GetMembers())
        {
            // 是否忽略编译器自动生成的成员
            if (isImplicitlyDeclared && item.IsImplicitlyDeclared)
                continue;
            if (item.Kind == SymbolKind.Method && item is IMethodSymbol method && method.MethodKind == MethodKind.Constructor)
                yield return method;
        }
    }
    #region GetMethods
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol type)
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
        var interfaces = type.AllInterfaces;
        var entityId = compilation.GetTypeByMetadataName("Hand.Models.IEntityId");
        if (entityId is null)
            return null;
        if (interfaces.Any(item => SymbolTypeDescriptor.CheckEquals(entityId, item)))
            return compilation.GetSpecialType(SpecialType.System_Int64);
        var entityProperty = compilation.GetTypeByMetadataName("Hand.Models.IEntityProperty`1");
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
    /// <summary>
    /// 判断是否需要检查null
    /// </summary>
    /// <param name="declareType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckNullable(INamedTypeSymbol declareType)
        => declareType.IsGenericType || !declareType.IsValueType;
}
