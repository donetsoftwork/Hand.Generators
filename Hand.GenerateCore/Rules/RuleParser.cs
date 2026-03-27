using System;
using System.Collections.Frozen;
using System.Linq;

namespace Hand.Rules;

/// <summary>
/// 规则解析器
/// </summary>
/// <param name="include"></param>
/// <param name="exclude"></param>
/// <param name="separator"></param>
public class RuleParser(string include, string exclude, char[] separator)
{
    #region 配置
    private readonly string _includePrefix = include;
    private readonly string _excludePrefix = exclude;
    private readonly char[] _separator = separator;
    /// <summary>
    /// 包含标记
    /// </summary>
    public string IncludePrefix
        => _includePrefix;
    /// <summary>
    /// 排除标记
    /// </summary>
    public string ExcludePrefix
        => _excludePrefix;
    /// <summary>
    /// 分割符
    /// </summary>
    public char[] Separator
        => _separator;
    #endregion
    /// <summary>
    /// 解析
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public IRule Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || text!.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            return DefaultRule.Instance;
        if (text!.Equals("Empty", StringComparison.OrdinalIgnoreCase))
            return EmptyRule.Instance;
        // 逐个排除
        if (text.StartsWith(_excludePrefix, StringComparison.OrdinalIgnoreCase))
            return new ExcludeRule(text.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Skip(1).Distinct().ToFrozenSet());
        // 逐个解析
        return new IncludeRule(text.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Skip(1).Distinct().ToFrozenSet());
    }
    /// <summary>
    /// 默认实例
    /// </summary>
    public static RuleParser Default
        => Inner.Instance;
    class Inner
    {
        /// <summary>
        /// 默认实例
        /// </summary>

        public static readonly RuleParser Instance = new("Include:", "Exclude:", [' ']);
    }
}
