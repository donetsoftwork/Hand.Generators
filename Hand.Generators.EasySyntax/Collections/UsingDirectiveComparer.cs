using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hand.Collections;

/// <summary>
/// 引用比较器
/// </summary>
public class UsingDirectiveComparer
    : IEqualityComparer<UsingDirectiveSyntax>
{
    private const int _seed = 47 ;
    #region IEqualityComparer<UsingDirectiveSyntax>
    /// <inheritdoc />
    bool IEqualityComparer<UsingDirectiveSyntax>.Equals(UsingDirectiveSyntax? x, UsingDirectiveSyntax? y)
    {
        if (x is null || y is null)
            return false;
        return Equals(x, y);
    }
    #endregion
    #region GetHashCode
    /// <inheritdoc />
    public int GetHashCode(UsingDirectiveSyntax obj)
    {
        unchecked
        {
            return GetAliasHashCode(obj.Alias)
                + obj.GlobalKeyword.GetHashCode()
                + (obj.StaticKeyword.GetHashCode() << 1)
                + (obj.UnsafeKeyword.GetHashCode() << 2)
                + TypeComparer.GetHashCode(obj.NamespaceOrType);
        }
    }
    /// <summary>
    /// 获取别名HashCode
    /// </summary>
    /// <param name="alias"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetAliasHashCode(NameEqualsSyntax? alias)
        => alias is null ? _seed : TypeComparer.GetTokenHashCode(alias.Name.Identifier);
    #endregion
    #region Equals
    /// <summary>
    /// 引用判同
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals(UsingDirectiveSyntax x, UsingDirectiveSyntax y)
    {
        return TypeComparer.TokenEquals(x.GlobalKeyword, y.GlobalKeyword)
            && TypeComparer.TokenEquals(x.StaticKeyword, y.StaticKeyword)
            && TypeComparer.TokenEquals(x.UnsafeKeyword, y.UnsafeKeyword)
            && AliasEquals(x.Alias, y.Alias)
            && TypeComparer.Equals(x.NamespaceOrType, y.NamespaceOrType);
    }    
    /// <summary>
    /// 比较别名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static bool AliasEquals(NameEqualsSyntax? x, NameEqualsSyntax? y)
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;
        return TypeComparer.IdentifierEquals(x.Name, y.Name);
    }    
    #endregion
    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly UsingDirectiveComparer Instance = new();
}
