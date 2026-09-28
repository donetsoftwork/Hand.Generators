using Hand;
using Hand.Attributes;
using Hand.Cachers;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;

namespace GeneratePocoTests;

public class AttributeTests
{
    [Fact]
    public void Map()
    {
        var sourceCode = @"
            using System;
            using System.ComponentModel.DataAnnotations;
            using System.ComponentModel.DataAnnotations.Schema;

            namespace ExampleNamespace;

            [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
            public class MapAttribute<TFrom> : Attribute
            {
                public Type From { get; } = typeof(TFrom);
            }
            public readonly record struct User([StringLength(100)]string Name);
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
        var attributeData = SymbolAttributeHelper.GetAttributesByType(symbol, attributeSymbol)
            .FirstOrDefault();
        Assert.NotNull(attributeData);
        var attributeClass = attributeData.AttributeClass;
        Assert.NotNull(attributeClass);
        var sourceSymbol = attributeClass.TypeArguments.FirstOrDefault() as INamedTypeSymbol;
        Assert.NotNull(sourceSymbol);
        var constructor = SymbolReflection.GetConstructors(sourceSymbol)
            .FirstOrDefault();
        Assert.NotNull(constructor);
        var parameterSymbol = constructor.Parameters.FirstOrDefault();
        Assert.NotNull(parameterSymbol);
        var attributeCacher = new AttributeSymbolCacher(compilation);
        var attribute = attributeCacher.CheckByTarget(parameterSymbol.GetAttributes(), AttributeTargets.Property)
            .FirstOrDefault();
        Assert.NotNull(attribute);
        var attributeNamespaces = new List<string>();
        var attributeSyntax = attribute.ToSyntax(attributeNamespaces);
        var attributeNamespace = attributeNamespaces.FirstOrDefault();
        Assert.NotNull(attributeNamespace);
        var propertyType = parameterSymbol.Type.ToSyntax();
        var property = propertyType.GetSetProperty(parameterSymbol.Name)
            .Public()
            .AddAttributeLists(attributeSyntax.ToSingletonList());
        var type = SyntaxFactory.ClassDeclaration(classDeclaration.Identifier)
            .WithModifiers(classDeclaration.Modifiers)
            .AddMembers(property);
        var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(symbol.ContainingNamespace.Name))
            .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(attributeNamespace)))
            .AddMembers(type);
        var code = @namespace.NormalizeWhitespace().ToFullString();
        Assert.Contains("[StringLength(100)]", code);
    }
}
