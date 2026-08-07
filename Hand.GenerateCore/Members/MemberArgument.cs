using System.Collections.Generic;

namespace Hand.Members;

/// <summary>
/// 成员映射
/// </summary>
/// <param name="member"></param>
/// <param name="source"></param>
public class MemberArgument(Member member, Member? source = null)
{
    #region 配置
    /// <summary>
    /// 成员
    /// </summary>
    public Member Member { get; } = member;
    private Member? _source = source;
    /// <summary>
    /// 实参(成员映射来源)
    /// </summary>
    public Member? Source
    {
        get => _source;
        set => _source = value;
    }
    #endregion
    /// <summary>
    /// 反转
    /// </summary>
    public virtual MemberArgument? Reverse()
    {
        if(_source is null)
            return null;
        return new(_source, Member);
    }
    /// <summary>
    /// 反转
    /// </summary>
    /// <param name="generateArguments"></param>
    /// <returns></returns>
    public static List<MemberArgument> Reverse(List<MemberArgument> generateArguments)
    {
        var count = generateArguments.Count;
        var arguments = new List<MemberArgument>(count);
        foreach (var generated in generateArguments)
        {
            var reversed = generated.Reverse();
            if (reversed is null)
                continue;
            arguments.Add(reversed);
        }
        return arguments;
    }
}
