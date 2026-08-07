using Hand;

namespace GenerateConvertTests;

public class ClassifyCommonConversionTest
{
    [Fact]
    public void SameType()
    {
        var source = "var i = 0;";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(source);
        var intSymbol = compilation.GetIntSymbol();
        var conversion = compilation.ClassifyCommonConversion(intSymbol, intSymbol);
        Assert.True(conversion.Exists);
        Assert.True(conversion.IsIdentity);
        Assert.True(conversion.IsImplicit);
    }
    [Fact]
    public void Nullable()
    {
        var source = "var i = 0;";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(source);

        var intSymbol = compilation.GetIntSymbol(); 
        var intNullable = compilation.GetNullable(intSymbol);
        var conversion1 = compilation.ClassifyCommonConversion(intSymbol, intNullable);
        Assert.True(conversion1.Exists);
        Assert.True(conversion1.IsImplicit);
        //Assert.True(conversion1.IsNullable);
        var conversion2 = compilation.ClassifyCommonConversion(intNullable, intSymbol);
        Assert.True(conversion2.Exists);
        //Assert.True(conversion2.IsNullable);

        var stringSymbol = compilation.GetStringSymbol();
        var stringNullable = compilation.GetNullable(stringSymbol);
        var conversion3 = compilation.ClassifyCommonConversion(intSymbol, intNullable);
        Assert.True(conversion3.Exists);
        Assert.True(conversion3.IsImplicit);
        //Assert.True(conversion3.IsNullable);
        var conversion4 = compilation.ClassifyCommonConversion(stringNullable, stringSymbol);
        Assert.True(conversion4.Exists);
        //Assert.True(conversion4.IsNullable);
    }
    [Fact]
    public void Implicit()
    {
        var source = "var i = 0;";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(source);
        var longSymbol = compilation.GetLongSymbol();
        var intSymbol = compilation.GetIntSymbol();
        var conversion1 = compilation.ClassifyCommonConversion(intSymbol, longSymbol);
        Assert.True(conversion1.Exists);
        Assert.True(conversion1.IsImplicit);
        Assert.True(conversion1.IsNumeric);
        var conversion2 = compilation.ClassifyCommonConversion(longSymbol, intSymbol);
        Assert.True(conversion2.Exists);
        Assert.True(conversion2.IsNumeric);
    }
    [Fact]
    public void String()
    {
        var source = "var i = 0;";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(source);
        var stringSymbol = compilation.GetStringSymbol();
        var intSymbol = compilation.GetIntSymbol();
        var conversion1 = compilation.ClassifyCommonConversion(intSymbol, stringSymbol);
        Assert.False(conversion1.Exists);
        var conversion2 = compilation.ClassifyCommonConversion(stringSymbol, intSymbol);
        Assert.False(conversion2.Exists);
    }
    [Fact]
    public void Enum()
    {
        var source = @"public enum MyColor
{
    None = 0,
    Red = 1,
    Green = 2,
    Blue = 3,
}";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enum>()
            .Reference<ConsoleColor>();
        var compilation = driver.Compile(source);
        var intSymbol = compilation.GetIntSymbol();
        var enumSymbol = compilation.GetTypeByMetadataName("MyColor");
        Assert.NotNull(enumSymbol);
        var type = compilation.GetTypeByMetadataName("System.ConsoleColor");
        Assert.NotNull(type);
        var conversion1 = compilation.ClassifyCommonConversion(intSymbol, enumSymbol);
        Assert.True(conversion1.Exists);
        var conversion2 = compilation.ClassifyCommonConversion(enumSymbol, intSymbol);
        Assert.True(conversion2.Exists);
    }
    [Fact]
    public void CustomDefine()
    {
        var source = "public record UserId(int Original);";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(source);
        var intSymbol = compilation.GetIntSymbol();
        var userSymbol = compilation.GetTypeByMetadataName("UserId");
        Assert.NotNull(userSymbol);
        var conversion1 = compilation.ClassifyCommonConversion(intSymbol, userSymbol);
        Assert.False(conversion1.Exists);
        var conversion2 = compilation.ClassifyCommonConversion(userSymbol, intSymbol);
        Assert.False(conversion2.Exists);
    }
    [Fact]
    public void CustomDefined()
    {
        var source = @"public record UserId(int Original)
{
    public static implicit operator UserId(int original)
        => new(original);
}";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(source);
        var intSymbol = compilation.GetIntSymbol();
        var userSymbol = compilation.GetTypeByMetadataName("UserId");
        Assert.NotNull(userSymbol);
        var conversion1 = compilation.ClassifyCommonConversion(intSymbol, userSymbol);
        Assert.True(conversion1.Exists);
        Assert.True(conversion1.IsImplicit);
        Assert.True(conversion1.IsUserDefined);
        var conversion2 = compilation.ClassifyCommonConversion(userSymbol, intSymbol);
        Assert.False(conversion2.Exists);
    }
}
//public record UserId(int Original)
//{
//    public static implicit operator UserId(int original)
//        => new(original);
//}

