using Hand;
using Hand.Enumerations;
using Hand.GenerateEnumeration;
using Hand.Primitives;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace GenerateEnumerationTests.Enum3;

public class CardTypeTests
{
    [Fact]
    public void Generator()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace GenerateEnumerationTests;

/// <summary>
/// Vip卡类型
/// </summary>
public enum CardType
{
    /// <summary>
    /// 银卡
    /// </summary>
    [Description(""银卡"")]
    Silver = 1,
    Vip = Silver,
    /// <summary>
    /// 金卡
    /// </summary>
    [Description(""金卡"")]
    Gold = 2,
    Vip2 = Gold,
    /// <summary>
    /// 白金卡
    /// </summary>
    [Description(""白金卡"")]
    Platinum = 3,
    Vip3 = Platinum,
    /// <summary>
    /// 黑金卡
    /// </summary>
    [Description(""黑金卡"")]
    [EnumMember(Value = ""SVIP"")]
    Black = 4,
    Vip4 = Black,
}

[GenerateEnumeration<CardType>]
public partial class CardEnumeration;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("CardEnumerationProvider", code);
    }
    [Fact]
    public void Enumeration()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace GenerateEnumerationTests;

/// <summary>
/// Vip卡类型
/// </summary>
public enum CardType
{
    /// <summary>
    /// 银卡
    /// </summary>
    [Description(""银卡"")]
    Silver = 1,
    Vip = Silver,
    /// <summary>
    /// 金卡
    /// </summary>
    [Description(""金卡"")]
    Gold = 2,
    Vip2 = Gold,
    /// <summary>
    /// 白金卡
    /// </summary>
    [Description(""白金卡"")]
    Platinum = 3,
    Vip3 = Platinum,
    /// <summary>
    /// 黑金卡
    /// </summary>
    [Description(""黑金卡"")]
    [EnumMember(Value = ""SVIP"")]
    Black = 4,
    Vip4 = Black,
}

[GenerateEnumeration<CardType>]
public partial class CardEnumeration : Enumeration;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("CardEnumerationProvider", code);
    }
    [Fact]
    public void FlagEnumeration()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace GenerateEnumerationTests;

/// <summary>
/// Vip卡类型
/// </summary>
public enum CardType
{
    /// <summary>
    /// 银卡
    /// </summary>
    [Description(""银卡"")]
    Silver = 1,
    Vip = Silver,
    /// <summary>
    /// 金卡
    /// </summary>
    [Description(""金卡"")]
    Gold = 2,
    Vip2 = Gold,
    /// <summary>
    /// 白金卡
    /// </summary>
    [Description(""白金卡"")]
    Platinum = 3,
    Vip3 = Platinum,
    /// <summary>
    /// 黑金卡
    /// </summary>
    [Description(""黑金卡"")]
    [EnumMember(Value = ""SVIP"")]
    Black = 4,
    Vip4 = Black,
}

[GenerateEnumeration<CardType>]
public partial class CardEnumeration : FlagEnumeration;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("CardEnumerationProvider", code);
    }
    [Fact]
    public void EnumerationProvider()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace GenerateEnumerationTests;

/// <summary>
/// Vip卡类型
/// </summary>
public enum CardType
{
    /// <summary>
    /// 银卡
    /// </summary>
    [Description(""银卡"")]
    Silver = 1,
    Vip = Silver,
    /// <summary>
    /// 金卡
    /// </summary>
    [Description(""金卡"")]
    Gold = 2,
    Vip2 = Gold,
    /// <summary>
    /// 白金卡
    /// </summary>
    [Description(""白金卡"")]
    Platinum = 3,
    Vip3 = Platinum,
    /// <summary>
    /// 黑金卡
    /// </summary>
    [Description(""黑金卡"")]
    [EnumMember(Value = ""SVIP"")]
    Black = 4,
    Vip4 = Black,
}

[GenerateEnumeration<CardType>]
public partial class CardProvider : EnumerationProvider<Enumeration>;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Instance", code);
    }
}

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

[GenerateEnumeration<CardType>]
public partial class CardEnumeration;
