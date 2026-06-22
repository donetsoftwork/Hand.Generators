using Hand.Members;
using Microsoft.CodeAnalysis;

namespace Hand.Providers;

/// <summary>
/// 
/// </summary>
/// <param name="info"></param>
/// <param name="symbol"></param>
public class ExtensionProvider(TypeNameInfo info, INamedTypeSymbol? symbol)
{
    private readonly TypeNameInfo _info = info;
    private readonly INamedTypeSymbol? _symbol = symbol;
}
