using System.Collections.Generic;

namespace Hand.Rules;

/// <summary>
/// 存在规则
/// </summary>
/// <param name="names"></param>
public class IncludeRule(ISet<string> names)
    : IRule
{
    private readonly ISet<string> _names = names;

    /// <inheritdoc />
    public bool Contains(string name)
        => _names.Contains(name);
}
