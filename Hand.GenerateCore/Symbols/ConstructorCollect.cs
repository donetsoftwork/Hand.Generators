using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 构造函数收集器
/// </summary>
/// <param name="members"></param>
public class ConstructorCollect(List<IMethodSymbol> members)
    : SymbolListCollect<IMethodSymbol>(SymbolKind.Method, members)
{
    /// <inheritdoc />
    protected override bool AddCore(IMethodSymbol member)
    {
        return member.MethodKind switch
        {
            MethodKind.Constructor or MethodKind.SharedConstructor or MethodKind.StaticConstructor
                => base.AddCore(member),
            _ => false,
        };
    }
}
