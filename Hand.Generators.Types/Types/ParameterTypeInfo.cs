using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 其他类型信息
/// </summary>
/// <param name="original"></param>
public class ParameterTypeInfo(ITypeSymbol original)
    : ITypeSymbolInfo
{
    #region 配置
    private readonly ITypeSymbol _original = original;

    /// <inheritdoc />
    public ITypeSymbol Original
        => _original;
    /// <inheritdoc />
    ITypeSymbol ITypeSymbolInfo.Symbol
        => _original;
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Parameter;
    /// <inheritdoc />
    bool ITypeSymbolInfo.IsNullable
        => false;
    /// <inheritdoc />
    public string Summary
        => string.Empty;
    #endregion
    /// <inheritdoc />
    public TypeSyntax Display(SyntaxGenerator generator)
        => SyntaxFactory.OmittedTypeArgument();
    /// <inheritdoc />
    TypeOfExpressionSyntax ITypeSymbolInfo.TypeOf(SyntaxGenerator generator)
        => SyntaxFactory.TypeOfExpression(SyntaxFactory.OmittedTypeArgument());
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => this;
}
