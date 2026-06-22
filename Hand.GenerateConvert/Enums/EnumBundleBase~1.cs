using Hand.Enums;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Enums;

/// <summary>
/// 枚举配置基类
/// </summary>
/// <typeparam name="TField"></typeparam>
/// <param name="enumType"></param>
/// <param name="underType"></param>
/// <param name="capacity"></param>
public abstract class EnumBundleBase<TField>(INamedTypeSymbol enumType, INamedTypeSymbol underType, int capacity)
    : IEnumBundle
    where TField : EnumField
{
    #region 配置
    private readonly INamedTypeSymbol _enumType = enumType;
    private readonly INamedTypeSymbol _underType = underType;
    /// <summary>
    /// 字段
    /// </summary>
    protected readonly List<TField> _fields = new(capacity);
    /// <inheritdoc />
    public INamedTypeSymbol EnumType
        => _enumType;
    /// <inheritdoc />
    public INamedTypeSymbol UnderType
        => _underType;
    /// <summary>
    /// 字段
    /// </summary>
    public List<TField> Fields
        => _fields;
    /// <inheritdoc />
    public virtual bool HasFlag
        => false;
    IEnumerable<IEnumField> IEnumBundle.Fields
        => _fields;
    #endregion
    /// <summary>
    /// 添加字段
    /// </summary>
    /// <param name="field"></param>
    public void AddField(TField field)
        => _fields.Add(field);
    /// <summary>
    /// 按名获取字段
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IEnumField? GetFieldByName(string name)
    {
        if(string.IsNullOrEmpty(name))
            return null;
        return _fields.FirstOrDefault(f => f.Match(name));
    }
    /// <summary>
    /// 按成员名获取字段
    /// </summary>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public IEnumField? GetFieldByMemberName(string memberName)
    {
        if (string.IsNullOrEmpty(memberName))
            return null;
        return _fields.FirstOrDefault(f => f.MatchMember(memberName));
    }
}
