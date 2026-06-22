using Microsoft.CodeAnalysis;

namespace Hand.Enums;

/// <summary>
/// 枚举配置
/// </summary>
/// <param name="enumType"></param>
/// <param name="underType"></param>
/// <param name="capacity"></param>
public class EnumBundle(INamedTypeSymbol enumType, INamedTypeSymbol underType, int capacity)
    : EnumBundleBase<EnumField>(enumType, underType, capacity)    
{
}
