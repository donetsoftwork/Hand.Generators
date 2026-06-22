using Hand.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Members;

/// <summary>
/// 成员
/// </summary>
/// <param name="name">成员名</param>
/// <param name="memberSymbol">成员类型</param>
public class Member(string name, MemberSymbolInfo memberSymbol)
{
    /// <summary>
    /// 成员名
    /// </summary>
    public string Name { get; } = name;
    /// <summary>
    /// 成员类型
    /// </summary>
    public MemberSymbolInfo MemberSymbol { get; } = memberSymbol;

    /// <summary>
    /// 构造来源成员字典
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public static Dictionary<string, Member> GetSourceMembers(Compilation compilation, INamedTypeSymbol from)
    {
        var fields = SymbolReflection.GetPublicFieldsWithBase(from)
            .ToArray();
        var properties = SymbolReflection.GetPublicPropertiesWithBase(from)
            .ToArray();
        
        var members = new Dictionary<string, Member>(properties.Length + fields.Length, StringComparer.OrdinalIgnoreCase‌);
        foreach (var field in fields)
        {
            var name = field.Name;
            if (members.ContainsKey(name))
                continue;
            if (field.Type is not INamedTypeSymbol fieldType)
                continue;
            var symbol = MemberSymbolInfo.Create(compilation, fieldType);
            members.Add(name, new Member(name, symbol));
        }
        foreach (var property in properties)
        {
            if (property.IsIndexer)
                continue;
            var name = property.Name;
            if (members.ContainsKey(name))
                continue;
            if(property.GetMethod is null)
                continue;
            if (property.Type is not INamedTypeSymbol propertyType)
                continue;
            var symbol = MemberSymbolInfo.Create(compilation, propertyType);
            members.Add(name, new Member(name, symbol));
        }
        return members;
    }
}
