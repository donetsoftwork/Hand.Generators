namespace Hand.Cache;

/// <summary>
/// 惰性属性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class GenerateLazyAttribute(string propertyName = "")
    : Attribute
{
    /// <summary>
    /// 指定属性名
    /// </summary>
    public string PropertyName { get; } = propertyName;
}
