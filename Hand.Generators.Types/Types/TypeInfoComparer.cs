using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Types;

/// <summary>
/// 类型信息比较
/// </summary>
/// <param name="original"></param>
public class TypeInfoComparer(SymbolEqualityComparer original)
    : IEqualityComparer<ITypeSymbolInfo>
{
    private readonly SymbolEqualityComparer _original = original;
    /// <inheritdoc />
    public bool Equals(ITypeSymbolInfo? x, ITypeSymbolInfo? y)
    {
        if(x is null || y is null)
            return false;
        return _original.Equals(x.Original, y.Original);
    }
    /// <inheritdoc />
    public int GetHashCode(ITypeSymbolInfo obj)
        => _original.GetHashCode(obj.Original);
    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly TypeInfoComparer Default = new(SymbolEqualityComparer.Default);
}
