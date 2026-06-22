using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class SystemConverterTests
{
    [Fact]
    public void ToInt32()
    {
        var converter = new SystemConverter(SyntaxFactory.IdentifierName(nameof(ToInt32)));
        var source = SyntaxGenerator.Literal("123");
        var dest = converter.Convert(source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("System.Convert.ToInt32(\"123\")", code);
    }
}
