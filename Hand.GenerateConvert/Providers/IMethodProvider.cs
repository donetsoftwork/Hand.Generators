using Hand.Members;
using Microsoft.CodeAnalysis;

namespace Hand.Providers;

/// <summary>
/// 方法提供者
/// </summary>
public interface IMethodProvider
{
    /// <summary>
    /// 获取转化方法
    /// </summary>
    /// <param name="info"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    IMethodSymbol? GetConvertMethod(ConvertMethodInfo info, ITypeSymbol dest);
}
