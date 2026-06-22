using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Hand.Members;

/// <summary>
/// 参数成员
/// </summary>
/// <param name="name">成员名</param>
/// <param name="memberSymbol">成员类型</param>
/// <param name="kind">Parameter/Property/Field</param>
public class MemberArgument(string name, MemberSymbolInfo memberSymbol, SymbolKind kind)
    : Member(name, memberSymbol)
{
    /// <summary>
    /// 成员种类(Parameter/Property/Field)
    /// </summary>
    public SymbolKind Kind { get; } = kind;

    /// <summary>
    /// 获取方法参数成员
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Dictionary<string, MemberArgument> GetMethodArguments(Compilation compilation, IMethodSymbol method)
    {
        var parameters = method.Parameters;
        var members = new Dictionary<string, MemberArgument>(parameters.Length, StringComparer.OrdinalIgnoreCase);
        foreach (var parameter in parameters)
        {
            var name = parameter.Name;
            var parameterType = (INamedTypeSymbol)parameter.Type;
            var symbol = MemberSymbolInfo.Create(compilation, parameterType);
            members.Add(name, new MemberArgument(name, symbol, SymbolKind.Parameter));
        }
        return members;
    }
    private static readonly char[] _fieldTrimChars = ['_'];
    /// <summary>
    /// 处理字段成员，加入成员参数列表
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="members"></param>
    /// <param name="fields"></param>
    public static void CheckFields(Compilation compilation, Dictionary<string, MemberArgument> members, IEnumerable<IFieldSymbol> fields)
    {
        foreach (var field in fields)
        {
            // 字段名去掉前缀下划线
            var name = field.Name.TrimStart(_fieldTrimChars);
            if (members.ContainsKey(name))
                continue;
            if (field.Type is not INamedTypeSymbol fieldType)
                continue;
            var symbol = MemberSymbolInfo.Create(compilation, fieldType);
            members.Add(name, new MemberArgument(name, symbol, SymbolKind.Field));
        }
    }
    /// <summary>
    /// 处理属性成员，加入成员参数列表
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="members"></param>
    /// <param name="properties"></param>
    public static void CheckProperties(Compilation compilation, Dictionary<string, MemberArgument> members, IEnumerable<IPropertySymbol> properties)
    {
        foreach (var property in properties)
        {
            if (property.IsIndexer)
                continue;
            var name = property.Name;
            if (members.ContainsKey(name))
                continue;
            if (property.SetMethod is null)
                continue;
            if (property.Type is not INamedTypeSymbol propertyType)
                continue;
            var symbol = MemberSymbolInfo.Create(compilation, propertyType);
            members.Add(name, new MemberArgument(name, symbol, SymbolKind.Property));
        }
    }
        ///// <summary>
        ///// 处理成员参数
        ///// </summary>
        ///// <param name="compilation"></param>
        ///// <param name="symbol"></param>
        ///// <param name="requirePublic"></param>
        ///// <param name="members"></param>
        //public static void CheckMembers(Compilation compilation, INamedTypeSymbol symbol, bool requirePublic, Dictionary<string, MemberArgument> members)
        //{
        //    var fields = requirePublic ? SymbolReflection.GetPublicFieldsWithBase(symbol) : SymbolReflection.GetFieldsWithBase(symbol);
        //    var properties = requirePublic ? SymbolReflection.GetPublicPropertiesWithBase(symbol) : SymbolReflection.GetPropertiesWithBase(symbol);


    //    foreach (var field in fields)
    //    {
    //        var name = field.Name;
    //        if (members.ContainsKey(name))
    //            continue;
    //        if (field.Type is not INamedTypeSymbol fieldType)
    //            continue;
    //        var symbol = MemberSymbolInfo.Create(compilation, fieldType);
    //        members.Add(name, new Member(name, symbol));
    //    }
    //    foreach (var property in properties)
    //    {
    //        if (property.IsIndexer)
    //            continue;
    //        var name = property.Name;
    //        if (members.ContainsKey(name))
    //            continue;
    //        if (property.GetMethod is null)
    //            continue;
    //        if (property.Type is not INamedTypeSymbol propertyType)
    //            continue;
    //        var symbol = MemberSymbolInfo.Create(compilation, propertyType);
    //        members.Add(name, new Member(name, symbol));
    //    }
    //}
    }
