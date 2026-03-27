using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EasySyntaxTests;

public class ParseTests
{
    [Fact]
    public void Attribute()
    {
        string sourceCode = @"
using System;

namespace ExampleNamespace
{
    [MyAttribute]
    public class MyClass;
}
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MyAttribute : Attribute;
";

        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        Assert.NotNull(syntaxTree);
        var attribute = syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault();
        Assert.NotNull(attribute);
        //var compilation = SyntaxTreeScript
    }


}
