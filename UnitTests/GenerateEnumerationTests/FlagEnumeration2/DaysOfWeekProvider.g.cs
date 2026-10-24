using Hand.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateEnumerationTests.FlagEnumeration2;

/// <summary>
/// 星期几枚举提供器
/// </summary>
public sealed class DaysOfWeekProvider : FlagEnumerationProvider<DaysOfWeek>
{
    private DaysOfWeekProvider()
        : base(
              new Dictionary<string, DaysOfWeek>
              {
                      { nameof(DaysOfWeek.Unknown), DaysOfWeek.Unknown },
                      { nameof(DaysOfWeek.Sunday), DaysOfWeek.Sunday},
                      { nameof(DaysOfWeek.Monday), DaysOfWeek.Monday},
                      { nameof(DaysOfWeek.Tuesday), DaysOfWeek.Tuesday},
                      { nameof(DaysOfWeek.Wednesday), DaysOfWeek.Wednesday},
                      { nameof(DaysOfWeek.Thursday),DaysOfWeek.Thursday},
                      { nameof(DaysOfWeek.Friday), DaysOfWeek.Friday},
                      { nameof(DaysOfWeek.Saturday), DaysOfWeek.Saturday}
              }
    )
    {
    }
    /// <inheritdoc />
    public override DaysOfWeek Empty => DaysOfWeek.Unknown;
    /// <inheritdoc />
    protected override DaysOfWeek CreateFlag(ISet<string> flags, long original, string description = "")
        => new(flags, original, description);

    /// <summary>
    /// 星期几枚举提供器
    /// </summary>
    public static readonly DaysOfWeekProvider Instance = new();
}
