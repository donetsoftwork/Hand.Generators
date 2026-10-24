using Hand.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateEnumerationTests.Enum2;

partial class CardEnumeration
{
    private CardEnumeration(string name, long original, string description)
        : base(name, original, description)
    {
    }

    /// <summary>
    /// 银卡
    /// </summary>
    public static readonly CardEnumeration Silver = new(nameof(Silver), 1, "银卡");
    /// <summary>
    /// 金卡
    /// </summary>
    public static readonly CardEnumeration Gold = new(nameof(Gold), 2, "金卡");
    /// <summary>
    /// 铂金卡
    /// </summary>
    public static readonly CardEnumeration Platinum = new(nameof(Platinum), 3, "铂金卡");
    /// <summary>
    /// 黑金卡
    /// </summary>
    public static readonly CardEnumeration Black = new(nameof(Black), 4, "黑金卡");

    /// <inheritdoc cref="Silver" path="/summary"/>
    public static CardEnumeration Vip => Silver;
    /// <inheritdoc cref="Gold" path="/summary"/>
    public static CardEnumeration Vip2 => Gold;
    /// <inheritdoc cref="Platinum" path="/summary"/>
    public static CardEnumeration Vip3 => Platinum;
    /// <inheritdoc cref="Black" path="/summary"/>
    public static CardEnumeration Vip4 => Black;
    /// <inheritdoc cref="Black" path="/summary"/>
    public static CardEnumeration SVIP => Black;

    /// <summary>
    /// Vip枚举提供器
    /// </summary>
    public static readonly IEnumerationProvider<CardEnumeration> Provider = new CardEnumerationProvider();
    /// <summary>
    /// Vip枚举提供器
    /// </summary>
    private class CardEnumerationProvider : EnumerationProvider<CardEnumeration>
    {
        internal CardEnumerationProvider()
            : base(
            [Silver, Gold, Platinum, Black], 
            new Dictionary<string, CardEnumeration>(StringComparer.Ordinal)
            {
                { nameof(Silver), Silver},
                { nameof(Gold), Gold},
                { nameof(Platinum), Platinum},
                { nameof(Black), Black},
                { nameof(Vip) , Silver},
                { nameof(Vip2) , Gold},
                { nameof(Vip3) , Platinum},
                { nameof(Vip4) , Black},
            }, 
            new Dictionary<long, CardEnumeration>()
            {
                { Silver.Original, Silver},
                { Gold.Original, Gold},
                { Platinum.Original, Platinum},
                { Black.Original, Black}
            }
        )
        {
        }
    }
}
