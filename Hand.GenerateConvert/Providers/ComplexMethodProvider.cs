using Hand.Members;
using Microsoft.CodeAnalysis;

namespace Hand.Providers;

/// <summary>
/// 复合方法提供者
/// </summary>
/// <param name="original"></param>
/// <param name="extension"></param>
public class ComplexMethodProvider(IMethodProvider original, IMethodProvider extension)
    : IMethodProvider
{
    #region 配置
    private readonly IMethodProvider _original = original;
    private readonly IMethodProvider _extension = extension;
    /// <summary>
    /// 原始方法提供者
    /// </summary>
    public IMethodProvider Original
        => _original;
    /// <summary>
    /// 扩展方法提供者
    /// </summary>
    public IMethodProvider Extension
        => _extension;
    #endregion
    /// <inheritdoc />
    public IMethodSymbol? GetConvertMethod(ConvertMethodInfo info, ITypeSymbol dest)
        => _original.GetConvertMethod(info, dest) ?? _extension.GetConvertMethod(info, dest);
}
