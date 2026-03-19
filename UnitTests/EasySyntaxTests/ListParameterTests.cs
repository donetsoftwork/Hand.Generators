using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class ListParameterTests
{
    [Fact]
    public void Default()
    {
        var recordDeclaration = SyntaxGenerator.RecordDeclaration("Person")
            .WithSemicolonToken();
        var code = recordDeclaration.NormalizeWhitespace().ToFullString();
        Assert.Equal("record Person;", code);
    }
    [Fact]
    public void Empty()
    {
        var recordDeclaration = SyntaxGenerator.RecordDeclaration("Person")
            .WithParameterList(SyntaxFactory.ParameterList())
            .WithSemicolonToken();
        var code = recordDeclaration.NormalizeWhitespace().ToFullString();
        Assert.Equal("record Person();", code);
    }
    [Fact]
    public void Null()
    {
        var recordDeclaration = SyntaxGenerator.RecordDeclaration("Person")
            .WithParameterList(null)
            .WithSemicolonToken();
        var code = recordDeclaration.NormalizeWhitespace().ToFullString();
        Assert.Equal("record Person;", code);
    }
}
