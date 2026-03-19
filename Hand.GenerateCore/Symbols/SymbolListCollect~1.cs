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
    private readonly List<TSymbol> _members = members;

    /// <inheritdoc />
    protected override bool AddCore(TSymbol member)
    {
        _members.Add(member);
        return true;
    }
}
