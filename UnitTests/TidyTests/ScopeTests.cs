using Hand;
using Hand.Tidy;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TidyTests;

public class ScopeTests
{
    [Fact]
    public void Different()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var type = SyntaxFactory.ClassDeclaration("Comment");
        var generator = SyntaxGenerator.Create(scope, type);
        var simplifier = TypeSimplifier.Create(scope);
        var id = simplifier.Simplify(SyntaxGenerator.GuidType).GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
        generator.AddProperty(content);
        var addTime = simplifier.Simplify(SyntaxGenerator.DateTimeType).GetInitProperty("AddTime").Public();
        generator.AddProperty(addTime);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("System.Guid", code);
    }
    [Fact]
    public void Same()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var userIdType = SyntaxFactory.IdentifierName("UserId").Qualify(scope);
        var userNameType = SyntaxFactory.IdentifierName("UserName").Qualify(scope);
        var type = SyntaxFactory.ClassDeclaration("User");
        var generator = SyntaxGenerator.Create(scope, type);
        var simplifier = TypeSimplifier.Create(scope);
        var id = simplifier.Simplify(userIdType).GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var name = simplifier.Simplify(userNameType).GetInitProperty("Name").Public();
        generator.AddProperty(name);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.DoesNotContain("Models.User", code);
    }
    [Fact]
    public void Include()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var userIdType = SyntaxFactory.IdentifierName("UserId").Qualify(scope);
        var scope2 = SyntaxFactory.IdentifierName("CMS").Qualify(scope);
        var type = SyntaxFactory.ClassDeclaration("Comment");
        var generator = SyntaxGenerator.Create(scope2, type);
        var simplifier = TypeSimplifier.Create(scope2);
        var userId = simplifier.Simplify(userIdType).GetInitProperty("UserId").Public();
        generator.AddProperty(userId);
        var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
        generator.AddProperty(content);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.DoesNotContain("Models.UserId", code);
    }
    [Fact]
    public void Part()
    {
        var scope0 = SyntaxFactory.IdentifierName("Models");
        var scope1 = SyntaxFactory.IdentifierName("Accounts").Qualify(scope0);
        var userIdType = SyntaxFactory.IdentifierName("UserId").Qualify(scope1);
        var scope2 = SyntaxFactory.IdentifierName("CMS").Qualify(scope0);
        var type = SyntaxFactory.ClassDeclaration("Comment");
        var generator = SyntaxGenerator.Create(scope2, type);
        var simplifier = TypeSimplifier.Create(scope2);
        var userId = simplifier.Simplify(userIdType).GetInitProperty("UserId").Public();
        generator.AddProperty(userId);
        var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
        generator.AddProperty(content);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.DoesNotContain("Models.Accounts.UserId", code);
        Assert.Contains("Accounts.UserId", code);
    }
}
