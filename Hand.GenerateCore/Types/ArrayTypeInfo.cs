using Hand.Reflection;
using Microsoft.CodeAnalysis;

namespace Hand.Types;

/// <summary>
/// 数组类型
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
/// <param name="elementInfo"></param>
public class ArrayTypeInfo(IArrayTypeSymbol original, IArrayTypeSymbol symbol, bool isNullable, ITypeSymbolInfo elementInfo)
    : ICollectionSymbolInfo
{
    #region 配置
    private readonly IArrayTypeSymbol _original = original;
    private readonly IArrayTypeSymbol _symbol = symbol;
    private readonly bool _isNullable = isNullable;
    private readonly ITypeSymbol _element = elementInfo.Original;
    private readonly ITypeSymbolInfo _elementInfo = elementInfo;

    /// <inheritdoc />
    public IArrayTypeSymbol Original
        => _original;
    /// <inheritdoc />
    ITypeSymbol ITypeSymbolInfo.Original
        => Original;
    /// <inheritdoc />
    public IArrayTypeSymbol Symbol
        => _symbol;
    /// <inheritdoc />
    ITypeSymbol ITypeSymbolInfo.Symbol
        => Symbol;
    /// <inheritdoc />
    public bool IsNullable
        => _isNullable;
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Array;
    /// <inheritdoc />
    public string Summary
        => _elementInfo.Summary;
    /// <inheritdoc />
    public ITypeSymbol Element
        => _element;
    /// <inheritdoc />
    public ITypeSymbolInfo ElementInfo
        => _elementInfo;
    #endregion
    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public ArrayTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((IArrayTypeSymbol)compilation.GetNullable(_original), _symbol, true, _elementInfo);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}