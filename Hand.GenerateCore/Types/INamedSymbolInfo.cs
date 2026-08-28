using Microsoft.CodeAnalysis;

namespace Hand.Types;

/// <summary>
/// 命名类型信息
/// </summary>
public interface INamedSymbolInfo : ITypeSymbolInfo
{
    /// <summary>
    /// 原始类型
    /// </summary>
    new INamedTypeSymbol Original { get; }
    /// <summary>
    /// 类型
    /// </summary>
    new INamedTypeSymbol Symbol { get; }
}
