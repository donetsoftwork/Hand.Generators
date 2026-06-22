using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class SyntaxGeneratorTests
{
    [Fact]
    public void CloneClass()
    {
        var @class = SyntaxFactory.ClassDeclaration("UserId")
            .Partial()
            .WithSemicolonToken();
        var code0 = @class.NormalizeWhitespace().ToFullString();
        var generator = SyntaxGenerator.Clone(@class);
        var code = generator.Build().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void CloneRecord()
    {
        var record = SyntaxGenerator.RecordDeclaration("UserId")
            .Partial()
            .WithSemicolonToken();
        var code0 = record.NormalizeWhitespace().ToFullString();
        var generator = SyntaxGenerator.Clone(record);
        var code = generator.Build().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void CloneStructDeclaration()
    {
        var record = SyntaxGenerator.RecordStructDeclaration("UserId")
            .Partial()
            .WithSemicolonToken();
        var code0 = record.NormalizeWhitespace().ToFullString();
        var generator = SyntaxGenerator.Clone(record);
        var code = generator.Build().ToFullString();
        Assert.Equal(code0, code);
    }
    [Fact]
    public void AddParameter()
    {
        var code0 = "partial class UserId;";
        var type = SyntaxFactory.ParseMemberDeclaration(code0) as ClassDeclarationSyntax;
        Assert.NotNull(type);

        var generator = SyntaxGenerator.Clone(type);
        var original = SyntaxFactory.IdentifierName("original");
        generator.Parameter(SyntaxGenerator.IntType, original.Identifier);
        var property = SyntaxGenerator.IntType.GetOnlyProperty("Original")
            .WithInitializer(original);
        generator.AddOther(property);
        var code = generator.Build().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void AddBaseType()
    {
        var code0 = "partial record Vip;";
        var type = SyntaxFactory.ParseMemberDeclaration(code0) as TypeDeclarationSyntax;
        Assert.NotNull(type);

        var generator = SyntaxGenerator.Clone(type);
        var name = SyntaxFactory.IdentifierName("Name");
        var level = SyntaxFactory.IdentifierName("Level");
        generator.Parameter(SyntaxGenerator.StringType, name.Identifier);
        generator.Parameter(SyntaxGenerator.IntType, level.Identifier);
        var baseType = SyntaxGenerator.PrimaryConstructorBaseType("Customer", name);
        generator.AddBaseType(baseType);
        var code = generator.Build().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Collection()
    {        
        var collection = SyntaxGenerator.Collection([SyntaxGenerator.Literal(1), SyntaxGenerator.Literal(2)]);
        var type = SyntaxGenerator.IntType.Array();
        var variable = type.Variable("userIds", collection);
        Assert.NotNull(variable);
        var code = variable.NormalizeWhitespace().ToFullString();
        Assert.Equal("int[] userIds = [1, 2]", code);
    }
    [Fact]
    public void NewWithArguments()
    {
        var type = SyntaxFactory.IdentifierName("UserId");
        var variable = type.Variable("userId", SyntaxGenerator.New([SyntaxGenerator.Literal(1)]));
        Assert.NotNull(variable);
        var code = variable.NormalizeWhitespace().ToFullString();
        Assert.Equal("UserId userId = new(1)", code);
    }
    [Fact]
    public void NewWithInitializer()
    {
        var type = SyntaxFactory.IdentifierName("UserId");
        var original = SyntaxFactory.IdentifierName("Original").Assign(SyntaxGenerator.Literal(1));
        var variable = type.Variable("userId", SyntaxGenerator.New([original]));
        Assert.NotNull(variable);
        var code = variable.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("UserId userId = new()", code);
        Assert.Contains("Original = 1", code);
    }
    [Fact]
    public void NewWithArgumentsAndInitializer()
    {
        var type = SyntaxFactory.IdentifierName("Vip");
        var level = SyntaxFactory.IdentifierName("Level").Assign(SyntaxGenerator.Literal(1));
        var variable = type.Variable("king", SyntaxGenerator.New([SyntaxGenerator.Literal("King")], [level]));
        Assert.NotNull(variable);
        var code = variable.NormalizeWhitespace().ToFullString();
        Assert.StartsWith("Vip king = new(\"King\")", code);
        Assert.Contains("Level = 1", code);
    }

    //partial class UserId(int original)
    //{
    //    int Original { get; } = original;
    //}
    //record Customer(string Name);
    //partial record Vip(string Name, int Level) : Customer(Name);

}
