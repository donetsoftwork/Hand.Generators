namespace Hand.Mapping;

/// <summary>
/// 类型转化标记
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class GenerateConvertAttribute : Attribute
{
}
