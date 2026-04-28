using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests;

public class TypeServicesTests
{
    [Theory]
    [InlineData("string")]
    [InlineData("int")]
    [InlineData("int?")]
    public void ToSyntax(string typeName)
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var syntaxTree = driver.Parse(typeName);
        var compilation = driver.Compile(syntaxTree);
        var definitionType = GetNamedTypeSymbol(compilation, syntaxTree);
        Assert.NotNull(definitionType);
        var syntax = definitionType.ToSyntax();
        Assert.NotNull(syntax);
        Assert.Equal(typeName, syntax.ToFullString());
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
