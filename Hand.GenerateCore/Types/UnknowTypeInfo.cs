using Hand.Reflection;
using Microsoft.CodeAnalysis;

namespace Hand.Types;

/// <summary>
/// 其他类型信息
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
public class UnknowTypeInfo(ITypeSymbol original, ITypeSymbol symbol, bool isNullable)
    : ITypeSymbolInfo
{
    #region 配置
    private readonly ITypeSymbol _original = original;
    private readonly ITypeSymbol _symbol = symbol;
    private readonly bool _isNullable = isNullable;

    /// <inheritdoc />
    public ITypeSymbol Original
        => _original;
    /// <inheritdoc />
    public ITypeSymbol Symbol
        => _symbol;
    /// <inheritdoc />
    public bool IsNullable
        => _isNullable;

    /// <inheritdoc />
    public TypeSymbolKind Kind
        => TypeSymbolKind.Unknow;
    /// <inheritdoc />
    public string Summary
        => string.Empty;
    #endregion
    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public UnknowTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new(compilation.GetNullable(_original), _symbol, true);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}
