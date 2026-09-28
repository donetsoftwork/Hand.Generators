using System.ComponentModel;

namespace Hand.Primitives;

/// <summary>
/// 生成属性类
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class GenerateEnumerationAttribute<TFrom>
    : Attribute
{
    #region 配置
    /// <summary>
    /// 规则
    /// </summary>
    public string[] Rules { get; set; }
    #endregion
}
