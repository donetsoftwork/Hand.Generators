using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxBench;

[MemoryDiagnoser, SimpleJob(launchCount: 2, warmupCount: 10, iterationCount: 10, invocationCount: 10000000)]
public class GenericNameBench
{
    [Benchmark(Baseline = true)]
    public GenericNameSyntax GenericName()
    {
        return SyntaxFactory.GenericName("List")
            .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
    }
    [Benchmark]
    public TypeSyntax ParseTypeName()
    {
        return SyntaxFactory.ParseTypeName("List<int>");
    }
}
