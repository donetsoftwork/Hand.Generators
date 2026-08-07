namespace Hand.Sql;

/// <summary>
/// 生成表结构
/// </summary>
/// <typeparam name="TFrom"></typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class GenerateTableAttribute<TFrom>
    : Attribute
{
    #region 配置
    /// <summary>
    /// 来源类型
    /// </summary>
    public Type From { get; } = typeof(TFrom);
    /// <summary>
    /// 规则
    /// </summary>
    public string Rule { get; set; }
    #endregion
}
