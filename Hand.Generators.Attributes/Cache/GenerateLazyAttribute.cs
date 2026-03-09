namespace Hand.Cache;

/// <summary>
/// 惰性属性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class GenerateLazyAttribute : Attribute
{
    private string _propertyName;
    /// <summary>
    /// 指定属性名
    /// </summary>
    public string PropertyName 
    { 
        get => _propertyName;
        set => _propertyName = value;
    }
}
