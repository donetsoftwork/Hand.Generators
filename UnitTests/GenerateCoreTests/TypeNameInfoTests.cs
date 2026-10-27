using Hand;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateCoreTests;

public class TypeNameInfoTests
{
    [Fact]
    public void Using()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var type = SyntaxFactory.ClassDeclaration("Comment");
        var generator = SyntaxGenerator.Create(scope, type);
        var id = TypeNameInfo.Guid.Display(generator).GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
        generator.AddProperty(content);
        var addTime = TypeNameInfo.DateTime.Display(generator).GetInitProperty("AddTime").Public();
        generator.AddProperty(addTime);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("using System;", code);
    }
    [Fact]
    public void Same()
    {
        var scope = SyntaxFactory.IdentifierName("Models");
        var userIdType = new TypeNameInfo("UserId", scope);
        var userNameType = new TypeNameInfo("UserName", scope);
        var type = SyntaxFactory.ClassDeclaration("User");
        var generator = SyntaxGenerator.Create(scope, type);
        var id = userIdType.Display(generator).GetInitProperty("Id").Public();
        generator.AddProperty(id);
        var name = userNameType.Display(generator).GetInitProperty("Name").Public();
        generator.AddProperty(name);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.DoesNotContain("using Models;", code);
    }
    [Fact]
    public void Duplicate()
    {
        var customerType = new TypeNameInfo("User", SyntaxFactory.IdentifierName("Customers"));
        var managerType = new TypeNameInfo("User", SyntaxFactory.IdentifierName("Managers"));
        var type = SyntaxFactory.ClassDeclaration("Flow");
        var generator = SyntaxGenerator.Create(SyntaxFactory.IdentifierName("OA"), type);
        var submitter = customerType.Display(generator).GetInitProperty("Submitter").Public();
        generator.AddProperty(submitter);
        var approver = managerType.Display(generator).GetInitProperty("Approver").Public();
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
        var customerId = new TypeNameInfo("UserId", customerScope);
        var customerName = new TypeNameInfo("UserName", customerScope);
        var managerScope = SyntaxFactory.IdentifierName("Managers");
        var managerId = new TypeNameInfo("UserId", managerScope);
        var managerName = new TypeNameInfo("UserName", managerScope);
        var type = SyntaxFactory.ClassDeclaration("Flow");
        var generator = SyntaxGenerator.Create(SyntaxFactory.IdentifierName("OA"), type);
        var submitterId = customerId.Display(generator).GetInitProperty("SubmitterId").Public();
        generator.AddProperty(submitterId);
        var approverName = managerName.Display(generator).GetInitProperty("ApproverName").Public();
        generator.AddProperty(approverName);
        var submitterName = customerName.Display(generator).GetInitProperty("SubmitterId").Public();
        generator.AddProperty(submitterName);
        var approverId = managerId.Display(generator).GetInitProperty("SubmitterId").Public();
        generator.AddProperty(approverId);
        var code = generator.BuildUnit().NormalizeWhitespace().ToFullString();
        Assert.Contains("using Customers;", code);
        Assert.Contains("using Managers;", code);
    }
}
