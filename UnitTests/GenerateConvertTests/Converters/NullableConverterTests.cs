using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class NullableConverterTests
{
    [Fact]
    public void Int()
    {
        var converter = new NullableConverter(ToStringConverter.Instance, SyntaxGenerator.Literal("0"));
        var source = SyntaxFactory.IdentifierName("value");
        var dest = converter.Convert(source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("value == null ? \"0\" : value.ToString()", code);
    }
}
