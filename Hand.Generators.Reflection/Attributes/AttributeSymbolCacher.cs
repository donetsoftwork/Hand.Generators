using Hand.Cache;
using Hand.Members;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Attributes;

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
    /// <param name="symbol"></param>
    /// <param name="target"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool VerifyTarget(ISymbol symbol, AttributeTargets target, AttributeData attribute)
    {
        var attributeClass = attribute.AttributeClass;
        if (attributeClass is null)
            return false;
        var info = Get(attributeClass);
        return info.VerifyTarget(symbol, target);
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
        return info.Targets.HasFlag(target);
    }
    /// <summary>
    /// 判断类型
    /// </summary>
    /// <param name="class"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool Verify(INamedTypeSymbol @class, AttributeData attribute)
    {
        if (@class.IsEnum())
            return VerifyTarget(@class, AttributeTargets.Enum, attribute);
        if (@class.IsValueType)
            return VerifyTarget(@class, AttributeTargets.Struct, attribute);
        return VerifyTarget(@class, AttributeTargets.Class, attribute);
    }
    /// <summary>
    /// 判断方法
    /// </summary>
    /// <param name="method"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool Verify(IMethodSymbol method, AttributeData attribute)
    {
        return method.MethodKind switch
        {
            MethodKind.Constructor or MethodKind.StaticConstructor
                => VerifyTarget(method, AttributeTargets.Constructor, attribute),
            MethodKind.DeclareMethod or MethodKind.Ordinary or MethodKind.ReducedExtension or MethodKind.LocalFunction
                => VerifyTarget(method, AttributeTargets.Method, attribute),
            _ => false,
        };
    }
    /// <summary>
    /// 判断属性
    /// </summary>
    /// <param name="property"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool Verify(IPropertySymbol property, AttributeData attribute)
        => VerifyTarget(property, AttributeTargets.Property, attribute);
    /// <summary>
    /// 判断字段
    /// </summary>
    /// <param name="field"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool Verify(IFieldSymbol field, AttributeData attribute)
        => VerifyTarget(field, AttributeTargets.Field, attribute);
    /// <summary>
    /// 判断参数
    /// </summary>
    /// <param name="parameter"></param>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public bool Verify(IParameterSymbol parameter, AttributeData attribute)
        => VerifyTarget(parameter, AttributeTargets.Parameter, attribute);
    /// <summary>
    /// 按范围过滤特性
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributes"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerable<AttributeData> CheckByTarget(ISymbol symbol, IEnumerable<AttributeData> attributes, AttributeTargets target)
    {
        foreach (var attribute in attributes)
        {
            if(VerifyTarget(symbol, target, attribute))
                yield return attribute;
        }
    }
    /// <summary>
    /// 按范围过滤特性
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerable<AttributeData> CheckByTarget(IEnumerable<AttributeData> attributes, AttributeTargets target)
    {
        foreach (var attribute in attributes)
        {
            if (VerifyTarget(target, attribute))
                yield return attribute;
        }
    }
    /// <summary>
    /// 获取特性标记
    /// </summary>
    /// <param name="sourse"></param>
    /// <param name="dest"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public AttributeData[] GetAttributes(IMemberInfo sourse, ISymbol dest, AttributeTargets target)
    {
        var attributes = sourse.GetAttributes();
        if (attributes.Length == 0)
            return [];
        return [.. CheckByTarget(dest, attributes, target)];
    }
    /// <summary>
    /// 获取特性标记
    /// </summary>
    /// <param name="sourse"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public AttributeData[] GetAttributes(IMemberInfo sourse, AttributeTargets target)
    {
        var attributes = sourse.GetAttributes();
        if (attributes.Length == 0)
            return [];
        return [.. CheckByTarget(attributes, target)];
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
