using Hand;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests.Symbols
{
    public class SymbolReflectionTests
    {
        static readonly string _code = @"
namespace Hand.Models
{
    public interface IEntityProperty<TProperty>
    {
        TProperty Original { get; }
    }
    public class EntityProperty<TProperty>(TProperty original) : IEntityProperty<TProperty>
    {
        public TProperty Original { get; } = original;
    }
    public class UserId(int id) : EntityProperty<int>(id);
}
        ";

        [Fact]
        public void IsGenericType()
        {
            // 解析代码为语法树
            var syntaxTree = SyntaxTreeScript.Default.Parse(_code);
            var compilation = SyntaxTreeScript.Default.Compile(syntaxTree);
            var definitionType = GetDeclaredTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(definitionType);
            Assert.True(definitionType.IsGenericType);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);  
            // Construct 是 INamedTypeSymbol 的标准泛型构造方法
            var type = definitionType.Construct(intType);
            Assert.NotNull(type);
            Assert.True(SymbolReflection.IsGenericType(type, definitionType));

            var listType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T);
            Assert.NotNull(listType);
            var intListType = listType.Construct(intType);
            Assert.NotNull(intListType);
            Assert.True(SymbolReflection.IsGenericType(intListType, listType));
            Assert.True(SymbolReflection.IsGenericType(intListType, SpecialType.System_Collections_Generic_IList_T));
        }
        [Fact]
        public void IsNullable()
        {
            var syntaxTree = SyntaxTreeScript.Default.Parse("int?");
            var compilation = SyntaxTreeScript.Default.Compile(syntaxTree);
            var definitionType = GetNullableTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(definitionType);
            Assert.True(SymbolReflection.IsNullable(definitionType));
        }
        [Fact]
        public void HasGenericType()
        {
            var syntaxTree = SyntaxTreeScript.Default.Parse(_code);
            var compilation = SyntaxTreeScript.Default.Compile(syntaxTree);
            var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");
            Assert.NotNull(listType);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);
            var intListType = listType.Construct(intType);
            Assert.NotNull(intListType);
            var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
            Assert.True(SymbolReflection.HasGenericType(intListType, enumerableType));
        }
        [Fact]
        public void GetGenericCloseInterfaces()
        {
            var syntaxTree = SyntaxTreeScript.Default.Parse(_code);
            var compilation = SyntaxTreeScript.Default.Compile(syntaxTree);
            var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");
            Assert.NotNull(listType);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);
            var intListType = listType.Construct(intType);
            Assert.NotNull(intListType);
            var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
            var enumerable = SymbolReflection.GetGenericCloseInterfaces(intListType, enumerableType)
                .FirstOrDefault();
            Assert.NotNull(enumerable);
            var collection = SymbolReflection.GetGenericCloseInterfaces(intListType, SpecialType.System_Collections_Generic_ICollection_T)
                .FirstOrDefault();
            Assert.NotNull(collection);
        }


        public static INamedTypeSymbol GetDeclaredTypeSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            //var diagnostics = compilation.GetDiagnostics();
            //foreach (var diagnostic in diagnostics)
            //{
            //    Console.WriteLine(diagnostic.ToString());
            //}
            var semanticModel = compilation.GetSemanticModel(syntaxTree, ignoreAccessibility: false);
            var type = GetSyntax<ClassDeclarationSyntax>(semanticModel); ;
            Assert.NotNull(type);
            var symbol = semanticModel.GetDeclaredSymbol(type);
            Assert.NotNull(symbol);
            return symbol;
        }
        public static INamedTypeSymbol? GetNullableTypeSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            //var diagnostics = compilation.GetDiagnostics();
            //foreach (var diagnostic in diagnostics)
            //{
            //    Console.WriteLine(diagnostic.ToString());
            //}
            var semanticModel = compilation.GetSemanticModel(syntaxTree, ignoreAccessibility: false);
            var type = GetSyntax<NullableTypeSyntax>(semanticModel);
            Assert.NotNull(type);
            var symbol = semanticModel.GetSymbolInfo(type);
            return symbol.Symbol as INamedTypeSymbol;
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
