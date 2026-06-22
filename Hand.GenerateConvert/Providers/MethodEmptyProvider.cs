using Hand.Members;
using Microsoft.CodeAnalysis;

namespace Hand.Providers;

/// <summary>
/// 空方法提供者
/// </summary>
public class MethodEmptyProvider: IMethodProvider
{
    private MethodEmptyProvider()
    {
    }

    IMethodSymbol? IMethodProvider.GetConvertMethod(ConvertMethodInfo info, INamedTypeSymbol dest)
        => null;
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly MethodEmptyProvider Instance = new();
}
