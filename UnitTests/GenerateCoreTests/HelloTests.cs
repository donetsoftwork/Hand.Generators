using GenerateCoreTests.Hello;
using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests;

public partial class HelloTests
{
    [Fact]
    public void Generate()
    {
        var code0 = @"
namespace GenerateCoreTests.Hello;

[HelloGenerator]
public partial class HelloTests;
";
        var generator = new HelloGenerator();
        var result = SyntaxTreeDriver.DefaultDriver
            .Generate(generator, code0)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("SayHello", code);
    }

    //public static void SayHello(string name)
    //{
    //    Console.WriteLine($"Hello: '{name}'");
    //}
    [Fact]
    public void Source()
    {
        var code0 = @"
namespace GenerateCoreTests.Hello;

[HelloGenerator]
public partial class HelloTests;
";
        var compilation = SyntaxTreeDriver.DefaultDriver
            .Compile(code0);
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var type = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .FirstOrDefault();
        Assert.NotNull(type);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(type);
        Assert.NotNull(symbol);
        HelloSource source = new(type, symbol);
        var builder = source.Generate();
        var code = builder.Build()
            .WithGenerated()
            .ToFullString();
        Assert.Contains("SayHello", code);
    }
}
