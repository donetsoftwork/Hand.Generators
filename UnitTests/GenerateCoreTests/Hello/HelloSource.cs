using Hand;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateCoreTests.Hello;

public class HelloSource(ClassDeclarationSyntax type, INamedTypeSymbol symbol)
    : IGeneratorSource
{
    #region 配置
    private readonly ClassDeclarationSyntax _type = type;
    private readonly INamedTypeSymbol _symbol = symbol;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_symbol.ToDisplayString()}.Hello.g.cs";
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var method = GenerateMethod();
        builder.AddMember(method);
        return builder;
    }
    /// <summary>
    /// 生成SayHello方法
    /// </summary>
    /// <returns></returns>
    public static MethodDeclarationSyntax GenerateMethod()
    {
        var name = SyntaxFactory.IdentifierName("name");
        var expression = SyntaxGenerator.Interpolation()
            .Add("Hello: '")
            .Add(name)
            .Add("'")
            .Build();

        return SyntaxGenerator.VoidType.Method("SayHello", SyntaxGenerator.StringType.Parameter(name.Identifier))
            .Public()
            .Static()
            .ToBuilder()
            .Add(SyntaxFactory.IdentifierName("Console").Access("WriteLine").Invocation([expression]))
            .End();
    }
}
