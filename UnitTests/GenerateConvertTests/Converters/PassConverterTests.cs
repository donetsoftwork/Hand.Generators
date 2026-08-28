using Hand;
using Hand.Builders;
using Hand.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Converters;

public class PassConverterTests
{
    [Fact]
    public void Default()
    {
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var source = SyntaxGenerator.Literal("abc");
        var dest = PassConverter.Default.Convert(generator, source);
        Assert.Equal(source, dest);
    }
    [Fact]
    public void Nullable()
    {
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var @default = SyntaxGenerator.Literal(1);
        var source = SyntaxGenerator.NullLiteral;
        var converter = new PassConverter(@default, true);
        var dest = converter.Convert(generator, source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("null ?? 1", code);
    }
    [Fact]
    public void ClassToBase()
    {
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Using("System");
        var compilation = driver.Compile("");
        var builder = new ConvertBuilder(compilation);
        var type = compilation.GetSpecialType(SpecialType.System_String);
        var baseType = compilation.GetSpecialType(SpecialType.System_Object);
        var converter = builder.Get(type, baseType) as PassConverter;
        Assert.NotNull(converter);
    }
    [Fact]
    public void ClassToInterface()
    {
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Using("System");
        var compilation = driver.Compile("");
        var type = compilation.GetSpecialType(SpecialType.System_String);
        var listType = compilation.GetList(type);
        Assert.NotNull(listType);
        var ilistType = compilation.GetIEnumerable(type);
        Assert.NotNull(ilistType);

        var builder = new ConvertBuilder(compilation);
        var converter = builder.Get(listType, ilistType) as PassConverter;
        Assert.NotNull(converter);
    }
}
