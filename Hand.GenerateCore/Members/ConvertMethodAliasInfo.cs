using Microsoft.CodeAnalysis;
using System;

namespace Hand.Members;

/// <summary>
/// 转化方法别名
/// </summary>
/// <param name="name"></param>
/// <param name="alias"></param>
/// <param name="isStatic"></param>
/// <param name="filter"></param>
public class ConvertMethodAliasInfo(string name, string alias, bool isStatic, Func<IMethodSymbol, bool> filter)
    : ConvertMethodInfo(name, isStatic, filter)
{
    /// <summary>
    /// 转化方法别名
    /// </summary>
    /// <param name="name"></param>
    /// <param name="alias"></param>
    /// <param name="isStatic"></param>
    public ConvertMethodAliasInfo(string name, string alias, bool isStatic)
        : this(name, alias, isStatic, CheckFilter(name, alias, isStatic))
    {
    }
    #region 配置
    private readonly string _alias = alias;
    /// <summary>
    /// 别名
    /// </summary>
    public string Alias 
        => _alias;
    #endregion
    /// <summary>
    /// 构造筛选规则
    /// </summary>
    /// <param name="name"></param>
    /// <param name="alias"></param>
    /// <param name="isStatic"></param>
    /// <returns></returns>
    public static Func<IMethodSymbol, bool> CheckFilter(string name, string alias, bool isStatic)
    {
        if (isStatic)
            return m => m.IsStatic && (EqualName(m, name) || EqualName(m, alias));
        return m => !m.IsStatic && (EqualName(m, name) || EqualName(m, alias));
    }
}
