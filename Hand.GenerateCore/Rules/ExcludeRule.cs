using System.Collections.Generic;

namespace Hand.Rules;

/// <summary>
/// 排除规则
/// </summary>
/// <param name="names"></param>
public class ExcludeRule(ISet<string> names)
    : IRule
{
    private readonly ISet<string> _names = names;

    /// <inheritdoc />
    public bool Contains(string name)
        => !_names.Contains(name);
}
