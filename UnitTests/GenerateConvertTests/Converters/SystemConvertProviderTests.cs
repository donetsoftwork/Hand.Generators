using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class SystemConvertProviderTests
{
    [Theory]
    [InlineData(SpecialType.System_Boolean, "ToBoolean")]
    [InlineData(SpecialType.System_Char, "ToChar")]
    [InlineData(SpecialType.System_SByte, "ToSByte")]
    [InlineData(SpecialType.System_Byte, "ToByte")]
    [InlineData(SpecialType.System_Int16, "ToInt16")]
    [InlineData(SpecialType.System_UInt16, "ToUInt16")]
    [InlineData(SpecialType.System_Int32, "ToInt32")]
    [InlineData(SpecialType.System_UInt32, "ToUInt32")]
    [InlineData(SpecialType.System_Int64, "ToInt64")]
    [InlineData(SpecialType.System_UInt64, "ToUInt64")]
    [InlineData(SpecialType.System_Single, "ToSingle")]
    [InlineData(SpecialType.System_Double, "ToDouble")]
    [InlineData(SpecialType.System_Decimal, "ToDecimal")]
    [InlineData(SpecialType.System_DateTime, "ToDateTime")]
    [InlineData(SpecialType.System_Object, null)]
    [InlineData(SpecialType.System_String, null)]
    public void GetConvertMethodName(SpecialType specialType, string? expected)
    {
        var actual = SystemConvertProvider.GetConvertMethodName(specialType);
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void Create()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference(typeof(Convert).Assembly);
        var compilation = driver.Compile("var i = 0;");
        var provider = SystemConvertProvider.Create(compilation);
        Assert.NotNull(provider);
    }
    [Theory]
    [InlineData(SpecialType.System_Boolean)]
    [InlineData(SpecialType.System_Char)]
    [InlineData(SpecialType.System_SByte)]
    [InlineData(SpecialType.System_Byte)]
    [InlineData(SpecialType.System_Int16)]
    [InlineData(SpecialType.System_UInt16)]
    [InlineData(SpecialType.System_Int32)]
    [InlineData(SpecialType.System_UInt32)]
    [InlineData(SpecialType.System_Int64)]
    [InlineData(SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_Single)]
    [InlineData(SpecialType.System_Double)]
    [InlineData(SpecialType.System_Decimal)]
    [InlineData(SpecialType.System_DateTime)]
    [InlineData(SpecialType.System_String)]
    public void ToInt32(SpecialType specialType)
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference(typeof(Convert).Assembly);
        var compilation = driver.Compile("var i = 0;");
        var provider = SystemConvertProvider.Create(compilation);
        var converter = provider.Get(compilation.GetSpecialType(specialType), compilation.GetIntSymbol());
        Assert.NotNull(converter);
        var source = SyntaxGenerator.Literal("123");
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var dest = converter.Convert(generator, source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("Convert.ToInt32(\"123\")", code);
    }
}
