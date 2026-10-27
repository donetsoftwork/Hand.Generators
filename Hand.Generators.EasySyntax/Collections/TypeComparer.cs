using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand.Collections;

/// <summary>
/// 类型比较器
/// </summary>
public partial class TypeComparer
    : IEqualityComparer<NameSyntax>, IEqualityComparer<TypeSyntax>
{
    /// <summary>
    /// global别名
    /// </summary>
    public static readonly IdentifierNameSyntax GlobalName = SyntaxFactory.IdentifierName("global");
    #region IEqualityComparer<TypeSyntax>
    /// <inheritdoc />
    bool IEqualityComparer<TypeSyntax>.Equals(TypeSyntax? x, TypeSyntax? y)
    {
        if (x is null || y is null)
            return false;
        return Equal(x, y);
    }
    /// <inheritdoc />
    int IEqualityComparer<TypeSyntax>.GetHashCode(TypeSyntax obj)
        => GetTypeHashCode(obj);

    #endregion
    #region IEqualityComparer<NameSyntax>
    /// <inheritdoc />
    bool IEqualityComparer<NameSyntax>.Equals(NameSyntax? x, NameSyntax? y)
    {
        if (x is null || y is null)
            return false;
        return Equal(x, y);
    }
    /// <inheritdoc />
    int IEqualityComparer<NameSyntax>.GetHashCode(NameSyntax obj)
        => GetNameHashCode(obj);
    #endregion
    #region Equal
    /// <summary>
    /// 类型判同
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equal(TypeSyntax x, TypeSyntax y)
    {
        var kind = x.Kind();
        if (y.IsKind(kind))
        {
            return kind switch
            {
                SyntaxKind.IdentifierName => IdentifierEqual((IdentifierNameSyntax)x, (IdentifierNameSyntax)y),
                SyntaxKind.QualifiedName => QualifiedEqual((QualifiedNameSyntax)x, (QualifiedNameSyntax)y),
                SyntaxKind.AliasQualifiedName => AliasQualifiedEqual((AliasQualifiedNameSyntax)x, (AliasQualifiedNameSyntax)y),
                SyntaxKind.GenericName => GenericEqual((GenericNameSyntax)x, (GenericNameSyntax)y),
                SyntaxKind.PredefinedType => PredefinedEqual((PredefinedTypeSyntax)x, (PredefinedTypeSyntax)y),
                SyntaxKind.ArrayType => ArrayEqual((ArrayTypeSyntax)x, (ArrayTypeSyntax)y),
                SyntaxKind.NullableType => NullableEquals((NullableTypeSyntax)x, (NullableTypeSyntax)y),
                //SyntaxKind.OmittedTypeArgument => true,
                SyntaxKind.TupleType => TupleEqual((TupleTypeSyntax)x, (TupleTypeSyntax)y),
                SyntaxKind.PointerType => PointerEqual((PointerTypeSyntax)x, (PointerTypeSyntax)y),
                SyntaxKind.FunctionPointerType => FunctionPointerEqual((FunctionPointerTypeSyntax)x, (FunctionPointerTypeSyntax)y),
                SyntaxKind.RefType => RefEqual((RefTypeSyntax)x, (RefTypeSyntax)y),
                SyntaxKind.ScopedType => ScopedEqual((ScopedTypeSyntax)x, (ScopedTypeSyntax)y),
                _ => true,
            };
        }
        return false;
    }
    /// <summary>
    /// 类型判同
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equal(NameSyntax x, NameSyntax y)
    {
        var xKind = x.Kind();
        var yKind = y.Kind();
        if (y.IsKind(xKind))
        {
            return xKind switch
            {
                SyntaxKind.IdentifierName => IdentifierEqual((IdentifierNameSyntax)x, (IdentifierNameSyntax)y),
                SyntaxKind.QualifiedName => QualifiedEqual((QualifiedNameSyntax)x, (QualifiedNameSyntax)y),
                SyntaxKind.AliasQualifiedName => AliasQualifiedEqual((AliasQualifiedNameSyntax)x, (AliasQualifiedNameSyntax)y),
                SyntaxKind.GenericName => GenericEqual((GenericNameSyntax)x, (GenericNameSyntax)y),
                _ => false,
            };
        }
        if (xKind == SyntaxKind.AliasQualifiedName)
            return TryGlobalEqual((AliasQualifiedNameSyntax)x, y);
        else if (yKind == SyntaxKind.AliasQualifiedName)
            return TryGlobalEqual((AliasQualifiedNameSyntax)y, x);
        return false;
    }
    /// <summary>
    /// 尝试匹配全局别名
    /// </summary>
    /// <param name="alias"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGlobalEqual(AliasQualifiedNameSyntax alias, NameSyntax other)
        => IsGlobal(alias.Alias) && Equal(alias.Name, other);
    /// <summary>
    /// 类型判同
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equal(SimpleNameSyntax x, SimpleNameSyntax y)
    {
        var kind = x.Kind();
        if (y.IsKind(kind))
        {
            return kind switch
            {
                SyntaxKind.IdentifierName => IdentifierEqual((IdentifierNameSyntax)x, (IdentifierNameSyntax)y),
                SyntaxKind.GenericName => GenericEqual((GenericNameSyntax)x, (GenericNameSyntax)y),
                _ => false,
            };
        }
        return false;
    }
    #endregion
    #region GetHashCode
    /// <summary>
    /// 获取类型HashCode
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNameHashCode(NameSyntax name)
    {
        return name.Kind() switch
        {
            SyntaxKind.IdentifierName => GetIdentifierHashCode((IdentifierNameSyntax)name),
            SyntaxKind.QualifiedName => GetQualifiedHashCode((QualifiedNameSyntax)name),
            SyntaxKind.AliasQualifiedName => GetAliasQualifiedHashCode((AliasQualifiedNameSyntax)name),
            SyntaxKind.GenericName => GetGenericHashCode((GenericNameSyntax)name),
            _ => 0,
        };
    }
    /// <summary>
    /// 获取简单类型HashCode
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetTypeHashCode(SimpleNameSyntax name)
    {
        return name.Kind() switch
        {
            SyntaxKind.IdentifierName => GetIdentifierHashCode((IdentifierNameSyntax)name),
            SyntaxKind.GenericName => GetGenericHashCode((GenericNameSyntax)name),
            _ => 0,
        };
    }
    /// <summary>
    /// 获取类型HashCode
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetTypeHashCode(TypeSyntax type)
    {
        return type.Kind() switch
        {
            SyntaxKind.IdentifierName => GetIdentifierHashCode((IdentifierNameSyntax)type),
            SyntaxKind.QualifiedName => GetQualifiedHashCode((QualifiedNameSyntax)type),
            SyntaxKind.AliasQualifiedName => GetAliasQualifiedHashCode((AliasQualifiedNameSyntax)type),
            SyntaxKind.GenericName => GetGenericHashCode((GenericNameSyntax)type),
            SyntaxKind.PredefinedType => GetPredefinedHashCode((PredefinedTypeSyntax)type),
            SyntaxKind.ArrayType => GetArrayHashCode((ArrayTypeSyntax)type),
            SyntaxKind.NullableType => GetNullableHashCode((NullableTypeSyntax)type),
            SyntaxKind.OmittedTypeArgument => _elementSeed,
            SyntaxKind.TupleType => GetTupleHashCode((TupleTypeSyntax)type),
            SyntaxKind.PointerType => GetPointerHashCode((PointerTypeSyntax)type),
            SyntaxKind.FunctionPointerType => GetFunctionPointerHashCode((FunctionPointerTypeSyntax)type),
            SyntaxKind.RefType => GetRefHashCode((RefTypeSyntax)type),
            SyntaxKind.ScopedType => GetScopedHashCode((ScopedTypeSyntax)type),
            _ => 0,
        };
    }
    #endregion
    /// <summary>
    /// 判断是否为空
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(NameSyntax name)
        => name.IsKind(SyntaxKind.IdentifierName) && IsEmpty((IdentifierNameSyntax)name);
    /// <summary>
    /// 判断是否为空
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(IdentifierNameSyntax name)
        => string.IsNullOrEmpty(name.Identifier.ValueText);
    /// <summary>
    /// 是否为global别名
    /// </summary>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static bool IsGlobal(AliasQualifiedNameSyntax alias)
        => IdentifierEqual(alias.Alias, GlobalName);
    /// <summary>
    /// 是否为global别名
    /// </summary>
    /// <param name="alias"></param>
    /// <returns></returns>
    public static bool IsGlobal(IdentifierNameSyntax alias)
        => IdentifierEqual(alias, GlobalName);

    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly TypeComparer Instance = new();
}
