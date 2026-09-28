using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 其他类型信息
/// </summary>
/// <param name="original"></param>
public sealed class VoidTypeInfo(ITypeSymbol original)
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
    bool ITypeSymbolInfo.IsNullable
        => false;

    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Void;
    /// <inheritdoc />
    public string Summary
        => string.Empty;
    #endregion

    /// <inheritdoc />
    public TypeSyntax Display(SyntaxGenerator generator)
        => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
    /// <inheritdoc />
    public TypeOfExpressionSyntax TypeOf(SyntaxGenerator generator)
        => SyntaxFactory.TypeOfExpression(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)));
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => this;
}
