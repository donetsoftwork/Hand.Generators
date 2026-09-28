using Hand;
using Hand.Builders;
using Hand.Converters;
using Hand.Converters.Constructors;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateConvertTests.Converters;

public class ConstructorConverterTests
{
    [Fact]
    public void Convert()
    {
        var driver = SyntaxTreeDriver.CreateDefaultDriver();
        var compilation = driver.Compile("partial record UserId(int Original);");
        Assert.NotNull(compilation);
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var type = syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().FirstOrDefault();
        Assert.NotNull(type);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(type);
        Assert.NotNull(symbol);
        var typeInfos = new TypeInfoBuilder(compilation);
        var typeInfo = typeInfos.Get(symbol);
        var converter = new ConstructorConverter(typeInfo);
        var source = SyntaxFactory.IdentifierName("value");
        var generator = SyntaxGenerator.Create(type);
        var dest = converter.Convert(generator, source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Equal("new UserId(value)", code);
    }
    //public record UserId(int Original);
}
