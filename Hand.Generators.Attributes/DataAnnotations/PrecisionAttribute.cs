namespace Hand.DataAnnotations;

/// <summary>
/// 精度标记
/// </summary>
/// <param name="precision"></param>
/// <param name="scale"></param>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class PrecisionAttribute(int precision, int scale = 0)
    : Attribute
{
    #region 配置
    private readonly int _precision = precision;
    private readonly int _scale = scale;
    /// <summary>
    /// 精度
    /// </summary>
    public int Precision 
        => _precision;
    /// <summary>
    /// 小数规则
    /// </summary>
    public int Scale 
        => _scale;
    #endregion
}
