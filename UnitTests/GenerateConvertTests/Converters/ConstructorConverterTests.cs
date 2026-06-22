using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class ConstructorConverterTests
{
    [Fact]
    public void Convert()
    {
        var type = SyntaxFactory.IdentifierName("UserId");
        var converter = new ConstructorConverter(type);
        var source = SyntaxFactory.IdentifierName("value");
        var dest = converter.Convert(source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("new UserId(value)", code);
    }
    //public record UserId(int Original);
}
