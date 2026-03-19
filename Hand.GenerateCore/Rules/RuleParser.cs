using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Rules;

/// <summary>
/// 规则解析器
/// </summary>
/// <param name="include"></param>
/// <param name="exclude"></param>
/// <param name="allRules"></param>
/// <param name="separator"></param>
public class RuleParser(string include, string exclude, HashSet<string> allRules, char[] separator)
{
    /// <summary>
    /// 简单规则
    /// </summary>
    /// <param name="all"></param>
    public RuleParser(HashSet<string> all)
        : this("Include:", "Exclude:", all, [' '])
    {
    }
    #region 配置
    private readonly string _includePrefix = include;
    private readonly string _excludePrefix = exclude;
    private readonly HashSet<string> _allRules = allRules;
    private readonly char[] _separator = separator;
    /// <summary>
    /// 全部规则
    /// </summary>
    public HashSet<string> ALLRules
        => _allRules;
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
    public ICollection<string> Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || text!.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            return _allRules;
        if (text!.Equals("Empty", StringComparison.OrdinalIgnoreCase))
            return [];
        // 逐个解析
        if (text.StartsWith(_includePrefix, StringComparison.OrdinalIgnoreCase))
            return IncludeRules(text.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Skip(1));
        // 逐个排除
        else if (text.StartsWith(_excludePrefix, StringComparison.OrdinalIgnoreCase))
            return ExcludeRules(text.Split(_separator, StringSplitOptions.RemoveEmptyEntries).Skip(1));
        // 逐个解析
        return IncludeRules(text.Split(_separator, StringSplitOptions.RemoveEmptyEntries));
    }
    /// <summary>
    /// 包含列表
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public ICollection<string> IncludeRules(IEnumerable<string> list)
    {
        var checkRules = new HashSet<string>();
        foreach (var item in list)
        {
            if (string.IsNullOrEmpty(item))
                continue;
            foreach (var rule in _allRules)
            {
                if (checkRules.Contains(rule))
                    continue;
                if (string.Equals(rule, item, StringComparison.OrdinalIgnoreCase))
                    checkRules.Add(rule);
            }
        }
        return checkRules;
    }
    /// <summary>
    /// 排除列表
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public ICollection<string> ExcludeRules(IEnumerable<string> list)
    {
        var checkRules = new HashSet<string>(_allRules);
        foreach (var item in list)
        {
            if (string.IsNullOrEmpty(item))
                continue;
            foreach (var rule in _allRules)
            {
                if (checkRules.Contains(rule) && string.Equals(rule, item, StringComparison.OrdinalIgnoreCase))
                    checkRules.Remove(rule);
            }
        }
        return checkRules;
    }
}
