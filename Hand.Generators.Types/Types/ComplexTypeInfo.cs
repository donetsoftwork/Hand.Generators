using Hand.Documentation;
using Hand.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Types;

/// <summary>
/// 复合类型
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isNullable"></param>
/// <param name="isInterface"></param>
/// <param name="summary"></param>
public class ComplexTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, bool isInterface, Lazy<string> summary)
    : TypeBaseInfo(original, symbol, isNullable), ITypeSymbolInfo, ISyntaxDisplay<TypeSyntax>
{
    /// <summary>
    /// 复合类型
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <param name="isInterface"></param>
    public ComplexTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, bool isInterface)
        : this(original, symbol, isNullable, isInterface, new(() => SummaryCacher.GetSummary(symbol, symbol.Name)))
    {
    }
    #region 配置
    /// <summary>
    /// 是否接口
    /// </summary>
    protected readonly bool _isInterface = isInterface;
    /// <summary>
    /// 备注
    /// </summary>
    protected readonly Lazy<string> _summary = summary;

    /// <summary>
    /// 是否接口
    /// </summary>
    public bool IsInterface
        => _isInterface;
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Complex;
    /// <inheritdoc />
    public override string Summary
        => _summary.Value;
    #endregion

    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public ComplexTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((INamedTypeSymbol)compilation.GetNullable(_original), _symbol, true, _isInterface, _summary);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);
}
