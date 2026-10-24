using Hand.Enumerations;

namespace GenerateEnumerationTests.FlagEnum1;

public class DaysOfWeekProvider : FlagEnumerationProvider<FlagEnumeration>
{
    private DaysOfWeekProvider()
        : base(
            [Unknown, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday],
            [Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday],
            new Dictionary<string, FlagEnumeration>
            {
                { nameof(Unknown), Unknown },
                { nameof(Sunday), Sunday},
                { nameof(Monday), Monday},
                { nameof(Tuesday), Tuesday},
                { nameof(Wednesday), Wednesday},
                { nameof(Thursday),Thursday},
                { nameof(Friday), Friday},
                { nameof(Saturday), Saturday}
            },
            new Dictionary<long, FlagEnumeration>
            {
                { 0, Unknown },
                { 1, Sunday},
                { 2, Monday},
                { 4, Tuesday},
                { 8, Wednesday},
                { 16, Thursday},
                { 32, Friday},
                { 64, Saturday}
            }
    )
    {
    }

    /// <summary>
    /// 未知
    /// </summary>
    public static readonly FlagEnumeration Unknown = new(nameof(Unknown), 0, "未知");
    /// <summary>
    /// 星期日
    /// </summary>
    public static readonly FlagEnumeration Sunday = new(nameof(Sunday), 1, "星期日");
    /// <summary>
    /// 星期一
    /// </summary>
    public static readonly FlagEnumeration Monday = new(nameof(Monday), 1 << 1, "星期一");
    /// <summary>
    /// 星期二
    /// </summary>
    public static readonly FlagEnumeration Tuesday = new(nameof(Tuesday), 1 << 2, "星期二");
    /// <summary>
    /// 星期三
    /// </summary>
    public static readonly FlagEnumeration Wednesday = new(nameof(Wednesday), 1 << 3, "星期三");
    /// <summary>
    /// 星期四
    /// </summary>
    public static readonly FlagEnumeration Thursday = new(nameof(Thursday), 1 << 4, "星期四");
    /// <summary>
    /// 星期五
    /// </summary>
    public static readonly FlagEnumeration Friday = new(nameof(Friday), 1 << 5, "星期五");
    /// <summary>
    /// 星期六
    /// </summary>
    public static readonly FlagEnumeration Saturday = new(nameof(Saturday), 1 << 6, "星期六");

    /// <inheritdoc />
    public override FlagEnumeration Empty => Unknown;
    /// <inheritdoc />
    protected override FlagEnumeration CreateFlag(ISet<string> flags, long original, string description = "")
        => new(flags, original, description);

    /// <summary>
    /// 单例
    /// </summary>
    public static readonly DaysOfWeekProvider Instance = new();
}

