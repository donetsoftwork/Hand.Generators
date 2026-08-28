using BenchmarkDotNet.Attributes;
using Hand;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxBench;

[MemoryDiagnoser, SimpleJob(launchCount: 2, warmupCount: 10, iterationCount: 10, invocationCount: 10000000)]
public class QualifiedGenericNameBench
{
    [Benchmark(Baseline = true)]
    public NameSyntax Qualifies()
    {
        //return SyntaxFactory.QualifiedName(
        //    SyntaxFactory.IdentifierName("System.Collections.Generic"), 
        //    SyntaxFactory.GenericName("List")
        //        .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
        return SyntaxGenerator.Generic("List", SyntaxGenerator.IntType).Qualifies("System", "Collections", "Generic");
    }
    [Benchmark]
    public NameSyntax Qualify()
    {
        //return SyntaxFactory.QualifiedName(
        //    SyntaxFactory.IdentifierName("System.Collections.Generic"), 
        //    SyntaxFactory.GenericName("List")
        //        .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
        return SyntaxGenerator.Generic("List", SyntaxGenerator.IntType).Qualify("System.Collections.Generic");
    }
    [Benchmark]
    public TypeSyntax ParseTypeName()
    {
        return SyntaxFactory.ParseTypeName("System.Collections.Generic.List<int>");
    }
    [Benchmark]
    public GenericNameSyntax Generic()
    {
        //return SyntaxFactory.GenericName("System.Collections.Generic.List")
        //    .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
        return SyntaxGenerator.Generic("System.Collections.Generic.List", SyntaxGenerator.IntType);
    }
}
