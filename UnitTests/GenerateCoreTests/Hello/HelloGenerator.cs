using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateCoreTests.Hello;

public class HelloGenerator()
    : ValuesGenerator<HelloSource>(
    "GenerateCoreTests.Hello.HelloGeneratorAttribute",
    new SyntaxFilter(true, SyntaxKind.ClassDeclaration),
    new HelloTransform(),
    new GeneratorExecutor<HelloSource>())
{
}
