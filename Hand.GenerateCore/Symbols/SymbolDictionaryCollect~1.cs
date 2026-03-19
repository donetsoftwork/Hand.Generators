using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 字典收集器
/// </summary>
/// <typeparam name="TSymbol"></typeparam>
/// <param name="kind"></param>
/// <param name="members"></param>
public class SymbolDictionaryCollect<TSymbol>(SymbolKind kind, Dictionary<string, TSymbol> members)
    : SymbolCollect<TSymbol>(kind)
    where TSymbol : ISymbol
{
    private readonly Dictionary<string, TSymbol> _members = members;

    /// <inheritdoc />
    protected override bool AddCore(TSymbol member)
    {
        _members[member.Name] = member;
        return true;
    }
}
