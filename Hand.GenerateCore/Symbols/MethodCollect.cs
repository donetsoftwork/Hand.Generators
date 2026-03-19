using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 方法收集器
/// </summary>
/// <param name="members"></param>
public class MethodCollect(List<IMethodSymbol> members)
    : SymbolListCollect<IMethodSymbol>(SymbolKind.Method, members)
{
    /// <inheritdoc />
    protected override bool AddCore(IMethodSymbol member)
    {
        return member.MethodKind switch
        {
            MethodKind.DeclareMethod or MethodKind.Ordinary
                => base.AddCore(member),
            _ => false,
        };
    }
}
