namespace Hand.Rules;

/// <summary>
/// 空规则
/// </summary>
public class EmptyRule : IRule
{
    private EmptyRule() { }

    /// <inheritdoc />
    public bool Contains(string name)
        => false;
    /// <summary>
    /// 单例
    /// </summary>

    public static readonly EmptyRule Instance = new();
}
