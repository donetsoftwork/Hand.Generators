using Hand.Symbols;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Hand.Reflection;

/// <summary>
/// 标记辅助类
/// </summary>
public static class SymbolAttributeHelper
{
    /// <summary>
    /// 获取标记类检查器
    /// </summary>
    /// <param name="attributeSymbol"></param>
    /// <returns></returns>
    public static Predicate<INamedTypeSymbol> GetAttributeClassChecker(INamedTypeSymbol attributeSymbol)
    {
        return attributeSymbol.IsGenericType ? AttributeDefinition : AttributeEquals;

        bool AttributeEquals(INamedTypeSymbol symbol)
            => SymbolTypeDescriptor.CheckEquals(attributeSymbol, symbol);
        bool AttributeDefinition(INamedTypeSymbol symbol)
            => symbol.IsGenericType(attributeSymbol);
    }
    /// <summary>
    /// 获取标记
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="attributeSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributesByType(IEnumerable<AttributeData> attributes, INamedTypeSymbol attributeSymbol)
    {
        var checker = GetAttributeClassChecker(attributeSymbol);
        foreach (var attribute in attributes)
        {
            // 检查标记
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null)
                continue;
            if (checker(attributeClass))
                yield return attribute;
        }
    }
    /// <summary>
    /// 获取标记
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributeSymbol"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributesByType(ISymbol symbol, INamedTypeSymbol attributeSymbol)
        => GetAttributesByType(symbol.GetAttributes(), attributeSymbol);
    #region GetArgumentConstant
    /// <summary>
    /// 获取标记参数常量
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TypedConstant? GetArgumentConstant(AttributeData attribute, string name)
    {
        foreach (var item in attribute.NamedArguments)
        {
            if (item.Key == name)
                return item.Value;
        }
        return null;
    }
    /// <summary>
    /// 获取标记参数常量
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TypedConstant? GetArgumentConstant(AttributeData attribute, int index)
    {
        var arguments = attribute.ConstructorArguments;
        if (arguments.Length <= index)
            return null;
        return arguments[index];
    }
    /// <summary>
    /// 获取标记参数常量
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TypedConstant? GetArgumentConstant(ISymbol symbol, INamedTypeSymbol attributeType, string name)
    {
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return null;
        return GetArgumentConstant(attribute, name);
    }
    /// <summary>
    /// 获取标记参数常量
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TypedConstant? GetArgumentConstant(ISymbol symbol, INamedTypeSymbol attributeType, int index)
    {
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return null;
        return GetArgumentConstant(attribute, index);
    }
    #endregion
    #region GetArgumentValue
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TValue? GetArgumentValue<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, string name, TValue defaultValue = default!)
    {
        if(attributeType is null)
            return defaultValue;
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return defaultValue;
        return GetArgumentValue(attribute, name, defaultValue);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TValue GetArgumentValue<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, int index, TValue defaultValue = default!)
    {
        if (attributeType is null)
            return defaultValue;
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return defaultValue;
        return GetArgumentValue(attribute, index, defaultValue);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TValue GetArgumentValue<TValue>(AttributeData attribute, string name, TValue defaultValue = default!)
    {
        var constant = GetArgumentConstant(attribute, name);
        if (constant is null) 
            return defaultValue;
        return constant.Value.GetValue(defaultValue);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="index"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static TValue GetArgumentValue<TValue>(AttributeData attribute, int index, TValue defaultValue = default!)
    {
        var arguments = attribute.ConstructorArguments;
        if (arguments.Length <= index)
            return defaultValue;
        return arguments[index].GetValue(defaultValue);
    }
    #endregion
    #region GetArgumentValues
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TValue[] GetArgumentValues<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, string name)
    {
        if (attributeType is null)
            return [];
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return [];
        return GetArgumentValues<TValue>(attribute, name);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TValue[] GetArgumentValues<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, int index)
    {
        if (attributeType is null)
            return [];
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return [];
        return GetArgumentValues<TValue>(attribute, index);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TValue[] GetArgumentValues<TValue>(AttributeData attribute, string name)
    {
        foreach (var item in attribute.NamedArguments)
        {
            if (item.Key == name)
                return item.Value.GetValues<TValue>();
        }
        return [];
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TValue[] GetArgumentValues<TValue>(AttributeData attribute, int index)
    {
        var arguments = attribute.ConstructorArguments;
        if (arguments.Length <= index)
            return [];

        return arguments[index].GetValues<TValue>();
    }
    #endregion
}
