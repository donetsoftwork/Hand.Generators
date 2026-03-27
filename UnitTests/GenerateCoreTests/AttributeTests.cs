using Hand;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests;

public class AttributeTests
{
    [Fact]
    public void Attribute()
    {
        string sourceCode = @"
using System;

namespace ExampleNamespace;

[MyAttribute]
public class MyClass;
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MyAttribute : Attribute;
";
        var compilation = SyntaxTreeScript.Default.Compile(sourceCode);
        var type1 = compilation.GetTypeByMetadataName("MyAttribute");
        Assert.Null(type1);
        var type2 = compilation.GetTypeByMetadataName("ExampleNamespace.MyAttribute");
        Assert.NotNull(type2);
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var attribute = syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault();
        Assert.NotNull(attribute);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetSymbolInfo(attribute)
            .Symbol;
        Assert.NotNull(symbol);
        var type = symbol.ContainingType;
        Assert.NotNull(type);
        Assert.True(SymbolTypeDescriptor.CheckEquals(type, type2));

        var targetNode = attribute.Parent?.Parent;
        Assert.NotNull(targetNode);
        var targetSymbol = semanticModel.GetDeclaredSymbol(targetNode);
        Assert.NotNull(targetSymbol);
        var targetSyntaxTree = targetNode.SyntaxTree;
        foreach (var attributeData in targetSymbol.GetAttributes())
        {
            var reference = attributeData.ApplicationSyntaxReference;
            Assert.NotNull(reference);
            var attributeClass = attributeData.AttributeClass;
            Assert.NotNull(attributeClass);
            Assert.Equal(reference.SyntaxTree, targetSyntaxTree);
        }
    }
}
