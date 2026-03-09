using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxBench;

[MemoryDiagnoser, SimpleJob(launchCount: 2, warmupCount: 10, iterationCount: 10, invocationCount: 10000000)]
public class QualifiedNameBench
{
    [Benchmark(Baseline = true)]
    public QualifiedNameSyntax QualifiedName()
    {
        return SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("Models"), SyntaxFactory.IdentifierName("User"));
    }
    [Benchmark]
    public TypeSyntax ParseTypeName()
    {
        return SyntaxFactory.ParseTypeName("Models.User");
    }
    [Benchmark]
    public IdentifierNameSyntax IdentifierName()
    {
        return SyntaxFactory.IdentifierName("Models.User");
    }
}
