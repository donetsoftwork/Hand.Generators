using Hand.Reflection;
using Microsoft.CodeAnalysis;

namespace Hand.Types;

/// <summary>
/// 基础类型
/// </summary>
/// <param name="specialType"></param>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
public class PrimitiveTypeInfo(SpecialType specialType, INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable)
    : TypeBaseInfo(original, symbol, isNullable), ITypeSymbolInfo
{
    /// <summary>
    /// 基础类型
    /// </summary>
    /// <param name="symbol"></param>
    public PrimitiveTypeInfo(INamedTypeSymbol symbol)
        : this(symbol.SpecialType, symbol, symbol, false)
    {
    }
    #region 配置
    private readonly SpecialType _specialType = specialType;

    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Primitive;
    /// <summary>
    /// 特殊类型
    /// </summary>
    public SpecialType SpecialType
        => _specialType;
    /// <inheritdoc />
    public override string Summary
        => string.Empty;
    #endregion
    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public PrimitiveTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new(_specialType, (INamedTypeSymbol)compilation.GetNullable(_original), _symbol, true);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}
