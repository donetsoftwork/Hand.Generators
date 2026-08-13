using Hand.Cachers;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Members;

/// <summary>
/// 属性成员
/// </summary>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
public class PropertyMember(IPropertySymbol original, TypeSymbolInfo symbolInfo)
    : SymbolMember(original.Name, original, symbolInfo)
{
    #region 配置
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.Property;
    /// <summary>
    /// 原始属性
    /// </summary>
    public new IPropertySymbol Original { get; } = original;
    #endregion
    /// <summary>
    /// 处理属性成员，加入成员参数列表
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="members"></param>
    /// <param name="properties"></param>
    public static void CheckProperties(TypeSymbolCacher typeSymbols, Dictionary<string, SymbolMember> members, IEnumerable<IPropertySymbol> properties)
    {
        foreach (var property in properties)
        {
            var name = property.Name;
            if (members.ContainsKey(name))
                continue;
            var symbol = typeSymbols.GetByType(property.Type);
            if (symbol is null)
                continue;
            members.Add(name, new PropertyMember(property, symbol));
        }
    }
}
