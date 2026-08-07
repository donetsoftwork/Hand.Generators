using Hand.Cache;
using Hand.Members;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Cachers;

/// <summary>
/// 标记符号信息缓存
/// </summary>
public class AttributeSymbolCacher(Compilation compilation)
    : CacheFactoryBase<INamedTypeSymbol, AttributeSymbolInfo>(new DictionaryCacher<INamedTypeSymbol, AttributeSymbolInfo>(new Dictionary<INamedTypeSymbol, AttributeSymbolInfo>(SymbolEqualityComparer.Default)))
{
    /// <summary>
    /// 主键标记类型
    /// </summary>
    protected readonly INamedTypeSymbol? _usageAttributeType = GetUsageAttributeType(compilation);
    /// <inheritdoc />
    protected override AttributeSymbolInfo CreateNew(in INamedTypeSymbol key)
    {
        if (_usageAttributeType is null)
            return new(key, AttributeTargets.All, true);
        var attribute = SymbolAttributeHelper.GetAttributesByType(key, _usageAttributeType)
            .FirstOrDefault();
        // attribute不为null,除非不是Attribute类
        if (attribute is null)
            return new(key, default, false);
        return new(key, GetTargets(attribute), GetAllowMultiple(attribute));
    }

    /// <summary>
    /// 判断应用的范围
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool VerifyTarget(AttributeTargets target, AttributeData attribute)
    {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null)
            return false;
        var info = Get(attributeClass);
        return info.VerifyTarget(target);
    }
    /// <summary>
    /// 按范围过滤特性
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerable<AttributeData> FilterByTarget(IEnumerable<AttributeData> attributes, AttributeTargets target)
    {
        foreach (var attribute in attributes)
        {
            if(VerifyTarget(target, attribute))
                yield return attribute;
        }
    }
    /// <summary>
    /// 获取特性标记
    /// </summary>
    /// <param name="sourseMember"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public AttributeData[] GetAttributes(SymbolMember sourseMember, AttributeTargets target)
    {
        var attributes = sourseMember.GetAttributes();
        if (attributes.Length == 0)
            return [];
        return [.. FilterByTarget(attributes, target)];
    }
    /// <summary>
    /// 获取可应用的范围
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static AttributeTargets GetTargets(AttributeData attribute)
    {
        var constant = SymbolAttributeHelper.GetArgumentConstant(attribute, 0);
        if (constant is null)
            return AttributeTargets.All;
        return constant.Value.GetEnum(AttributeTargets.All);
    }
    /// <summary>
    /// 是否可以多个
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static bool GetAllowMultiple(AttributeData attribute)
    {
        var constant = SymbolAttributeHelper.GetArgumentConstant(attribute, "AllowMultiple");
        // 默认false
        if (constant is null)
            return false;
        return constant.Value.GetPrimitive(false);
    }
    ///// <summary>
    ///// 是否可以继承
    ///// </summary>
    ///// <param name="attribute"></param>
    ///// <returns></returns>
    //public static bool GetInherited(AttributeData attribute)
    //{
    //    var constant = SymbolAttributeHelper.GetArgumentConstant(attribute, "Inherited");
    //    // 默认true
    //    if (constant is null)
    //        return true;
    //    return constant.Value.GetPrimitive(true);
    //}
    /// <summary>
    /// 获取Key特性类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetUsageAttributeType(Compilation compilation)
        => compilation.GetTypeByMetadataName("System.AttributeUsageAttribute");
}
