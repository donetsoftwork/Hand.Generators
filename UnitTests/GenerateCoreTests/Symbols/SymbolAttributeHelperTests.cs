using Hand;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests.Symbols
{
    public class SymbolAttributeHelperTests
    {
        static readonly string _code = @"
namespace Hand.Models
{
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
        public void GetAttributesByType()
        {
            // 解析代码为语法树
            var syntaxTree = SyntaxTreeScript.Default.Parse(_code);
            var compilation = SyntaxTreeScript.Default.Compile(syntaxTree);
            var symbol = GetTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(symbol);
            var attributeSymbol = compilation.GetTypeByMetadataName("Hand.Models.IndexAttribute");
            Assert.NotNull(attributeSymbol);
            var attribute = SymbolAttributeHelper.GetAttributesByType(symbol, attributeSymbol).FirstOrDefault();
            Assert.NotNull(attribute);
        }
        [Fact]
        public void GetArgumentValueByIndex()
        {
            // 解析代码为语法树
            var syntaxTree = SyntaxTreeScript.Default.Parse(_code);
            var compilation = SyntaxTreeScript.Default.Compile(syntaxTree);
            var symbol = GetTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(symbol);
            var attributeSymbol = compilation.GetTypeByMetadataName("Hand.Models.IndexAttribute");
            Assert.NotNull(attributeSymbol);
            var attribute = SymbolAttributeHelper.GetAttributesByType(symbol, attributeSymbol).FirstOrDefault();
            Assert.NotNull(attribute);
            var indexValue = SymbolAttributeHelper.GetArgumentValue<string>(attribute, 0);
            Assert.Equal("Include", indexValue);
        }
        [Fact]
        public void GetArgumentValueByName()
        {
            // 解析代码为语法树
            var syntaxTree = SyntaxTreeScript.Default.Parse(_code);
            var compilation = SyntaxTreeScript.Default.Compile(syntaxTree);
            var symbol = GetMethodSymbol(compilation, syntaxTree);
            Assert.NotNull(symbol);
            var attributeSymbol = compilation.GetTypeByMetadataName("Hand.Models.NamedAttribute");
            Assert.NotNull(attributeSymbol);
            var attribute = SymbolAttributeHelper.GetAttributesByType(symbol, attributeSymbol).FirstOrDefault();
            Assert.NotNull(attribute);
            var namedValue = SymbolAttributeHelper.GetArgumentValue<string>(attribute, "Rules");
            Assert.Equal("Exclude", namedValue);
        }

        public static INamedTypeSymbol GetTypeSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            //var diagnostics = compilation.GetDiagnostics();
            //foreach (var diagnostic in diagnostics)
            //{
            //    Console.WriteLine(diagnostic.ToString());
            //}
            var semanticModel = compilation.GetSemanticModel(syntaxTree, ignoreAccessibility: false);
            var type = GetSyntax<StructDeclarationSyntax>(semanticModel); ;
            Assert.NotNull(type);
            var symbol = semanticModel.GetDeclaredSymbol(type);
            Assert.NotNull(symbol);
            return symbol;
        }
        public static IMethodSymbol GetMethodSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree, ignoreAccessibility: false);
            var method = GetSyntax<MethodDeclarationSyntax>(semanticModel);
            var symbol = semanticModel.GetDeclaredSymbol(method);
            Assert.NotNull(symbol);
            return symbol;
        }
        /// <summary>
        /// 获取节点
        /// </summary>
        /// <typeparam name="TSyntax"></typeparam>
        /// <param name="semanticModel"></param>
        /// <returns></returns>
        public static TSyntax GetSyntax<TSyntax>(SemanticModel semanticModel)
            where TSyntax : SyntaxNode
        {
            var nodes = semanticModel.SyntaxTree
                .GetRoot()
                .DescendantNodes();
            var node = nodes.OfType<TSyntax>()
                .FirstOrDefault();
            Assert.NotNull(node);
            return node;
        }
    }
}

