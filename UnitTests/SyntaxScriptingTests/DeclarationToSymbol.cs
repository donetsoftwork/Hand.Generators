using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxScriptingTests;

public class DeclarationToSymbol
{
    [Fact]
    public void ToSymbol()
    {
        var declaration = SyntaxGenerator.RecordDeclaration("User")
            .AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
        var unit = SyntaxFactory.CompilationUnit()
            .AddMembers(declaration);
        var compilation = CSharpCompilation.Create("Tests", [unit.SyntaxTree]);
        INamedTypeSymbol? userSymbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(userSymbol);
    }

    [Fact]
    public void ToType()
    {
        var declaration = SyntaxGenerator.RecordDeclaration("User")
            .AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
        var unit = SyntaxFactory.CompilationUnit()
            .AddMembers(declaration);
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var compilation = driver.Compile(unit.SyntaxTree);
        INamedTypeSymbol? userSymbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(userSymbol);
        using var context = new ScriptLoadContext();
        var assembly = context.GetAssembly(compilation);
        var type = assembly.GetType("User");
        Assert.NotNull(type);
    }
}
