using Hand.Enumerations;

namespace GenerateEnumerationTests.Enumeration2;

/// <summary>
/// Vip枚举提供器
/// </summary>
public class CardTypeProvider : EnumerationProvider<CardType>
{
    private CardTypeProvider()
        : base(
              new Dictionary<string, CardType>
              {
                    { nameof(CardType.Silver), CardType.Silver},
                    { nameof(CardType.Gold), CardType.Gold},
                    { nameof(CardType.Platinum), CardType.Platinum},
                    { nameof(CardType.Black), CardType.Black},
                    { nameof(CardType.Vip) , CardType.Vip},
                    { nameof(CardType.Vip2) , CardType.Vip2},
                    { nameof(CardType.Vip3) , CardType.Vip3},
                    { nameof(CardType.Vip4) , CardType.Vip4},
              }
    )
    {
    }

    /// <summary>
    /// Vip枚举提供器
    /// </summary>
    public static readonly CardTypeProvider Instance = new();
}
