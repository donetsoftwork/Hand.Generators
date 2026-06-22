using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class CompatibleConverterTests
{
    [Fact]
    public void Convert()
    {
        var compatible = new SystemConverter(SyntaxFactory.IdentifierName("ToInt32"));
        var type = SyntaxFactory.IdentifierName("UserId");
        var original = new ConstructorConverter(type);
        var converter = new CompatibleConverter(compatible, original);
        var source = SyntaxFactory.IdentifierName("value");
        var dest = converter.Convert(source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("new UserId(System.Convert.ToInt32(value))", code);
    }
    //public record UserId(int Original);
}
