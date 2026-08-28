using Hand;
using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class ToStringConverterTests
{
    [Fact]
    public void Int()
    {
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var converter = ToStringConverter.Instance;
        var source = SyntaxGenerator.Literal(1);
        var dest = converter.Convert(generator, source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("1.ToString()", code);
    }
}
