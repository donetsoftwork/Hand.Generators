using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class AttributeTest
{
    [Fact]
    public void Simple()
    {
        var attribute = SyntaxFactory.IdentifierName("Fact").Attribute();
        var code = attribute.ToFullString();
        Assert.Equal("Fact", code);
        var list = attribute.ToSingletonList();
        code = list.ToFullString();
        Assert.Equal("[Fact]", code);
    }
    [Fact]
    public void AddAttributeLists()
    {
        var attribute = SyntaxFactory.IdentifierName("Fact").Attribute();
        var method = SyntaxGenerator.VoidType.Method("Simple")
            .WithBody(SyntaxFactory.Block())
            .AddAttributeLists(attribute.ToSingletonList());
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("[Fact]", code);
    }
    [Fact]
    public void Simple0()
    {
        var attribute = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Fact"));
        var code = attribute.ToFullString();
        Assert.Equal("Fact", code);
    }
    [Fact]
    public void ConstructorArguments()
    {
        var attributeName = SyntaxFactory.IdentifierName("InlineData");
        var argument = SyntaxFactory.AttributeArgument(SyntaxGenerator.Literal(1));
        var attribute = attributeName.Attribute([argument]);
        var code = attribute.ToFullString();
        Assert.Equal("InlineData(1)", code);
        var list = attribute.ToSingletonList();
        code = list.ToFullString();
        Assert.Equal("[InlineData(1)]", code);
    }
    [Fact]
    public void ConstructorArguments0()
    {
        var attributeName = SyntaxFactory.IdentifierName("InlineData");
        var argument = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1));
        var attribute = SyntaxFactory.Attribute(
            attributeName,
            SyntaxFactory.AttributeArgumentList(SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.AttributeArgument(argument))));
        var code = attribute.ToFullString();
        Assert.Equal("InlineData(1)", code);
    }
    [Fact]
    public void NamedArguments()
    {
        var attributeName = SyntaxFactory.IdentifierName("AttributeUsage");
        var targets = SyntaxFactory.IdentifierName("AttributeTargets").
            Access("Method");
        var inherited = SyntaxGenerator.FalseLiteral;
        var attribute = attributeName.Attribute([
            targets.ToAttributeArgument(), 
            inherited.ToAttributeArgument("Inherited")]);
        var code = attribute.NormalizeWhitespace().ToFullString();
        Assert.Equal("AttributeUsage(AttributeTargets.Method, Inherited = false)", code);
        var list = attribute.ToSingletonList();
        code = list.NormalizeWhitespace().ToFullString();
        Assert.Equal("[AttributeUsage(AttributeTargets.Method, Inherited = false)]", code);
    }
    [Fact]
    public void NamedArguments0()
    {
        var attributeName = SyntaxFactory.IdentifierName("AttributeUsage");
        var targets = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, 
            SyntaxFactory.IdentifierName("AttributeTargets"), 
            SyntaxFactory.IdentifierName("Method"));
        var inherited = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression);
        var attribute = SyntaxFactory.Attribute(
            attributeName,
            SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList([
                SyntaxFactory.AttributeArgument(targets),
                SyntaxFactory.AttributeArgument(
                    SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName("Inherited")), 
                    default, 
                    inherited)])));
        var code = attribute.NormalizeWhitespace().ToFullString();
        Assert.Equal("AttributeUsage(AttributeTargets.Method, Inherited = false)", code);
    }
    [Fact]
    public void Attributes()
    {
        var source = @"public class Product
            {
                [Key, Unique]
                public int ProductId { get; set; }
                [Unique]
                [StringLength(100, MinimumLength = 6)]
                public string ProductName { get; set; }
            }";
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var properties = syntaxTree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>();
        foreach (var property in properties)
        {
            var attributeLists = property.AttributeLists;
            foreach (var attributeList in attributeLists)
            {
                var attributes = attributeList.Attributes;
                foreach (AttributeSyntax attribute in attributes)
                {
                    var name = attribute.Name.ToFullString();
                    Assert.NotEmpty(name);
                }
                Assert.True(attributes.Any());
            }
            Assert.True(attributeLists.Any());
        }
    }
}
