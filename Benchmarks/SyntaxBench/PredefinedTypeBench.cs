using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxBench;

[MemoryDiagnoser, SimpleJob(launchCount: 2, warmupCount: 10, iterationCount: 10, invocationCount: 10000000)]
public class PredefinedTypeBench
{
    [Benchmark(Baseline = true)]
    public PredefinedTypeSyntax PredefinedType()
    {
        return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
    }
    [Benchmark]
    public TypeSyntax ParseTypeName()
    {
        return SyntaxFactory.ParseTypeName("int");
    }
}
