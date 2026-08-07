namespace Hand.Sql;

/// <summary>
/// 唯一键标识
/// </summary>
/// <param name="name"></param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class UniqueAttribute(string name = "")
    : Attribute
{
    /// <summary>
    /// 约束名
    /// </summary>
    public string Name { get; } = name;
}
