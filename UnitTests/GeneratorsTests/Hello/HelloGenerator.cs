using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Microsoft.CodeAnalysis.CSharp;

namespace GeneratorsTests.Hello;

public class HelloGenerator()
    : ValuesGenerator<HelloSource>(
    "GeneratorsTests.Hello.HelloGeneratorAttribute",
    new SyntaxFilter(true, SyntaxKind.ClassDeclaration),
    new HelloTransform(),
    new GeneratorExecutor<HelloSource>())
{
}
