using Microsoft.CodeAnalysis;

namespace Hand.Symbols;

/// <summary>
/// 收集符号成员
/// </summary>
public interface ISymbolCollect
{
    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    bool Add(ISymbol symbol);
}
