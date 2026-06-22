using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;

namespace GenerateConvertTests.Converters;

public class ToStringConverterTests
{
    [Fact]
    public void Int()
    {
        var converter = ToStringConverter.Instance;
        var source = SyntaxGenerator.Literal(1);
        var dest = converter.Convert(source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("1.ToString()", code);
    }
}
