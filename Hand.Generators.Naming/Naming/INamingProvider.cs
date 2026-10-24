using Hand.Members;
using Hand.Methods;

namespace Hand.Naming;

/// <summary>
/// 成员命名器
/// </summary>
public interface INamingProvider : IMethodContainer
{
    /// <summary>
    /// 是否已经命名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);
    /// <summary>
    /// 添加成员类型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="kind"></param>
    void AddKind(string name, MemberKind kind);
    /// <summary>
    /// 尝试获取成员类型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    bool TryGetMemberKind(string name, out MemberKind kind);
}
