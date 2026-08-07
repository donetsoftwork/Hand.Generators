using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Sources;

/// <summary>
/// 类型转化生成源
/// </summary>
/// <param name="type"></param>
/// <param name="typeSymbol"></param>
/// <param name="methods"></param>
public class ConvertToSource(TypeDeclarationSyntax type, INamedTypeSymbol typeSymbol, ComplexSource[] methods)
    : IGeneratorSource
{
    #region 配置
    /// <summary>
    /// 类型
    /// </summary>
    private readonly TypeDeclarationSyntax _type = type;
    /// <summary>
    /// 类型符号
    /// </summary>
    private readonly INamedTypeSymbol _typeSymbol = typeSymbol;
    private readonly ComplexSource[] _methods = methods;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_typeSymbol.ToDisplayString()}.ConvertTo.g.cs";
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        foreach (var method in _methods)
        {
            builder.AddMethod(method.CreateMethod());
        }
        return builder;
    }
}
