using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Enums;

/// <summary>
/// 位域枚举字段
/// </summary>
/// <param name="name"></param>
/// <param name="member"></param>
/// <param name="under"></param>
/// <param name="flag"></param>
public class FlagEnumField(string name, string member/*, MemberAccessExpressionSyntax expression*/, LiteralExpressionSyntax under, ulong flag)
    : EnumField(name, member/*, expression*/, under)
{
    #region 配置
    private readonly ulong _flag = flag;
    /// <summary>
    /// 位域值
    /// </summary>
    public ulong Flag
        => _flag;
    #endregion
    /// <summary>
    /// 按位域获取字段
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static FlagEnumField? GetFieldsByFlag(IEnumerable<FlagEnumField> fields, ulong flag)
    {
        foreach (var field in fields)
        {
            if (field._flag == flag)
                return field;
        }
        return null;
    }
}
