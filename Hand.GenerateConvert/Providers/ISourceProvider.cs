using Hand.Members;
using Hand.Sources;

namespace Hand.Providers;

/// <summary>
/// 生成源提供者
/// </summary>
public interface ISourceProvider
{
    /// <summary>
    /// 获取已生成源
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IGeneratorSource? Get(PairTypeSymbolKey key);
    /// <summary>
    /// 设置生成源
    /// </summary>
    /// <param name="key"></param>
    /// <param name="source"></param>
    void Save(PairTypeSymbolKey key, IGeneratorSource source);
    /// <summary>
    /// 转化为
    /// </summary>
    /// <param name="dest"></param>
    /// <returns></returns>
    ConvertSourceInfo ConvertTo(string dest);
    ///// <summary>
    ///// 转化自
    ///// </summary>
    ///// <param name="source"></param>
    ///// <returns></returns>
    //ConvertSourceInfo ConvertFrom(string source);
    ///// <summary>
    ///// 构造方法
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="dest"></param>
    ///// <returns></returns>
    //ConvertSourceInfo? BuildMethod(INamedTypeSymbol source, INamedTypeSymbol dest); 
}
