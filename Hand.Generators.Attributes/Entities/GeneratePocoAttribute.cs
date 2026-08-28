namespace Hand.Entities;

/// <summary>
/// 生成Poco(Plain Old CLR Object)
/// </summary>
/// <typeparam name="TFrom"></typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class GeneratePocoAttribute<TFrom>
    : Attribute
{
    #region 配置
    /// <summary>
    /// 规则
    /// </summary>
    public string[] Rules { get; set; }
    /// <summary>
    /// 可空规则
    /// </summary>
    public string NullableRule { get; set; }
    /// <summary>
    /// 初始化类型
    /// </summary>
    public InitializeKind Initializer { get; set; }
    /// <summary>
    /// 是否生成特性标记
    /// </summary>
    public bool GenerateAttribute { get; set; }
    /// <summary>
    /// 是否生成ConvertTo方法
    /// </summary>
    public bool ConvertTo { get; set; }
    /// <summary>
    /// 是否生成ConvertFrom方法
    /// </summary>
    public bool ConvertFrom { get; set; }
    /// <summary>
    /// 是否生成默认值
    /// </summary>
    public bool Default { get; set; }
    #endregion
}
