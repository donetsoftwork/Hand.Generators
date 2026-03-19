using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Symbols;

/// <summary>
/// 运算符重载收集器
/// </summary>
/// <param name="members"></param>
public class OperatorCollect(List<IMethodSymbol> members)
    : SymbolListCollect<IMethodSymbol>(SymbolKind.Method, members)
{
    /// <inheritdoc />
    protected override bool AddCore(IMethodSymbol member)
    {
        return member.MethodKind switch
        {
            MethodKind.BuiltinOperator or MethodKind.UserDefinedOperator
                => base.AddCore(member),
            _ => false,
        };
    }
}
