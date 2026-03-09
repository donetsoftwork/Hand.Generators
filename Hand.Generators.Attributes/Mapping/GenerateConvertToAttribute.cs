namespace Hand.Mapping;

/// <summary>
/// 转化为标记
/// </summary>
/// <param name="to"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateConvertToAttribute(Type to)
    : Attribute
{
    #region 配置
    /// <summary>
    /// 目标类型
    /// </summary>
    public Type To { get; } = to;
    #endregion
}