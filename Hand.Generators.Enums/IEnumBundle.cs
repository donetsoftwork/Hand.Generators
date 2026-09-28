using System.Collections.Generic;

namespace Hand.Enums;

/// <summary>
/// 枚举配置
/// </summary>
public interface IEnumBundle
{
    /// <summary>
    /// 字段
    /// </summary>
    IEnumerable<IEnumField> Fields { get; }
    /// <summary>
    /// 位域标记
    /// </summary>
    bool HasFlag { get; }
    /// <summary>
    /// 按名获取字段
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEnumField? GetFieldByName(string name);
    /// <summary>
    /// 按成员名获取字段
    /// </summary>
    /// <param name="memberName"></param>
    /// <returns></returns>
    IEnumField? GetFieldByMemberName(string memberName);
}
