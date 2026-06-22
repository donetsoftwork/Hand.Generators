using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;

namespace GenerateConvertTests.Converters;

public class PassConverterTests
{
    [Fact]
    public void Default()
    {
        var source = SyntaxGenerator.Literal("abc");
        var dest = PassConverter.Default.Convert(source);
        Assert.Equal(source, dest);
    }
    [Fact]
    public void Nullable()
    {
        var @default = SyntaxGenerator.Literal(1);
        var source = SyntaxGenerator.NullLiteral;
        var converter = new PassConverter(@default, true);
        var dest = converter.Convert(source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("null ?? 1", code);
    }
}
