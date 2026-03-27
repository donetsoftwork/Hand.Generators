using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 收集成员
/// </summary>
/// <typeparam name="TSymbol"></typeparam>
/// <param name="kind"></param>
public class SymbolCollect<TSymbol>(SymbolKind kind)
    : ISymbolCollect
    where TSymbol : ISymbol
{
    #region 配置
    /// <summary>
    /// 成员种类
    /// </summary>
    private readonly SymbolKind _kind = kind;
    #endregion
    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public virtual bool Add(ISymbol symbol)
    {
        if (symbol.Kind == _kind && symbol is TSymbol member)
            return AddCore(member);
        return false;
    }
    /// <summary>
    /// 收集
    /// </summary>
    /// <param name="members"></param>
    /// <returns></returns>
    public IEnumerable<TSymbol> Select(IEnumerable<ISymbol> members)
    {
        foreach (ISymbol item in members)
        {
            if (item.Kind == _kind && item is TSymbol member)
                yield return member;
        }
    }
    /// <summary>
    /// 添加
    /// </summary>
    /// <param name="member"></param>
    protected virtual bool AddCore(TSymbol member)
        => false;
}
