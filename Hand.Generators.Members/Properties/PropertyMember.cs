using Hand.Builders;
using Hand.Members;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Properties;

/// <summary>
/// 属性成员
/// </summary>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
public class PropertyMember(IPropertySymbol original, ITypeSymbolInfo symbolInfo)
    : SymbolMember(original.Name, original, symbolInfo), ISymbolMemberInfo
{
    #region 配置
    private readonly IPropertySymbol _original = original;
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.Property;
    /// <summary>
    /// 原始属性
    /// </summary>
    public new IPropertySymbol Original => _original;
    /// <inheritdoc />
    public override bool IsPublic 
        => _original.DeclaredAccessibility == Accessibility.Public;
    #endregion

    /// <summary>
    /// 处理属性成员，加入成员参数列表
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="members"></param>
    /// <param name="properties"></param>
    public static void CheckProperties(TypeInfoBuilder typeSymbols, IDictionary<string, IMemberInfo> members, IEnumerable<IPropertySymbol> properties)
    {
        foreach (var property in properties)
        {
            var name = property.Name;
            if (members.ContainsKey(name))
                continue;
            var symbol = typeSymbols.Get(property.Type);
            if (symbol is null)
                continue;
            members.Add(name, new PropertyMember(property, symbol));
        }
    }
}
