using Hand.Cache;
using Hand.Documentation;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Cachers;

/// <summary>
/// 备注缓存
/// </summary>
public class SummaryCacher()
    : CacheFactoryBase<ISymbol, string>(new DictionaryCacher<ISymbol, string>(new Dictionary<ISymbol, string>(SymbolEqualityComparer.Default)))
{
    /// <inheritdoc />
    protected override string CreateNew(in ISymbol key)
        => CommentParser.GetSummary(key);
    /// <summary>
    /// 获取备注
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string GetSummary(ISymbol symbol)
        => Instance.Get(symbol);
    /// <summary>
    /// 获取备注
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetSummary(ISymbol symbol, string defaultValue)
    {
        var summary = Instance.Get(symbol);
        if (string.IsNullOrEmpty(summary))
            return defaultValue;
        return summary;
    }
    /// <summary>
    /// 单列
    /// </summary>
    public static readonly SummaryCacher Instance = new();
}
