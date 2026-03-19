using Hand.Transform;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests.Hello;

public class HelloTransform : IGeneratorTransform<HelloSource>
{
    public HelloSource? Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellation)
    {
        if(cancellation.IsCancellationRequested)
            return null;
        if (context.TargetNode is ClassDeclarationSyntax type && context.TargetSymbol is INamedTypeSymbol symbol)
            return Transform(type, symbol);
        return null;
    }
    /// <summary>
    /// 转化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static HelloSource Transform(ClassDeclarationSyntax type, INamedTypeSymbol symbol)
        => new(type, symbol);
}
