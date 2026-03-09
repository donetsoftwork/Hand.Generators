using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxBench;

[MemoryDiagnoser, SimpleJob(launchCount: 2, warmupCount: 10, iterationCount: 10, invocationCount: 10000000)]
public class QualifiedGenericNameBench
{
    [Benchmark(Baseline = true)]
    public QualifiedNameSyntax QualifiedName()
    {
        return SyntaxFactory.QualifiedName(
            SyntaxFactory.IdentifierName("System.Collections.Generic"), 
            SyntaxFactory.GenericName("List")
                .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
    }
    [Benchmark]
    public TypeSyntax ParseTypeName()
    {
        return SyntaxFactory.ParseTypeName("System.Collections.Generic.List<int>");
    }
    [Benchmark]
    public GenericNameSyntax GenericName()
    {
        return SyntaxFactory.GenericName("System.Collections.Generic.List")
            .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
    }
}
