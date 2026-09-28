using Hand.Members;
using Hand.Methods;
using Microsoft.CodeAnalysis;

namespace Hand.Providers;

/// <summary>
/// 静态函数提供者
/// </summary>
/// <param name="declare"></param>
/// <param name="symbol"></param>
public class StaticMethodProvider(INamedTypeSymbol declare, INamedTypeSymbol symbol)
    : IMethodProvider
{
    #region 配置
    private readonly INamedTypeSymbol _declare = declare;
    private readonly INamedTypeSymbol _symbol = symbol;
    #endregion

    /// <inheritdoc />
    IMethodSymbol? IMethodProvider.GetConvertMethod(ConvertMethodInfo info, ITypeSymbol dest)
        => MethodProvider.GetStaticMethod(_declare, _symbol, dest, info.Filter);
}
