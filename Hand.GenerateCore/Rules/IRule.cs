namespace Hand.Rules;

/// <summary>
/// 规则接口
/// </summary>
public interface IRule
{
    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);
}
