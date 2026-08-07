using GenerateConvertTests.Supports;

namespace GenerateConvertTests.Enums;

public class EnumTests
{
    [Fact]
    public void Parse()
    {
        
        var color = Enum.Parse<ConsoleColor>("Red", true);
        Assert.Equal(ConsoleColor.Red, color);
        //color = Enum.Parse<MyColor>("B", true);
    }
    [Fact]
    public void ToObject()
    {
        var color = (MyColor)Enum.ToObject(typeof(MyColor), 1);
        Assert.Equal(MyColor.Red, color);
    }
    [Fact]
    public void FlagsToString()
    {
        var columnType = ColumnType.Identity | ColumnType.Key;
        var str = columnType.ToString();
        var expected = "Identity, Key";
        Assert.Equal(expected, str);
        var parsed = Enum.Parse<ColumnType>(expected, true);
        Assert.Equal(columnType, parsed);
    }
    [Theory]
    [InlineData(ConsoleColor.Red, 12UL)]
    [InlineData(ConsoleColor.Green, 10UL)]
    [InlineData(ConsoleColor.Blue, 9UL)]
    public void EnumToNumber(ConsoleColor color, ulong expected)
    {
        var number = (ulong)color;
        Assert.Equal(expected, number);
    }
    [Theory]
    [InlineData(12UL, ConsoleColor.Red)]
    [InlineData(10UL, ConsoleColor.Green)]
    [InlineData(9UL, ConsoleColor.Blue)]
    public void NumberToEnum(ulong number, ConsoleColor expected)
    {
        var color = (ConsoleColor)number;
        Assert.Equal(expected, color);
    }
    [Theory]
    [InlineData(MyColor.Red, ConsoleColor.DarkBlue)]
    [InlineData(MyColor.Green, ConsoleColor.DarkGreen)]    
    [InlineData(MyColor.Blue, ConsoleColor.DarkRed)]
    public void EnumToEnum(MyColor myColor, ConsoleColor expected)
    {
        // 默认枚举转枚举是按值转的, 与常人直觉不符
        // 代码生成应该避开此逻辑, 应该按枚举名转
        var consoleColor = (ConsoleColor)myColor;
        Assert.Equal(expected, consoleColor);
    }
}

