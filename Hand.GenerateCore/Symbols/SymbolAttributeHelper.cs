using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Symbols;

/// <summary>
/// 标记辅助类
/// </summary>
public static class SymbolAttributeHelper
{
    /// <summary>
    /// 获取标记
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributesByType(IEnumerable<AttributeData> attributes, INamedTypeSymbol attributeType)
    {
        foreach (var attribute in attributes)
        {
            // 检查标记
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null)
                continue;
            if (SymbolTypeDescriptor.CheckEquals(attributeType, attributeClass))
                yield return attribute;
        }
    }
    /// <summary>
    /// 获取标记
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>
    public static IEnumerable<AttributeData> GetAttributesByType(ISymbol symbol, INamedTypeSymbol attributeType)
        => GetAttributesByType(symbol.GetAttributes(), attributeType);
    #region GetArgumentValue
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TValue? GetArgumentValue<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, string name)
    {
        if(attributeType is null)
            return default;
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return default;
        return GetArgumentValue<TValue>(attribute, name);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="symbol"></param>
    /// <param name="attributeType"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TValue? GetArgumentValue<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, int index)
    {
        if (attributeType is null)
            return default;
        var attribute = GetAttributesByType(symbol, attributeType)
            .FirstOrDefault();
        if (attribute is null)
            return default;
        return GetArgumentValue<TValue>(attribute, index);
    }
    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TValue? GetArgumentValue<TValue>(AttributeData attribute, string name)
    {
        foreach (var item in attribute.NamedArguments)
        {
            if (item.Key == name)
            {
                if (item.Value.Value is TValue value)
                    return value;
                return default;
            }
        }
        return default;
    }

    /// <summary>
    /// 获取标记参数值
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="attribute"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TValue? GetArgumentValue<TValue>(AttributeData attribute, int index)
    {
        var arguments = attribute.ConstructorArguments;
        if (arguments.Length <= index)
            return default;
        return arguments[index].GetValue<TValue?>();
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
    public static IEnumerable<TValue> GetArgumentValues<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, string name)
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
    public static IEnumerable<TValue> GetArgumentValues<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, int index)
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
    public static IEnumerable<TValue> GetArgumentValues<TValue>(AttributeData attribute, string name)
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
    public static IEnumerable<TValue> GetArgumentValues<TValue>(AttributeData attribute, int index)
    {
        var arguments = attribute.ConstructorArguments;
        if (arguments.Length <= index)
            return [];

        return arguments[index].GetValues<TValue>();
    }
    #endregion
}
