using Hand.Cachers;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System;

namespace Hand.Types;

/// <summary>
/// 枚举类型信息
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
/// <param name="element"></param>
/// <param name="elementInfo"></param>
/// <param name="summary"></param>
public class EnumTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, INamedTypeSymbol element, PrimitiveTypeInfo elementInfo, Lazy<string> summary)
    : EntityTypeInfo(original, symbol, isNullable, element, false, elementInfo, summary), ITypeSymbolInfo
{
    /// <summary>
    /// 枚举类型信息
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <param name="element"></param>
    public EnumTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, INamedTypeSymbol element)
        : this(original, symbol, isNullable, element, new(element), new(() => SummaryCacher.GetSummary(symbol, symbol.Name)))
    {
    }
    /// <summary>
    /// 枚举类型信息
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    public EnumTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable)
    : this(original, symbol, isNullable, symbol.EnumUnderlyingType!)
    {
    }
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Enum;
    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public new EntityTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((INamedTypeSymbol)compilation.GetNullable(_original), _symbol, true, _element, _elementInfo, _summary);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}
