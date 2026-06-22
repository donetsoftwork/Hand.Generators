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
    [Fact]
    public void AddParameterListParameters()
    {
        var recordDeclaration = SyntaxGenerator.RecordDeclaration("Customer")
            .AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"))
            .WithSemicolonToken();
        var code = recordDeclaration.NormalizeWhitespace().ToFullString();
        Assert.Equal("record Customer(string Name);", code);
    }
    [Fact]
    public void WithInitializer()
    {
        var constructor = SyntaxFactory.ConstructorDeclaration("Vip")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("name")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("level")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
            .WithInitializer(SyntaxFactory.ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("name"))))))
            .WithBody(SyntaxFactory.Block());
        var vipType = SyntaxFactory.ClassDeclaration("Vip")
            .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("Customer")))
            .AddMembers(constructor);
        var code = vipType.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void WithInitializer2()
    {
        var vipType = SyntaxFactory.ClassDeclaration("Vip")
            .AddBaseTypes("Customer");
        var constructor = vipType.Constructor(SyntaxGenerator.StringType.Parameter("name"), SyntaxGenerator.IntType.Parameter("level"))
            .WithBaseInitializer(SyntaxFactory.IdentifierName("name"))
            .WithBody(SyntaxFactory.Block());
        vipType = vipType.AddMembers(constructor);
        var code = vipType.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void PrimaryConstructorBaseType()
    {
        var baseType = SyntaxFactory.PrimaryConstructorBaseType(
            SyntaxFactory.IdentifierName("Customer"), 
            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("Name")))));
        var recordDeclaration = SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), "Vip")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("Name")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))),
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("Level")).WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
            .AddBaseListTypes(baseType)
            .WithSemicolonToken();
        var code = recordDeclaration.NormalizeWhitespace().ToFullString();
        Assert.Equal("record Vip(string Name, int Level) : Customer(Name);", code);
    }
    [Fact]
    public void PrimaryConstructorBaseType2()
    {
        var recordDeclaration = SyntaxGenerator.RecordDeclaration("Vip")            
            .AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"), SyntaxGenerator.IntType.Parameter("Level"))
            .AddPrimaryConstructorBaseType("Customer", SyntaxFactory.IdentifierName("Name"))
            .WithSemicolonToken();
        var code = recordDeclaration.NormalizeWhitespace().ToFullString();
        Assert.Equal("record Vip(string Name, int Level) : Customer(Name);", code);
    }
}
