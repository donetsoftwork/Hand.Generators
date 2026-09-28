using Hand.Documentation;
using Microsoft.CodeAnalysis;
using System;

namespace Hand.Types;

/// <summary>
/// 枚举类型信息
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="isFlag"></param>
/// <param name="isNullable"></param>
/// <param name="isInterface"></param>
/// <param name="nameInfo"></param>
/// <param name="originalInfo"></param>
/// <param name="summary"></param>
public class EnumerationTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isFlag, bool isNullable, bool isInterface, PrimitiveTypeInfo nameInfo, PrimitiveTypeInfo originalInfo, Lazy<string> summary)
    : ComplexTypeInfo(original, symbol, isNullable, isInterface, summary), ITypeSymbolInfo
{
    /// <summary>
    /// 实体属性类型(Hand.Primitives.IEntityProperty)
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isFlag"></param>
    /// <param name="isNullable"></param>
    /// <param name="isInterface"></param>
    /// <param name="nameType"></param>
    /// <param name="originalType"></param>
    public EnumerationTypeInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isFlag, bool isNullable, bool isInterface, INamedTypeSymbol nameType, INamedTypeSymbol originalType)
        : this(original, symbol, isFlag, isNullable, isInterface, new PrimitiveTypeInfo(nameType, nameType, false), new PrimitiveTypeInfo(originalType, originalType, false), new(() => SummaryCacher.GetSummary(symbol, symbol.Name)))
    {
    }
    #region 配置
    private readonly bool _isFlag = isFlag;
    private readonly PrimitiveTypeInfo _nameInfo = nameInfo;
    private readonly PrimitiveTypeInfo _originalInfo = originalInfo;

    /// <summary>
    /// 是否位标记枚举
    /// </summary>
    public bool IsFlag
        => _isFlag;
    /// <inheritdoc />
    TypeSymbolKind ITypeSymbolInfo.Kind
        => TypeSymbolKind.Enumeration;
    /// <summary>
    /// 枚举名类型(string)
    /// </summary>
    public PrimitiveTypeInfo NameInfo => _nameInfo;
    /// <summary>
    /// 原始值类型(long)
    /// </summary>
    public PrimitiveTypeInfo OriginalInfo => _originalInfo;
    #endregion
    
    /// <summary>
    /// 获取可空枚举类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public new EnumerationTypeInfo GetNullable(Compilation compilation)
        => _isNullable ? this : new((INamedTypeSymbol)compilation.GetNullable(_original), _symbol, _isFlag, true, _isInterface, _nameInfo, _originalInfo, _summary);
    /// <inheritdoc />
    ITypeSymbolInfo ITypeSymbolInfo.GetNullable(Compilation compilation)
        => GetNullable(compilation);

    /// <summary>
    /// 获取枚举类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetEnumerationInterface(Compilation compilation)
        => compilation.GetTypeByMetadataName("Hand.Primitives.IEnumeration");
    /// <summary>
    /// 获取位标记枚举类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetFlagEnumerationInterface(Compilation compilation)
        => compilation.GetTypeByMetadataName("Hand.Primitives.IFlagEnumeration");
}