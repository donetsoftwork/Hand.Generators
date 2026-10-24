namespace GenerateEnumerationTests.Enumeration2;

public class Enumeration2Tests
{
    [Fact]
    public void Items()
    {
        var provider = CardTypeProvider.Instance;
        CardType[] cardTypes = [.. provider.Items];
        Assert.Equal(4, cardTypes.Length);
    }
    [Fact]
    public void FromName()
    {
        var provider = CardTypeProvider.Instance;
        // 按枚举名获取
        Assert.True(provider.TryGet("Silver", out var silver));
        // 按别名获取
        Assert.True(provider.TryGet("Vip", out var vip));
        Assert.Equal(CardType.Silver, silver);
        Assert.Equal(silver, vip);
    }
    [Fact]
    public void FromName_Null()
    {
        // 获取不存在的枚举时，返回 null
        Assert.False(CardTypeProvider.Instance.TryGet("", out _));
    }
    [Fact]
    public void FromName_Default()
    {
        // 获取不存在的枚举时指定默认值，返回默认值
        var unknownDefault = CardTypeProvider.Instance.Get("", CardType.Silver);
        Assert.Equal(CardType.Silver, unknownDefault);
    }
    [Fact]
    public void FromOriginal()
    {
        // 按枚举值获取
        Assert.True(CardTypeProvider.Instance.TryGet(1, out _));
    }
    [Fact]
    public void FromOriginal_Null()
    {
        // 获取不存在的枚举时，返回 null
        Assert.False(CardTypeProvider.Instance.TryGet(9, out _));
    }
    [Fact]
    public void FromOriginal_Default()
    {
        // 获取不存在的枚举时指定默认值，返回默认值
        var cardType0Default = CardTypeProvider.Instance.Get(0, CardType.Silver);
        Assert.Equal(CardType.Silver, cardType0Default);
    }
    [Fact]
    public void IsDefined()
    {
        var provider = CardTypeProvider.Instance;
        Assert.True(provider.IsDefined(nameof(CardType.Gold)));
        Assert.True(provider.IsDefined(CardType.Silver.Original));
        Assert.True(provider.IsDefined(CardType.Silver));
        Assert.False(provider.IsDefined("SVip"));
    }
}
