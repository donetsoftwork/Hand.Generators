namespace Hand.Rules;

/// <summary>
/// 默认规则
/// </summary>
public class DefaultRule : IRule
{
    private DefaultRule() { }

    /// <inheritdoc />
    public bool Contains(string name)
        => true;
    /// <summary>
    /// 单例
    /// </summary>

    public static readonly DefaultRule Instance = new();
}
