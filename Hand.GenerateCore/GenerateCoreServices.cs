using Hand.Members;

namespace Hand;

/// <summary>
/// 扩展方法
/// </summary>
public static class GenerateCoreServices
{
    /// <summary>
    /// 是否参数类型
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static bool IsParameter(this MemberKind kind)
        => kind >= MemberKind.Parameter;
    /// <summary>
    /// 是否参数类型
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    public static bool IsParameter(this IMemberInfo member)
        => member.Kind >= MemberKind.Parameter;
}
