namespace Hand.Mapping;

/// <summary>
/// 转化为标记
/// </summary>
/// <typeparam name="TTo"></typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateConvertAttribute<TTo>
    : Attribute
{
    #region 配置
    /// <summary>
    /// 目标类型
    /// </summary>
    public Type To { get; } = typeof(TTo);
    /// <summary>
    /// 规则
    /// </summary>
    public string[] Rules { get; set; }
    /// <summary>
    /// 是否生成ConvertFrom方法
    /// </summary>
    public bool ConvertFrom { get; set; }
    #endregion
}