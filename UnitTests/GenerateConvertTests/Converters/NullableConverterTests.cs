using Hand;
using Hand.Converters;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class NullableConverterTests
{
    [Fact]
    public void Int()
    {
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var converter = new NullableConverter(ToStringConverter.Instance, SyntaxGenerator.Literal("0"));
        var source = SyntaxFactory.IdentifierName("value");
        var dest = converter.Convert(generator, source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("value == null ? \"0\" : value.ToString()", code);
    }
}
