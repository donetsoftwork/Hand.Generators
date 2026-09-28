using Microsoft.CodeAnalysis;
using System;

namespace Hand.Types;

/// <summary>
/// 类型关联键
/// </summary>
/// <param name="left"></param>
/// <param name="right"></param>
public readonly struct PairTypeSymbolKey(ITypeSymbol left, ITypeSymbol right)
     : IEquatable<PairTypeSymbolKey>
{
    #region 配置
    private readonly ITypeSymbol _left = left;
    private readonly ITypeSymbol _right = right;
    /// <summary>
    /// 映射源类型
    /// </summary>
    public ITypeSymbol Left
        => _left;
    /// <summary>
    /// 映射目标类型
    /// </summary>
    public ITypeSymbol Right
        => _right;
    #endregion
    /// <inheritdoc />
    public override int GetHashCode()
#if NETSTANDARD2_0
        => SymbolEqualityComparer.Default.GetHashCode(_left) * 31 + SymbolEqualityComparer.Default.GetHashCode(_right);
#else
        => HashCode.Combine(SymbolEqualityComparer.Default.GetHashCode(_left), SymbolEqualityComparer.Default.GetHashCode(_right));
#endif
    #region IEquatable
    /// <inheritdoc />
    public bool Equals(PairTypeSymbolKey other)
        => SymbolEqualityComparer.Default.Equals(_left, other._left) && SymbolEqualityComparer.Default.Equals(_right, other._right);
    /// <inheritdoc />
    public override bool Equals(object? other)
        => other is PairTypeSymbolKey key && Equals(key);
    #endregion
    /// <summary>
    /// 解构
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void Deconstruct(out ITypeSymbol left, out ITypeSymbol right)
    {
        left = _left;
        right = _right;
    }
    #region operator
    /// <summary>
    /// 相等运算符，行为与 Equals 一致
    /// </summary>
    public static bool operator ==(PairTypeSymbolKey left, PairTypeSymbolKey right)
        => left.Equals(right);
    /// <summary>
    /// 不等运算符，行为与 Equals 一致
    /// </summary>
    public static bool operator !=(PairTypeSymbolKey left, PairTypeSymbolKey right)
        => !left.Equals(right);
    #endregion
}