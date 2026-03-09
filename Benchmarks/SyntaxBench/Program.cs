using BenchmarkDotNet.Running;
using SyntaxBench;

//BenchmarkRunner.Run<PredefinedTypeBench>();
//BenchmarkRunner.Run<IdentifierNameBench>();
//BenchmarkRunner.Run<QualifiedNameBench>();
//BenchmarkRunner.Run<GenericNameBench>();
BenchmarkRunner.Run<QualifiedGenericNameBench>(); 
//BenchmarkRunner.Run<AccessBench>();
