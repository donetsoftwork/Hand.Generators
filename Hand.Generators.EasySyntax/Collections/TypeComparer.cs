using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand.Collections;

/// <summary>
/// 类型比较器
/// </summary>
public class TypeComparer : IEqualityComparer<TypeSyntax>
{
    private const int _seed = 23;

    #region IEqualityComparer<TypeSyntax>
    /// <inheritdoc />
    bool IEqualityComparer<TypeSyntax>.Equals(TypeSyntax? x, TypeSyntax? y)
    {
        if (x is null || y is null)
            return false;
        return Equals(x, y);
    }
    /// <inheritdoc />
    int IEqualityComparer<TypeSyntax>.GetHashCode(TypeSyntax obj)
        => GetHashCode(obj);
    #endregion
    #region GetHashCode
    /// <inheritdoc />
    public static int GetHashCode(TypeSyntax obj)
    {
        var seed = _seed;
        return GetTypeHashCode(obj, _seed, ref seed);
    }
    /// <summary>
    /// 获取SyntaxToken的HashCode
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetTokenHashCode(SyntaxToken token)
        => token.ValueText.GetHashCode();
    /// <summary>
    /// 获取简单标识名HashCode
    /// </summary>
    /// <param name="name"></param>
    /// <param name="hashCode"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetIdentifierHashCode(IdentifierNameSyntax name, int hashCode, int seed)
        => GetTokenHashCode(name.Identifier) ^ unchecked(hashCode + seed);
    /// <summary>
    /// 获取预定义类型HashCode
    /// </summary>
    /// <param name="name"></param>
    /// <param name="hashCode"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetPredefinedHashCode(PredefinedTypeSyntax name, int hashCode, int seed)
        => GetTokenHashCode(name.Keyword) ^ unchecked(hashCode + seed);
    /// <summary>
    /// 获取简单标识名HashCode
    /// </summary>
    /// <param name="genericName"></param>
    /// <param name="hashCode"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetGenericHashCode(GenericNameSyntax genericName, int hashCode, ref int seed)
    {
        hashCode = GetTokenHashCode(genericName.Identifier) ^ unchecked(hashCode + seed);
        foreach (var argument in genericName.TypeArgumentList.Arguments)
            hashCode = GetTypeHashCode(argument, hashCode, ref seed);
        return hashCode;
    }
    /// <summary>
    /// 获取类型HashCode
    /// </summary>
    /// <param name="name"></param>
    /// <param name="hashCode"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetTypeHashCode(TypeSyntax name, int hashCode, ref int seed)
    {
        return name.Kind() switch
        {
            SyntaxKind.IdentifierName => GetIdentifierHashCode((IdentifierNameSyntax)name, hashCode, seed),
            SyntaxKind.QualifiedName => GetQualifiedHashCode((QualifiedNameSyntax)name, hashCode, ref seed),
            SyntaxKind.GenericName => GetGenericHashCode((GenericNameSyntax)name, hashCode, ref seed),
            SyntaxKind.PredefinedType => GetPredefinedHashCode((PredefinedTypeSyntax)name, hashCode, seed),
            _ => hashCode,
        };
    }
    /// <summary>
    /// 获取限定标识名HashCode
    /// </summary>
    /// <param name="qualifiedName"></param>
    /// <param name="hashCode"></param>
    /// <param name="seed"></param>
    /// <returns></returns>
    internal static int GetQualifiedHashCode(QualifiedNameSyntax qualifiedName, int hashCode, ref int seed)
    {
        unchecked
        {
            seed = (seed << 1) | _seed;
        }
        return GetTypeHashCode(qualifiedName.Left, GetTypeHashCode(qualifiedName.Right, hashCode, ref seed), ref seed);
    }
    #endregion
    #region Equals
    /// <summary>
    /// 类型判同
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals(TypeSyntax x, TypeSyntax y)
    {
        var kind = x.Kind();
        if (y.IsKind(kind))
        {
            return kind switch
            {
                SyntaxKind.IdentifierName => IdentifierEquals((IdentifierNameSyntax)x, (IdentifierNameSyntax)y),
                SyntaxKind.QualifiedName => QualifiedEquals((QualifiedNameSyntax)x, (QualifiedNameSyntax)y),
                SyntaxKind.GenericName => GenericEquals((GenericNameSyntax)x, (GenericNameSyntax)y),
                SyntaxKind.PredefinedType => PredefinedEquals((PredefinedTypeSyntax)x, (PredefinedTypeSyntax)y),
                _ => false,
            };
        }
        return false;
    }
    /// <summary>
    /// 比较字符片段
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static bool TokenEquals(SyntaxToken x, SyntaxToken y)
        => string.Equals(x.ValueText, y.ValueText, System.StringComparison.Ordinal);
    /// <summary>
    /// 比较标识名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IdentifierEquals(IdentifierNameSyntax x, IdentifierNameSyntax y)
        => TokenEquals(x.Identifier, y.Identifier);
    /// <summary>
    /// 比较标识名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool PredefinedEquals(PredefinedTypeSyntax x, PredefinedTypeSyntax y)
        => TokenEquals(x.Keyword, y.Keyword);
    ///// <summary>
    ///// <summary>
    ///// 比较简单名
    ///// </summary>
    ///// <param name="x"></param>
    ///// <param name="y"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //internal static bool SimpleEquals(SimpleNameSyntax x, SimpleNameSyntax y)
    //    => TokenEquals(x.Identifier, y.Identifier);
    /// <summary>
    /// 比较限定名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool QualifiedEquals(QualifiedNameSyntax x, QualifiedNameSyntax y)
        => Equals(x.Left, y.Left) && Equals(x.Right, y.Right);
    /// <summary>
    /// 比较限定名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static bool GenericEquals(GenericNameSyntax x, GenericNameSyntax y)
    {
        if (!TokenEquals(x.Identifier, y.Identifier))
            return false;
        var xArguments = x.TypeArgumentList.Arguments;
        var yArguments = y.TypeArgumentList.Arguments;
        var count = xArguments.Count;
        if (count != yArguments.Count)
            return false;
        for (var i = 0; i < count; i++)
        {
            if (!Equals(xArguments[i], yArguments[i]))
                return false;
        }
        return true;
    }
    #endregion
    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly TypeComparer Instance = new();
}
