using Hand.Rules;
using System.Collections.Generic;

namespace Hand.GenerateProperty;

/// <summary>
/// 生成实体属性规则
/// </summary>
/// <param name="rules"></param>
public class PropertyRule(ICollection<string> rules)
{
    /// <summary>
    /// 生成实体属性规则
    /// </summary>
    /// <param name="rules"></param>
    public PropertyRule(string? rules)
        : this(Parser.Parse(rules))
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
    /// <summary>
    /// 解析器
    /// </summary>
    public static readonly RuleParser Parser = new([_constructor, _field, _toString, _hashCode, _equals, _operator]);
    #endregion
    private readonly ICollection<string> _rules = rules;
    /// <summary>
    /// 规则
    /// </summary>
    public ICollection<string> Rules
        => _rules;
    #endregion
    /// <summary>
    /// 构造函数
    /// </summary>
    public bool Constructor
        => _rules.Contains(_constructor);
    /// <summary>
    /// 字段
    /// </summary>
    public bool Field
        => _rules.Contains(_field);
    /// <summary>
    /// ToString
    /// </summary>
    public bool ToStringMethod
        => _rules.Contains(_toString);
    /// <summary>
    /// GetHashCode
    /// </summary>
    public bool GetHashCodeMethod
         => _rules.Contains(_hashCode);
    /// <summary>
    /// Equals
    /// </summary>
    public bool EqualsMethod
        => _rules.Contains(_equals);
    /// <summary>
    /// 重载运算符
    /// </summary>
    public bool Operator
        => _rules.Contains(_operator);
}
