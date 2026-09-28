using Hand.Documentation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace Hand.Types;

/// <summary>
/// 枚举类型信息
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isFlag"></param>
/// <param name="isNullable"></param>
/// <param name="element"></param>
/// <param name="elementInfo"></param>
/// <param name="summary"></param>
public class EnumTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isFlag, bool isNullable, INamedTypeSymbol element, PrimitiveTypeInfo elementInfo, Lazy<string> summary)
    : EntityPropertyTypeInfo(original, symbol, isNullable, element, false, elementInfo, summary), ITypeSymbolInfo
{
    /// <summary>
    /// 枚举类型信息
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isFlag"></param>
    /// <param name="isNullable"></param>
    /// <param name="element"></param>
    public EnumTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isFlag, bool isNullable, INamedTypeSymbol element)
        : this(original, symbol, isFlag, isNullable, element, new(element), new(() => SummaryCacher.GetSummary(symbol, symbol.Name)))
    {
    }
    /// <summary>
    /// 枚举类型信息
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isFlag"></param>
    /// <param name="isNullable"></param>
    public EnumTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isFlag, bool isNullable)
    : this(original, symbol, isFlag, isNullable, symbol.EnumUnderlyingType!)
    {
    }
    #region 配置
    private readonly bool _isFlag = isFlag;
    /// <summary>
    /// 是否位标记枚举
    /// </summary>
    public bool IsFlag
        => _isFlag;
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Enum;
    #endregion

    /// <inheritdoc />
    public override TypeOfExpressionSyntax TypeOf(SyntaxGenerator generator)
        => SyntaxFactory.TypeOfExpression(Display(generator));
    /// <summary>
    /// 获取可空类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public new EntityPropertyTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((INamedTypeSymbol)compilation.GetNullable(_original), _symbol, _isFlag, true, _element, _elementInfo, _summary);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);

    /// <summary>
    /// 获取FlagsAttribute特性类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagsAttributeType(Compilation compilation)
        => compilation.GetTypeByMetadataName("System.FlagsAttribute");
}
