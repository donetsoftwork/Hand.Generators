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
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
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
    [Fact]
    public async Task AttributeData()
    {
        string sourceCode = @"
[MyAttribute2(1)]
public class MyClass2;
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MyAttribute2(int val) : Attribute
{
    public int Val { get; } = val;
}
";
        var driver = SyntaxTreeDriver.ScriptDriver;
        var compilation = driver.ScriptCompile(driver.Parse(sourceCode));
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var attribute = syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>().FirstOrDefault();
        Assert.NotNull(attribute);
        //var expression = attribute.CreateToUnit();
        //Assert.NotNull(expression);
        //var script = driver.CreateScript<object>(expression.SyntaxTree, previous: compilation);
        //var result = await script.ExecuteAsync();
        //Assert.NotNull(result);
    }
    [Fact]
    public void Script()
    {
        var code = @"return new object();";
        var tree = SyntaxFactory.ParseSyntaxTree(code, CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
        Assert.NotNull(tree);
        var expression = SyntaxGenerator.ObjectType.New().ToUnit();
        var tree2 = expression.SyntaxTree;
        tree2 = tree2.WithRootAndOptions(tree2.GetRoot(), CSharpParseOptions.Default.WithKind(SourceCodeKind.Script));
        Assert.NotNull(tree2);
    }
}
