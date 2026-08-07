using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace Hand.Reflection;

/// <summary>
/// 标记符号信息
/// </summary>
public class AttributeSymbolInfo(INamedTypeSymbol attributeSymbol, AttributeTargets targets, bool allowMultiple/*, bool inherited*/)
{
    #region 配置
    private readonly INamedTypeSymbol _attributeSymbol = attributeSymbol;
    private readonly AttributeTargets _targets = targets;
    private readonly bool _allowMultiple = allowMultiple;
    //private readonly bool _inherited = inherited;

    /// <summary>
    /// 标记符号
    /// </summary>
    public INamedTypeSymbol AttributeSymbol
        => _attributeSymbol;
    /// <summary>
    /// 可应用的范围
    /// </summary>
    public AttributeTargets Targets 
        => _targets;
    /// <summary>
    /// 是否可以多个
    /// </summary>
    public bool AllowMultiple 
        => _allowMultiple;
    ///// <summary>
    ///// 是否可以继承
    ///// </summary>
    //public bool Inherited 
    //    => _inherited;
    #endregion
    /// <summary>
    /// 判断应用的范围
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool VerifyTarget(AttributeTargets target)
        => (target & _targets) == target;
    /// <summary>
    /// 判断类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public bool VerifyClass(INamedTypeSymbol symbol)
    {
        if ((AttributeTargets.Class & _targets) == AttributeTargets.Class)
        {
            if (_allowMultiple)
                return true;
            if (SymbolAttributeHelper.GetAttributesByType(symbol, _attributeSymbol).Any())
                return false;
            return true;
        }
        return false;
    }

}
