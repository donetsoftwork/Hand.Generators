using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static System.Formats.Asn1.AsnWriter;

namespace EasySyntaxTests;

public class ScopeTests
{
    [Fact]
    public void Class()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var type = SyntaxFactory.ClassDeclaration("User");
        var generator = SyntaxGenerator.Create(scope, type);
        var id = SyntaxGenerator.IntType.GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var name = SyntaxGenerator.StringType.GetInitProperty("Name").Public();
        generator.AddProperty(name);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    [Fact]
    public void Multiple()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var type = SyntaxFactory.ClassDeclaration("User");
        var generator = SyntaxGenerator.Create(scope, type);
        var userIdType = SyntaxFactory.IdentifierName("UserId");
        var userIdGenerator = SyntaxGenerator.Create(generator.Container, SyntaxFactory.ClassDeclaration(userIdType.Identifier));
        var userIdValue = SyntaxGenerator.IntType.GetInitProperty("Value").Public();
        userIdGenerator.AddProperty(userIdValue);
        var userNameType = SyntaxFactory.IdentifierName("UserName");
        var userNameGenerator = SyntaxGenerator.Create(generator.Container, SyntaxFactory.ClassDeclaration(userNameType.Identifier));
        var userNameValue = SyntaxGenerator.StringType.GetInitProperty("Value").Public();
        userNameGenerator.AddProperty(userNameValue);
        var id = userIdType.GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var name = userNameType.GetInitProperty("Name").Public();
        generator.AddProperty(name);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    [Fact]
    public void Nested()
    {
        var outerClass = SyntaxFactory.ClassDeclaration("OuterClass");
        var generator = SyntaxGenerator.Create(outerClass);
        var outerValue = SyntaxGenerator.IntType.GetInitProperty("OuterValue").Public();
        generator.AddProperty(outerValue);
        var innerClass = SyntaxFactory.ClassDeclaration("InnerClass");
        var innerGenerator = SyntaxGenerator.Create(generator, innerClass);
        var innerValue = SyntaxGenerator.IntType.GetInitProperty("InnerValue").Public();
        innerGenerator.AddProperty(innerValue);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("OuterValue", code);
        Assert.Contains("InnerValue", code);
    }
}
