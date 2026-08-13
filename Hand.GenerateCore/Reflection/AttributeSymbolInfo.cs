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
    /// <param name="symbol"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool VerifyTarget(ISymbol symbol, AttributeTargets target)
    {
        if(!_targets.HasFlag(target))
            return false;
        if (_allowMultiple)
            return true;
        return !SymbolAttributeHelper.GetAttributesByType(symbol, _attributeSymbol).Any();
    }
    /// <summary>
    /// 判断类型
    /// </summary>
    /// <param name="class"></param>
    /// <returns></returns>
    public bool VerifyClass(INamedTypeSymbol @class)
    {
        if (@class.IsEnum())
            return VerifyTarget(@class, AttributeTargets.Enum);
        if (@class.IsValueType)
            return VerifyTarget(@class, AttributeTargets.Struct);
        return VerifyTarget(@class, AttributeTargets.Class);
    }
    /// <summary>
    /// 判断属性
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public bool VerifyProperty(IPropertySymbol property)
        => VerifyTarget(property, AttributeTargets.Property);
    /// <summary>
    /// 判断字段
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public bool VerifyField(IFieldSymbol field)
        => VerifyTarget(field, AttributeTargets.Field);
    /// <summary>
    /// 判断参数
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool VerifyParameter(IParameterSymbol parameter)
        => VerifyTarget(parameter, AttributeTargets.Parameter);
}
