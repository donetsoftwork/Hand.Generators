using Hand.Reflection;
using Microsoft.CodeAnalysis;

namespace Hand.Types;

/// <summary>
/// 类型信息基类
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
public abstract class TypeBaseInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable)
    : INamedSymbolInfo
{
    #region 配置
    /// <summary>
    /// 原始类型
    /// </summary>
    protected readonly INamedTypeSymbol _original = original;
    /// <summary>
    /// 类型
    /// </summary>
    protected readonly INamedTypeSymbol _symbol = symbol;
    /// <summary>
    /// 是否可空
    /// </summary>
    protected readonly bool _isNullable = isNullable;
    /// <inheritdoc />
    public INamedTypeSymbol Original
        => _original;
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
        => TypeSymbolKind.Generic;
    /// <inheritdoc />
    public abstract string Summary { get; }
    #endregion
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => this;
}
