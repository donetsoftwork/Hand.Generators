using System.ComponentModel;
using System.Runtime.Serialization;

namespace GenerateEnumerationTests.Enum2;

/// <summary>
/// Vip卡类型
/// </summary>
public enum CardType
{
    /// <summary>
    /// 银卡
    /// </summary>
    [Description("银卡")]
    Silver = 1,
    Vip = Silver,
    /// <summary>
    /// 金卡
    /// </summary>
    [Description("金卡")]
    Gold = 2,
    Vip2 = Gold,
    /// <summary>
    /// 白金卡
    /// </summary>
    [Description("白金卡")]
    Platinum = 3,
    Vip3 = Platinum,
    /// <summary>
    /// 黑金卡
    /// </summary>
    [Description("黑金卡")]
    [EnumMember(Value = "SVIP")]
    Black = 4,
    Vip4 = Black,
}