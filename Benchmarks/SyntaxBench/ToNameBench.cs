using BenchmarkDotNet.Attributes;
using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SyntaxBench;

[MemoryDiagnoser, SimpleJob(launchCount: 2, warmupCount: 10, iterationCount: 10, invocationCount: 10000000)]
public class ToNameBench
{
    private INamespaceSymbol _namespace;
    public ToNameBench()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<object>();
        var compilation = CSharpCompilation.Create(null);
        var type = compilation.GetSpecialType( SpecialType.System_Collections_Generic_IList_T);
        _namespace = type.ContainingNamespace;
    }

    [Benchmark(Baseline = true)]
    public NameSyntax ToName()
    {
        return _namespace.ToName();
    }

    [Benchmark]
    public NameSyntax ParseName()
    {
        return SyntaxFactory.ParseName(_namespace.ToDisplayString());
    }
}
