using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxBench;

[MemoryDiagnoser, SimpleJob(launchCount: 2, warmupCount: 10, iterationCount: 10, invocationCount: 100000000)]
public class AccessBench
{
    private readonly IdentifierNameSyntax _user = SyntaxFactory.IdentifierName("user");
    private readonly IdentifierNameSyntax _name = SyntaxFactory.IdentifierName("Name");
    [Benchmark]
    public QualifiedNameSyntax Qualified()
    {
        return SyntaxFactory.QualifiedName(_user, _name);
    }
    [Benchmark(Baseline = true)]
    public MemberAccessExpressionSyntax Access()
    {
        return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, _user, _name);
    }
}
