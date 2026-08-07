namespace Hand.Members;

/// <summary>
/// 已反转的成员映射(手动反转)
/// </summary>
/// <param name="member"></param>
/// <param name="source"></param>
/// <param name="reversed"></param>
public class MemberReversedArgument(Member member, Member? source, MemberArgument? reversed)
    : MemberArgument(member, source)
{
    #region 配置
    private readonly MemberArgument? _reversed = reversed;
    /// <summary>
    /// 反转参数
    /// </summary>
    public MemberArgument? Reversed
        => _reversed;
    #endregion

    /// <inheritdoc />
    public override MemberArgument? Reverse()
        => _reversed;
}
