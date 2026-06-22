using Hand.Members;
using Microsoft.CodeAnalysis;

namespace Hand.Symbols;

/// <summary>
/// 
/// </summary>
/// <param name="symbol"></param>
public class ConvertSymbol(INamedTypeSymbol symbol)
{
    private readonly INamedTypeSymbol _symbol = symbol;
    private readonly TypeNameInfo _info = TypeNameInfo.GetExtensionInfo(symbol);

}
