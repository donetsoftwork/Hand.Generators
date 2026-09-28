using Hand;
using Hand.Attributes;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReflectionTests;

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
        //type2.TypeKind = TypeKind.Error;
        //Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.NonErrorNamedTypeSymbol typeSymbol = type2;
        //Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ErrorTypeSymbol
        //Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ArrayTypeSymbol
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
        Assert.True(type.Equals(type2, SymbolEqualityComparer.Default));

        var targetNode = attribute.Parent?.Parent;
        Assert.NotNull(targetNode);
        var targetSymbol = semanticModel.GetDeclaredSymbol(targetNode);
        Assert.NotNull(targetSymbol);
        var targetSyntaxTree = targetNode.SyntaxTree;
        var attributeData = targetSymbol.GetAttributes()
            .FirstOrDefault();
        Assert.NotNull(attributeData);
        var reference = attributeData.ApplicationSyntaxReference;
        Assert.NotNull(reference);
        var attributeClass = attributeData.AttributeClass;
        Assert.NotNull(attributeClass);
        Assert.Equal(reference.SyntaxTree, targetSyntaxTree);
    }
    [Fact]
    public void HasAttribute()
    {
        var sourceCode = @"
            using System;

            namespace ExampleNamespace;

            [My]
            public class MyClass;
            [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
            public class MyAttribute : Attribute;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var attributeSymbol = compilation.GetTypeByMetadataName("ExampleNamespace.MyAttribute");
        Assert.NotNull(attributeSymbol);
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        Assert.NotNull(classDeclaration);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
        Assert.NotNull(symbol);
        var attributeData = symbol.GetAttributes().FirstOrDefault();
        Assert.NotNull(attributeData);
        Assert.True(attributeSymbol.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default));
    }
    [Fact]
    public void IsGenericType()
    {
        var sourceCode = @"
            using System;

            namespace ExampleNamespace;

            [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
            public class MapAttribute<TFrom> : Attribute
            {
                public Type From { get; } = typeof(TFrom);
            }
            public readonly record struct User(string Name);
            [Map<User>]
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var attributeSymbol = compilation.GetTypeByMetadataName("ExampleNamespace.MapAttribute`1");
        Assert.NotNull(attributeSymbol);
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().LastOrDefault();
        Assert.NotNull(classDeclaration);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
        Assert.NotNull(symbol);
        var attributeData = symbol.GetAttributes().FirstOrDefault();
        Assert.NotNull(attributeData);
        var attributeClass = attributeData.AttributeClass;
        Assert.NotNull(attributeClass);
        Assert.True(attributeClass.IsGenericType(attributeSymbol));
        var attributeTypeArgument = attributeClass.TypeArguments.FirstOrDefault();
        Assert.NotNull(attributeTypeArgument);
    }
    [Fact]
    public void GetAttributesByType()
    {
        var sourceCode = @"
            using System;

            namespace ExampleNamespace;

            [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
            public class MapAttribute(Type from) : Attribute
            {
                public Type From { get; } = from;
            }
            [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
            public class MapAttribute<TFrom> : Attribute
            {
                public Type From { get; } = typeof(TFrom);
            }
            public readonly record struct User(string Name);
            [Map<User>]
            [Map(typeof(User))]
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var attributeSymbol = compilation.GetTypeByMetadataName("ExampleNamespace.MapAttribute");
        Assert.NotNull(attributeSymbol);
        var attributeGenericSymbol = compilation.GetTypeByMetadataName("ExampleNamespace.MapAttribute`1");
        Assert.NotNull(attributeGenericSymbol);
        var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().LastOrDefault();
        Assert.NotNull(classDeclaration);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var symbol = semanticModel.GetDeclaredSymbol(classDeclaration);
        Assert.NotNull(symbol);
        var attributeData = SymbolAttributeHelper.GetAttributesByType(symbol, attributeSymbol)
            .FirstOrDefault();
        Assert.NotNull(attributeData);
        var attributeGenericData = SymbolAttributeHelper.GetAttributesByType(symbol, attributeGenericSymbol)
            .FirstOrDefault();
        Assert.NotNull(attributeGenericData);
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
        var compilation = driver.ScriptCompile(sourceCode);
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
        var driver = SyntaxTreeDriver.CreateDefaultDriver();
        var compilation = driver.Compile(source);
        var productType = compilation.GetTypeByMetadataName("Product");
        Assert.NotNull(productType);
        var properties = SymbolReflection.GetProperties(productType);
        foreach (var property in properties)
        {
            var attributes = property.GetAttributes();
            foreach (AttributeData attribute in attributes)
            {
                var name = attribute.AttributeClass!.Name;
                Assert.NotEmpty(name);
            }
            Assert.True(attributes.Any());
        }
    }
    [Fact]
    public void ToSyntax()
    {
        var sourceCode = @"
            using System;
            namespace ReflectionTests;

            [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class))]
            public class MyAttribute(int value) : Attribute
            {
                public int Value { get; } = value;
            }
            [My(3)]
            public readonly record struct User(string Name);
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var sourceType = compilation.GetTypeByMetadataName("ReflectionTests.User");
        Assert.NotNull(sourceType);
        AttributeData? attribute = sourceType.GetAttributes().FirstOrDefault();
        Assert.NotNull(attribute);
        List<string> namespaces = [];
        AttributeSyntax attributeSyntax = attribute.ToSyntax(namespaces);
        var code = attributeSyntax.ToFullString();
        Assert.Equal("My(3)", code);
        Assert.Single(namespaces);
        Assert.Equal("ReflectionTests", namespaces[0]);
    }
    //[Fact]
    //public void EqualsTest()
    //{
    //    int a = 10;
    //    object objA = a; // 装箱

    //    int b = 10;
    //    object objB = b; // 装箱
    //    // 使用引用比较，结果为false
    //    Assert.False(objA == objB);
    //    // 使用值比较，结果为true
    //    Assert.True(objA.Equals(objB));
    //}
}
