using Hand.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 实体转化构造器
/// </summary>
public partial class ConvertBuilder
{
    public static IEnumerable<IMethodSymbol> GetConstructors(INamedTypeSymbol type)
        => SymbolReflection.GetMethods(type)
                .Where(m => m.MethodKind == MethodKind.Constructor);
    //public static IMethodSymbol GetConstructor(INamedTypeSymbol type)
}
