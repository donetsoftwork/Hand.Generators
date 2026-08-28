using Hand.Types;
using System;

namespace Hand.Members;

/// <summary>
/// 类型关联键
/// </summary>
/// <param name="left"></param>
/// <param name="right"></param>
public readonly struct PairSymbolInfoKey(ITypeSymbolInfo left, ITypeSymbolInfo right)
     : IEquatable<PairSymbolInfoKey>
{
    #region 配置
    private readonly ITypeSymbolInfo _left = left;
    private readonly ITypeSymbolInfo _right = right;
    /// <summary>
    /// 映射源类型
    /// </summary>
    public ITypeSymbolInfo Left
        => _left;
    /// <summary>
    /// 映射目标类型
    /// </summary>
    public ITypeSymbolInfo Right
        => _right;
    #endregion
    /// <inheritdoc />
    public override int GetHashCode()
#if NETSTANDARD2_0
        => (_left, _right).GetHashCode();
#else
        => HashCode.Combine(_left.GetHashCode(), _right.GetHashCode());
#endif
    #region IEquatable
    /// <inheritdoc />
    public bool Equals(PairSymbolInfoKey other)
        => _left.Equals(other._left) && _right.Equals(other._right);
    /// <inheritdoc />
    public override bool Equals(object? other)
        => other is PairSymbolInfoKey key && Equals(key);
    #endregion
    /// <summary>
    /// 解构
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void Deconstruct(out ITypeSymbolInfo left, out ITypeSymbolInfo right)
    {
        left = _left;
        right = _right;
    }
    #region operator
    /// <summary>
    /// 相等运算符，行为与 Equals 一致
    /// </summary>
    public static bool operator ==(PairSymbolInfoKey left, PairSymbolInfoKey right)
        => left.Equals(right);
    /// <summary>
    /// 不等运算符，行为与 Equals 一致
    /// </summary>
    public static bool operator !=(PairSymbolInfoKey left, PairSymbolInfoKey right)
        => !left.Equals(right);
    #endregion
}