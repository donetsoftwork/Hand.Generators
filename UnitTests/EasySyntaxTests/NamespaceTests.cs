using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class NamespaceTests
{
    [Theory]
    [InlineData(SpecialType.System_DateTime, "System")]
    [InlineData(SpecialType.System_Collections_IEnumerable, "System.Collections")]
    [InlineData(SpecialType.System_Collections_Generic_IList_T, "System.Collections.Generic")]
    public void ToName(SpecialType specialType, string expected)
    {
        var compilation = CSharpCompilation.Create(null);
        var symbol = compilation.GetSpecialType(specialType);
        var @namespace = symbol.ContainingNamespace;
        var name = @namespace.ToName();
        Assert.Equal(expected, name.ToFullString());
    }
    [Fact]
    public void IsGlobalNamespace()
    {
        var declaration = SyntaxGenerator.RecordDeclaration("User")
            .AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
        var unit = SyntaxFactory.CompilationUnit()
            .AddMembers(declaration);
        var compilation = CSharpCompilation.Create("Tests", [unit.SyntaxTree]);
        var symbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(symbol);
        var @namespace = symbol.ContainingNamespace;
        Assert.True(@namespace.IsGlobalNamespace);
    }
}
