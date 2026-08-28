using Hand.Cachers;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System;

namespace Hand.Types;

/// <summary>
/// 泛型
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="definition"></param>
/// <param name="isNullable"></param>
/// <param name="isInterface"></param>
/// <param name="elements"></param>
/// <param name="summary"></param>
public class GenericTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, INamedTypeSymbol definition, bool isNullable, bool isInterface, ITypeSymbolInfo[] elements, Lazy<string> summary)
    : ComplexTypeInfo(original, symbol, isNullable, isInterface, summary), ITypeSymbolInfo
{
    /// <summary>
    /// 泛型
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="definition"></param>
    /// <param name="isNullable"></param>
    /// <param name="isInterface"></param>
    /// <param name="elements"></param>
    public GenericTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, INamedTypeSymbol definition, bool isNullable, bool isInterface, ITypeSymbolInfo[] elements)
        : this(original, symbol, definition, isNullable, isInterface, elements, new(() => SummaryCacher.GetSummary(symbol, symbol.Name)))
    {
    }
    #region 配置
    private readonly INamedTypeSymbol _definition = definition;
    private readonly ITypeSymbolInfo[] _elements = elements;

    /// <summary>
    /// 泛型定义
    /// </summary>
    public INamedTypeSymbol Definition
        => _definition;
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Generic;
    /// <inheritdoc />
    public ITypeSymbolInfo[] Elements
        => _elements;
    #endregion
    /// <summary>
    /// 可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public new GenericTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((INamedTypeSymbol)compilation.GetNullable(_original), _symbol, _definition, true, _isInterface, _elements, _summary);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}