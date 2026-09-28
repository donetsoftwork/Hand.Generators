using Hand.Builders;
using Hand.Members;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Fields;

/// <summary>
/// 字段成员
/// </summary>
/// <param name="name"></param>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
public class FieldMember(string name, IFieldSymbol original, ITypeSymbolInfo symbolInfo)
    : SymbolMember(name, original, symbolInfo), ISymbolMemberInfo
{
    #region 配置
    private static readonly char[] _fieldTrimChars = ['_'];
    private readonly IFieldSymbol _original = original;

    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.Field;
    /// <summary>
    /// 原始字段
    /// </summary>
    public new IFieldSymbol Original => _original;
    /// <inheritdoc />
    public override bool IsPublic 
        => _original.DeclaredAccessibility == Accessibility.Public;
    #endregion

    /// <summary>
    /// 处理字段成员，加入成员参数列表
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="members"></param>
    /// <param name="fields"></param>
    public static void CheckFields(TypeInfoBuilder typeSymbols, IDictionary<string, IMemberInfo> members, IEnumerable<IFieldSymbol> fields)
    {
        foreach (var field in fields)
        {
            // 字段名去掉前缀下划线
            var name = field.Name.TrimStart(_fieldTrimChars);
            if (members.ContainsKey(name))
                continue;
            var symbol = typeSymbols.Get(field.Type);
            if (symbol is null)
                continue;
            members.Add(name, new FieldMember(name, field, symbol));
        }
    }
}
