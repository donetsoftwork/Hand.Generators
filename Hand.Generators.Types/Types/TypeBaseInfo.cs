using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Types;

/// <summary>
/// 类型信息基类
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
public abstract class TypeBaseInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable)
    : TypeSymbolInfo(original), INamedSymbolInfo
{
    #region 配置
    /// <summary>
    /// 类型
    /// </summary>
    protected readonly INamedTypeSymbol _symbol = symbol;
    /// <summary>
    /// 是否可空
    /// </summary>
    protected readonly bool _isNullable = isNullable;
    /// <inheritdoc />
    ITypeSymbol ITypeSymbolInfo.Original
        => Original;
    /// <inheritdoc />
    public INamedTypeSymbol Symbol 
        => _symbol;
    /// <inheritdoc />
    ITypeSymbol ITypeSymbolInfo.Symbol
        => Symbol;
    /// <inheritdoc />
    public bool IsNullable 
        => _isNullable;
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Unknow;
    /// <inheritdoc />
    public abstract string Summary { get; }
    #endregion
    /// <inheritdoc />
    public override TypeSyntax Display(SyntaxGenerator generator)
        => generator.Display(_symbol, _isNullable);
    /// <inheritdoc />
    public virtual TypeOfExpressionSyntax TypeOf(SyntaxGenerator generator)
        => SyntaxFactory.TypeOfExpression(_symbol.IsValueType ? Display(generator) : generator.Display(_symbol));

    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => this;
}
