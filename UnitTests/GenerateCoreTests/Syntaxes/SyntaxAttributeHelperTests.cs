using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests.Syntaxes
{
    public class SyntaxAttributeHelperTests
    {
        private static readonly string _code = @"
namespace Hand.Models
{
    using System;

    public interface IEntityProperty<TProperty>
    {
        TProperty Original { get; }
    }
    [Index(""Include"")]
    public partial struct UserAge(int? original) : IEntityProperty<int?>
    {
        public int? Original { get; } = original;
        [Named(Rules = ""Exclude"")]
        public partial int? GetAge();
    }
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IndexAttribute(string rules) : Attribute
    {
        public string Rules { get; } = rules;
    }
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NamedAttribute : Attribute
    {
        public string? Rules { get; set; }
    }
}
        ";
        [Fact]
        public void TypeIsPartial()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var type = root.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(type);
            Assert.True(type.Modifiers.IsPartial());
        }
        [Fact]
        public void MethodIsPartial()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var method = root.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(method);
            Assert.True(method.Modifiers.IsPartial());
        }
        [Fact]
        public void HasAttribute()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var type = root.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(type);
            Assert.True(SyntaxAttributeHelper.HasAttribute(type, "Index"));
        }
        [Fact]
        public void GetAttributesByType()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var type = root.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(type);
            var attribute = SyntaxAttributeHelper.GetAttributesByType(type, "Index")
                .FirstOrDefault();
            Assert.NotNull(attribute);
        }
        [Fact]
        public void GetArgumentByIndex()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var type = root.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(type);
            var argument = SyntaxAttributeHelper.GetArgument(type, "Index", 0);
            Assert.NotNull(argument);
        }
        [Fact]
        public void GetArgumentByName()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var method = root.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(method);
            var argument = SyntaxAttributeHelper.GetArgument(method, "Named", "Rules");
            Assert.NotNull(argument);
        }
        [Fact]
        public void GetArgumentValueByIndex()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var type = root.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(type);
            SemanticModel semanticModel = CSharpCompilation.Create(null, syntaxTrees: [syntaxTree])
                .GetSemanticModel(syntaxTree, ignoreAccessibility: false);
            var attributeHelper = new SyntaxAttributeHelper(semanticModel);
            var attributeValue = attributeHelper.GetArgumentValue<string>(type, "Index", 0);
            Assert.Equal("Include", attributeValue);
        }
        [Fact]
        public void GetArgumentValueByName()
        {
            // 解析代码为语法树
            var syntaxTree = CSharpSyntaxTree.ParseText(_code);
            var root = syntaxTree.GetRoot();
            var method = root.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();
            Assert.NotNull(method);
            SemanticModel semanticModel = CSharpCompilation.Create(null, syntaxTrees: [syntaxTree])
                .GetSemanticModel(syntaxTree, ignoreAccessibility: false);
            var attributeHelper = new SyntaxAttributeHelper(semanticModel);
            var attributeValue = attributeHelper.GetArgumentValue<string>(method, "Named", "Rules");
            Assert.Equal("Exclude", attributeValue);
        }
    }
}