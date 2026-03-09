namespace Hand.Mapping;

/// <summary>
/// 转化自标记
/// </summary>
/// <param name="from"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateConvertFromAttribute(Type from)
    : Attribute
{
    #region 配置
    /// <summary>
    /// 来源类型
    /// </summary>
    public Type From { get; } = from;
    #endregion
}
