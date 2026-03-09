using System.ComponentModel;

namespace Hand.Entities;

/// <summary>
/// 生成属性类
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class GeneratePropertyAttribute(string rules = "ALL")
    : Attribute
{
    /// <summary>
    /// 生成规则
    /// </summary>
    public string Rules { get; } = rules;
}
