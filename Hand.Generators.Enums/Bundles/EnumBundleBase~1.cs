using Hand.Enums.Fields;
using System.Collections.Generic;

namespace Hand.Enums.Bundles;

/// <summary>
/// 枚举配置基类
/// </summary>
/// <typeparam name="TField"></typeparam>
/// <param name="fields"></param>
public abstract class EnumBundleBase<TField>(List<TField> fields)
    : IEnumBundle
    where TField : EnumField
{
    #region 配置
    /// <summary>
    /// 字段
    /// </summary>
    protected readonly List<TField> _fields = fields;
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
        return _fields.Find(f => f.Match(name));
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
        return _fields.Find(f => f.MatchMember(memberName));
    }
}
