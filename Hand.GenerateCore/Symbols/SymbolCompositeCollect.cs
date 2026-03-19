using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 构造复合收集器
/// </summary>
public class SymbolCompositeCollect : ISymbolCollect
{
    private readonly List<ISymbolCollect> _collects = [];
    private readonly HashSet<object> _keys = [];


    /// <inheritdoc />
    public bool Add(ISymbol symbol)
    {
        // 忽略编译器自动生成的成员
        if (symbol.IsImplicitlyDeclared)
            return false;
        foreach (var item in _collects)
        {
            if(item.Add(symbol))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 判断key是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(object key)
        => _keys.Contains(key);
    /// <summary>
    /// 添加收集器
    /// </summary>
    /// <param name="key"></param>
    /// <param name="collect"></param>
    public void Use(object key, ISymbolCollect collect)
    {
        _keys.Add(key);
        _collects.Add(collect);
    }
}
