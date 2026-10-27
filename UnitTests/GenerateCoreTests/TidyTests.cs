using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateCoreTests;

public class TidyTests
{
    [Fact]
    public void Using()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var type = SyntaxFactory.ClassDeclaration("Comment");
        var generator = SyntaxGenerator.Create(scope, type);
        var id = generator.Simplify(SyntaxGenerator.GuidType).GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
        generator.AddProperty(content);
        var addTime = generator.Simplify(SyntaxGenerator.DateTimeType).GetInitProperty("AddTime").Public();
        generator.AddProperty(addTime);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("using System;", code);
    }
    [Fact]
    public void Same()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var userIdType = SyntaxFactory.IdentifierName("UserId").Qualify(scope);
        var userNameType = SyntaxFactory.IdentifierName("UserName").Qualify(scope);
        var type = SyntaxFactory.ClassDeclaration("User");
        var generator = SyntaxGenerator.Create(scope, type);
        var id = generator.Simplify(userIdType).GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var name = generator.Simplify(userNameType).GetInitProperty("Name").Public();
        generator.AddProperty(name);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.DoesNotContain("using Models;", code);
    }
    [Fact]
    public void Duplicate()
    {
        var customerType = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("Customers"));
        var managerType = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("Managers"));
        var type = SyntaxFactory.ClassDeclaration("Flow");
        var generator = SyntaxGenerator.Create(SyntaxFactory.IdentifierName("OA"), type);
        var submitter = generator.Simplify(customerType).GetInitProperty("Submitter").Public();
        generator.AddProperty(submitter);
        var approver = generator.Simplify(managerType).GetInitProperty("Approver").Public();
        generator.AddProperty(approver);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("using Customers;", code);
        Assert.DoesNotContain("using Managers;", code);
        Assert.Contains("Managers.User", code);
    }
    [Fact]
    public void Error()
    {
        var customerScope = SyntaxFactory.IdentifierName("Customers");
        var customerId = SyntaxFactory.IdentifierName("UserId").Qualify(customerScope);
        var customerName = SyntaxFactory.IdentifierName("UserName").Qualify(customerScope);
        var managerScope = SyntaxFactory.IdentifierName("Managers");
        var managerId = SyntaxFactory.IdentifierName("UserId").Qualify(managerScope);
        var managerName = SyntaxFactory.IdentifierName("UserName").Qualify(managerScope);
        var type = SyntaxFactory.ClassDeclaration("Flow");
        var generator = SyntaxGenerator.Create(SyntaxFactory.IdentifierName("OA"), type);
        var submitterId = generator.Simplify(customerId).GetInitProperty("SubmitterId").Public();
        generator.AddProperty(submitterId);
        var approverName = generator.Simplify(managerName).GetInitProperty("ApproverName").Public();
        generator.AddProperty(approverName);
        var submitterName = generator.Simplify(customerName).GetInitProperty("SubmitterId").Public();
        generator.AddProperty(submitterName);
        var approverId = generator.Simplify(managerId).GetInitProperty("SubmitterId").Public();
        generator.AddProperty(approverId);
        // 错误示例,代码可以生成,但生成的代码无法通过编译
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("using Customers;", code);
        Assert.Contains("using Managers;", code);
    }
}
