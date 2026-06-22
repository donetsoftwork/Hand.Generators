using Microsoft.CodeAnalysis;
using System;

namespace Hand.Members;

/// <summary>
/// 类型关联键
/// </summary>
/// <param name="leftSymbol"></param>
/// <param name="rightSymbol"></param>
public readonly struct PairTypeSymbolKey(INamedTypeSymbol leftSymbol, INamedTypeSymbol rightSymbol)
     : IEquatable<PairTypeSymbolKey>
{
    #region 配置
    private readonly INamedTypeSymbol _leftSymbol = leftSymbol;
    private readonly INamedTypeSymbol _rightSymbol = rightSymbol;
    /// <summary>
    /// 映射源类型
    /// </summary>
    public INamedTypeSymbol LeftSymbol
        => _leftSymbol;
    /// <summary>
    /// 映射目标类型
    /// </summary>
    public INamedTypeSymbol RightSymbol
        => _rightSymbol;
    #endregion
    /// <summary>
    /// HashCode
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
#if !NET45
        => HashCode.Combine(SymbolEqualityComparer.Default.GetHashCode(_leftSymbol), SymbolEqualityComparer.Default.GetHashCode(_rightSymbol));
#else
        => _leftSymbol.GetHashCode() ^ _rightSymbol.GetHashCode();
#endif
    #region IEquatable
    /// <summary>
    /// 判同
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(PairTypeSymbolKey other)
        => SymbolEqualityComparer.Default.Equals(_leftSymbol, other._leftSymbol) && SymbolEqualityComparer.Default.Equals(_rightSymbol, other._rightSymbol);
    /// <summary>
    /// 判同
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(object other)
        => other is PairTypeSymbolKey key && Equals(key);
    #endregion
    /// <summary>
    /// 解构
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    public void Deconstruct(out INamedTypeSymbol left, out INamedTypeSymbol right)
    {
        left = _leftSymbol;
        right = _rightSymbol;
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