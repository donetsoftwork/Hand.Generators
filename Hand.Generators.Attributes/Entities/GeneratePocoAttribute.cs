namespace Hand.Entities;

/// <summary>
/// 生成Poco(Plain Old CLR Object)
/// </summary>
/// <param name="from"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GeneratePocoAttribute(Type from)
    : Attribute
{
    #region 配置
    /// <summary>
    /// 来源类型
    /// </summary>
    public Type From { get; } = from;
    /// <summary>
    /// 规则
    /// </summary>
    public string[] Rules { get; set; }
    /// <summary>
    /// 可空规则
    /// </summary>
    public string NullableRule { get; set; }
    /// <summary>
    /// 是否使用init访问器
    /// </summary>
    public bool Init { get; set; }
    #endregion
}
