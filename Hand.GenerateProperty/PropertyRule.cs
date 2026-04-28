using Hand.Rule;

namespace Hand.GenerateProperty;

/// <summary>
/// 生成实体属性规则
/// </summary>
/// <param name="original"></param>
public class PropertyRule(IValidation<string> original)
{
    /// <summary>
    /// 生成实体属性规则
    /// </summary>
    /// <param name="rule"></param>
    public PropertyRule(string? rule)
        : this(MemberRuleParser.Default.Parse(rule))
    {
    }
    #region 配置
    #region 常量
    /// <summary>
    /// 构造函数
    /// </summary>
    private const string _constructor = "Constructor";
    /// <summary>
    /// 字段
    /// </summary>
    private const string _field = "Field";
    /// <summary>
    /// ToString
    /// </summary>
    private const string _toString = "ToString";
    /// <summary>
    /// GetHashCode
    /// </summary>
    private const string _hashCode = "HashCode";
    /// <summary>
    /// Equals
    /// </summary>
    private const string _equals = "Equals";
    /// <summary>
    /// 重载运算符
    /// </summary>
    private const string _operator = "Operator";
    #endregion
    private readonly IValidation<string> _original = original;
    /// <summary>
    /// 原始规则
    /// </summary>
    public IValidation<string> Original
        => _original;
    #endregion
    /// <summary>
    /// 构造函数
    /// </summary>
    public bool Constructor
        => _original.Validate(_constructor);
    /// <summary>
    /// 字段
    /// </summary>
    public bool Field
        => _original.Validate(_field);
    /// <summary>
    /// ToString
    /// </summary>
    public bool ToStringMethod
        => _original.Validate(_toString);
    /// <summary>
    /// GetHashCode
    /// </summary>
    public bool GetHashCodeMethod
         => _original.Validate(_hashCode);
    /// <summary>
    /// Equals
    /// </summary>
    public bool EqualsMethod
        => _original.Validate(_equals);
    /// <summary>
    /// 重载运算符
    /// </summary>
    public bool Operator
        => _original.Validate(_operator);
}
