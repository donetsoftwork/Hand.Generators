using Microsoft.CodeAnalysis;
using System;

namespace Hand.Members;

/// <summary>
/// 转化方法名
/// </summary>
/// <param name="name"></param>
/// <param name="isStatic"></param>
/// <param name="filter"></param>
public class ConvertMethodInfo(string name, bool isStatic, Func<IMethodSymbol, bool> filter)
{
    /// <summary>
    /// 转化方法名
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isStatic"></param>
    public ConvertMethodInfo(string name, bool isStatic)
        : this(name, isStatic, CheckFilter(name, isStatic))
    {
    }
    #region 配置
    private readonly string _name = name;
    private readonly bool _isStatic = isStatic;
    private readonly Func<IMethodSymbol, bool> _filter = filter;

    /// <summary>
    /// 方法名
    /// </summary>
    public string Name 
        => _name;
    /// <summary>
    /// 是否静态方法
    /// </summary>
    public bool IsStatic
        => _isStatic;
    /// <summary>
    /// 方法按名筛选规则
    /// </summary>
    public Func<IMethodSymbol, bool> Filter 
        => _filter;
    #endregion
    /// <summary>
    /// 忽略大小写比较Name
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool EqualName(ISymbol symbol, string name)
        => string.Equals(symbol.Name, name, StringComparison.OrdinalIgnoreCase‌);
    /// <summary>
    /// 构造转化方法名
    /// </summary>
    /// <param name="name"></param>
    /// <param name="prefix"></param>
    /// <param name="isStatic"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ConvertMethodInfo Create(string name, string prefix, bool isStatic, string action = "To")
    {
        var methodName = action + name;
        if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase‌))
        {
            var methodName0 = action + name.Substring(prefix.Length);
            return new ConvertMethodAliasInfo(methodName0, methodName, isStatic);
        }
        return new(methodName, isStatic);
    }
    /// <summary>
    /// 构造筛选规则
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isStatic"></param>
    /// <returns></returns>
    public static Func<IMethodSymbol, bool> CheckFilter(string name, bool isStatic)
    {
        if (isStatic)
            return m => m.IsStatic && EqualName(m, name);
        return m => !m.IsStatic && EqualName(m, name);
    }
}
