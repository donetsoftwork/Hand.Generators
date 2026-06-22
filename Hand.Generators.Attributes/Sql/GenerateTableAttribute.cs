namespace Hand.Sql;

/// <summary>
/// 生成表结构
/// </summary>
/// <param name="from"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class GenerateTableAttribute(Type from)
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
    public string Rule { get; set; }
    #endregion
}
