using Microsoft.CodeAnalysis;

namespace Hand.Types;

/// <summary>
/// 集合类型信息
/// </summary>
public interface ICollectionTypeInfo : ITypeSymbolInfo
{
    /// <summary>
    /// 子元素类型
    /// </summary>
    ITypeSymbol Element{ get; }
    /// <summary>
    /// 子类型信息
    /// </summary>
    ITypeSymbolInfo ElementInfo { get; }
}
