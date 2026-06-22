using Hand.Builders;
using Hand.Members;
using Microsoft.CodeAnalysis;

namespace Hand.Providers;

/// <summary>
/// 实例方法提供者
/// </summary>
/// <param name="symbol"></param>
public class MethodProvider(INamedTypeSymbol symbol)
    : IMethodProvider
{
    #region 配置
    private readonly INamedTypeSymbol _symbol = symbol;
    #endregion

    /// <inheritdoc />
    public IMethodSymbol? GetConvertMethod(ConvertMethodInfo info, INamedTypeSymbol dest)
        => ConvertBuilder.GetInstanceMethod(_symbol, dest, info.Filter);

    /// <summary>
    /// 构造方法提供者
    /// </summary>
    /// <param name="source"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static IMethodProvider Create(INamedTypeSymbol source, INamedTypeSymbol? extension = null)
    {
        var isPrimitiveType = source.IsPrimitiveType();
        if (extension is null)
        {
            if (isPrimitiveType)
                return MethodEmptyProvider.Instance;
            return new MethodProvider(source);
        }
        if (isPrimitiveType)
            return new StaticMethodProvider(extension, source);
        return new ComplexMethodProvider(new MethodProvider(source), new StaticMethodProvider(extension, source));
    }
}
