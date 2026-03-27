using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 列表收集器
/// </summary>
/// <typeparam name="TSymbol"></typeparam>
/// <param name="kind"></param>
/// <param name="members"></param>
public class SymbolListCollect<TSymbol>(SymbolKind kind, List<TSymbol> members)
    : SymbolCollect<TSymbol>(kind)
    where TSymbol : ISymbol
{
    #region 配置
    private readonly List<TSymbol> _members = members;
    #endregion

    /// <summary>
    /// 列表收集器
    /// </summary>
    /// <param name="kind"></param>
    public SymbolListCollect(SymbolKind kind)
        : this(kind, [])
    {
    }
    /// <inheritdoc />
    protected override bool AddCore(TSymbol member)
    {
        _members.Add(member);
        return true;
    }
}
