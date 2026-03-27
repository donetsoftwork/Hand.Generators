using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Microsoft.CodeAnalysis.CSharp;

namespace Hello;

public class HelloGenerator()
    : ValuesGenerator<HelloSource>(
    "Hello.HelloGeneratorAttribute",
    new SyntaxFilter(true, SyntaxKind.ClassDeclaration),
    new HelloTransform(),
    new GeneratorExecutor<HelloSource>())
{
}
