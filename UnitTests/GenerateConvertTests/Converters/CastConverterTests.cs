using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;

namespace GenerateConvertTests.Converters;

public class CastConverterTests
{
    [Fact]
    public void Cast()
    {
        var converter = new CastConverter(SyntaxGenerator.ShortType);
        var source = SyntaxGenerator.Literal(1);
        var dest = converter.Convert(source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("(short)1", code);
    }
}
