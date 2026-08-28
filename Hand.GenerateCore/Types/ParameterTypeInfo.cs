using Hand.Reflection;
using Microsoft.CodeAnalysis;

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
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => this;
}
