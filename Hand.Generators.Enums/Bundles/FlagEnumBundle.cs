using Hand.Enums.Fields;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Enums.Bundles;

/// <summary>
/// 位域枚举配置
/// </summary>
/// <param name="fields"></param>
public class FlagEnumBundle(List<FlagEnumField> fields)
    : EnumBundleBase<FlagEnumField>(fields)
{
    /// <inheritdoc />
    override public bool HasFlag
        => true;
    /// <summary>
    /// 按位域获取字段
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public FlagEnumField? GetFieldsByFlag(ulong flag)
        => FlagEnumField.GetFieldsByFlag(_fields, flag);
    /// <summary>
    /// 按位域获取字段
    /// </summary>
    /// <param name="name"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public IEnumerable<FlagEnumField> GetFieldsByName(string name, string member)
    {
        if (string.IsNullOrEmpty(member))
            return _fields.Where(field => field.Match(name) || field.MatchMember(name));
        return _fields.Where(field => field.Match(name) || field.MatchMember(name) || field.Match(member) || field.MatchMember(member));
    }
}
