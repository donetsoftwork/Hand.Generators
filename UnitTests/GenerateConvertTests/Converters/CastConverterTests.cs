using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class CastConverterTests
{
    [Fact]
    public void Cast()
    {
        var source = SyntaxGenerator.Literal(1);
        var dest = CastConverter.Convert(source, SyntaxGenerator.ShortType);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("(short)1", code);
    }
}
