using Hand.Cachers;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System;

namespace Hand.Types;

/// <summary>
/// 实体属性类型(Hand.Models.IEntityProperty)
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
/// <param name="element"></param>
/// <param name="isInterface"></param>
/// <param name="elementInfo"></param>
/// <param name="summary"></param>
public class EntityTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, INamedTypeSymbol element, bool isInterface, PrimitiveTypeInfo elementInfo, Lazy<string> summary)
    : ComplexTypeInfo(original, symbol, isNullable, isInterface, summary), ITypeSymbolInfo
{
    /// <summary>
    /// 实体属性类型(Hand.Models.IEntityProperty)
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <param name="element"></param>
    /// <param name="isInterface"></param>
    public EntityTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, INamedTypeSymbol element, bool isInterface)
        : this(original, symbol, isNullable, element, isInterface, new(element), new(() => SummaryCacher.GetSummary(symbol, symbol.Name)))
    {
    }
    #region 配置
    /// <summary>
    /// 子元素类型
    /// </summary>
    protected readonly INamedTypeSymbol _element = element;
    /// <summary>
    /// 子元素信息
    /// </summary>
    protected readonly PrimitiveTypeInfo _elementInfo = elementInfo;

    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Entity;
    /// <summary>
    /// 子类型
    /// </summary>
    public INamedTypeSymbol Element
        => _element;
    /// <summary>
    /// 子类型信息
    /// </summary>
    public PrimitiveTypeInfo ElementInfo
        => _elementInfo;
    #endregion
    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public new EntityTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((INamedTypeSymbol)compilation.GetNullable(_original), _symbol, true, _element, _isInterface, _elementInfo, _summary);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}