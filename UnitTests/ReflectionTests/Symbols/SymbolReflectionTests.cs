using Hand;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReflectionTests.Symbols
{
    public class SymbolReflectionTests
    {
        static readonly string _code = @"
namespace Hand.Primitives;

public interface IEntityProperty<TProperty>
{
    TProperty Original { get; }
}
public class EntityProperty<TProperty>(TProperty original) : IEntityProperty<TProperty>
{
    TProperty _original = original;
    public TProperty Original => _original;
}
public class UserId(int id) : EntityProperty<int>(id);";

        [Fact]
        public void IsGenericType()
        {
            // 解析代码为语法树
            var syntaxTree = SyntaxTreeDriver.DefaultDriver.Parse(_code);
            var compilation = SyntaxTreeDriver.DefaultDriver.Compile(syntaxTree);
            var definitionType = GetDeclaredTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(definitionType);
            Assert.Equal("EntityProperty`1", definitionType.MetadataName);
            Assert.True(definitionType.IsGenericType);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);  
            // Construct 是 INamedTypeSymbol 的标准泛型构造方法
            var type = definitionType.Construct(intType);
            Assert.NotNull(type);
            Assert.True(type.IsGenericType(definitionType));

            var listType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T);
            Assert.Equal("IList`1", listType.MetadataName);
            var intListType = listType.Construct(intType);
            Assert.NotNull(intListType);
            Assert.True(intListType.IsGenericType(listType));
            Assert.True(intListType.IsGenericType(SpecialType.System_Collections_Generic_IList_T));
        }
        [Fact]
        public void DeclaringSyntaxReferences()
        {
            // 解析代码为语法树
            var syntaxTree = SyntaxTreeDriver.DefaultDriver.Parse(_code);
            var compilation = SyntaxTreeDriver.DefaultDriver.Compile(syntaxTree);
            var definitionType = GetDeclaredTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(definitionType);
            var reference = definitionType.DeclaringSyntaxReferences.FirstOrDefault();
            Assert.NotNull(reference);
            var node = reference.GetSyntax();
            Assert.True(node is TypeDeclarationSyntax);
        }
        [Fact]
        public void IsNullable_true()
        {
            var driver = SyntaxTreeDriver.CreateDriver()
                .Reference<int>();
            var syntaxTree = driver.Parse("int?");
            var compilation = driver.Compile(syntaxTree);
            var definitionType = GetNamedTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(definitionType);
            Assert.True(definitionType.IsNullable());
        }
        [Fact]
        public void IsNullable_false()
        {
            var driver = SyntaxTreeDriver.CreateDriver()
                .Reference<int>();
            var syntaxTree = driver.Parse("int");
            var compilation = driver.Compile(syntaxTree);
            var definitionType = GetNamedTypeSymbol(compilation, syntaxTree);
            Assert.NotNull(definitionType);
            Assert.False(definitionType.IsNullable());
        }
        [Fact]
        public void HasGenericType()
        {
            var syntaxTree = SyntaxTreeDriver.DefaultDriver.Parse(_code);
            var compilation = SyntaxTreeDriver.DefaultDriver.Compile(syntaxTree);
            var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");
            Assert.NotNull(listType);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);
            var intListType = listType.Construct(intType);
            Assert.NotNull(intListType);
            var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
            Assert.True(intListType.HasGenericType(enumerableType));
        }
        [Fact]
        public void GetGenericCloseInterfaces()
        {
            var syntaxTree = SyntaxTreeDriver.DefaultDriver.Parse(_code);
            var compilation = SyntaxTreeDriver.DefaultDriver.Compile(syntaxTree);
            var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");
            Assert.NotNull(listType);
            var intType = compilation.GetSpecialType(SpecialType.System_Int32);
            var intListType = listType.Construct(intType);
            Assert.NotNull(intListType);
            var enumerableType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T);
            var enumerable = intListType.GetGenericCloseInterfaces(enumerableType)
                .FirstOrDefault();
            Assert.NotNull(enumerable);
            var collection = intListType.GetGenericCloseInterfaces(SpecialType.System_Collections_Generic_ICollection_T)
                .FirstOrDefault();
            Assert.NotNull(collection);
        }
        [Fact]
        public void DeclaredAccessibility()
        {
            var compilation = SyntaxTreeDriver.DefaultDriver.Compile(_code);
            var type = compilation.GetSymbol("Hand.Primitives.EntityProperty`1");
            Assert.NotNull(type);
            var _original = type.GetMembers("_original")
                .FirstOrDefault();
            Assert.NotNull(_original);
            Assert.Equal(Accessibility.Private, _original.DeclaredAccessibility);
            var Original = type.GetMembers("Original")
                .FirstOrDefault();
            Assert.NotNull(Original);
            Assert.Equal(Accessibility.Public, Original.DeclaredAccessibility);

            var baseType = type.BaseType;
            Assert.NotNull(baseType);
            Assert.True(baseType.IsObject());
        }
        [Fact]
        public void GetEnumField()
        {
            string sourceCode = @"
using System;

namespace ExampleNamespace;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MyAttribute : Attribute;
";
            var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
            var attributeType = compilation.GetTypeByMetadataName("ExampleNamespace.MyAttribute");
            Assert.NotNull(attributeType);
            var attributeUsage = attributeType.GetAttributes()
                .FirstOrDefault();
            Assert.NotNull(attributeUsage);
            var attributeTargets = attributeUsage.ConstructorArguments
                .FirstOrDefault();
            var enumExpression = attributeTargets.EnumToExpression();
            var enumCode = enumExpression.ToFullString();
            Assert.Equal("AttributeTargets.All", enumCode);

            var attributeTargetsType = compilation.GetTypeByMetadataName("System.AttributeTargets");
            Assert.NotNull(attributeTargetsType);
            var field = SymbolReflection.GetEnumField(attributeTargetsType, (int)AttributeTargets.All);
            Assert.NotNull(field);
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
        public static INamedTypeSymbol? GetNamedTypeSymbol(Compilation compilation, SyntaxTree syntaxTree)
        {
            //var diagnostics = compilation.GetDiagnostics();
            //foreach (var diagnostic in diagnostics)
            //{
            //    Console.WriteLine(diagnostic.ToString());
            //}
            var semanticModel = compilation.GetSemanticModel(syntaxTree, ignoreAccessibility: false);
            var type = GetSyntax<TypeSyntax>(semanticModel);
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
