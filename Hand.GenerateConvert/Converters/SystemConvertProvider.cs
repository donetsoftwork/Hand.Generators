using Hand.Cache;
using Hand.Members;
using Hand.Reflection;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Converters;

/// <summary>
/// 系统转化器，提供System.Convert类中对应SpecialType的转换方法名称查询功能
/// </summary>
/// <param name="convertType">System.Convert类的类型符号</param>
public class SystemConvertProvider(INamedTypeSymbol convertType)
    : CacheFactoryBase<PairTypeSymbolKey, SystemConverter?>()
{
    #region 配置
    private readonly List<IMethodSymbol> _methods = [.. SymbolReflection.GetMethods(convertType).Where(m => m.DeclaredAccessibility == Accessibility.Public && m.IsStatic)];
    private readonly Dictionary<PairTypeSymbolKey, string?> _types = [];
    /// <summary>
    /// 支持的来源类型
    /// </summary>
    private static readonly HashSet<SpecialType> _supportedTypes =
    [
        SpecialType.System_Boolean,
        SpecialType.System_Char,
        SpecialType.System_SByte,
        SpecialType.System_Byte,
        SpecialType.System_Int16,
        SpecialType.System_UInt16,
        SpecialType.System_Int32,
        SpecialType.System_UInt32,
        SpecialType.System_Int64,
        SpecialType.System_UInt64,
        SpecialType.System_Single,
        SpecialType.System_Double,
        SpecialType.System_Decimal,
        SpecialType.System_DateTime,
        SpecialType.System_String
    ];
    #endregion
    /// <summary>
    /// 构造系统转化器
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static SystemConvertProvider Create(Compilation compilation)
    {
        var convertType = compilation.GetTypeByMetadataName("System.Convert")
            ?? throw new InvalidOperationException("无法找到System.Convert类型");
        return new SystemConvertProvider(convertType);
    }
    /// <summary>
    /// 获取转化方法名称
    /// </summary>
    /// <param name="from">来源类型</param>
    /// <param name="to">目标类型</param>
    /// <returns>对应的转换方法名称，如果不支持则返回null</returns>
    public string? GetConvertMethodName(INamedTypeSymbol from, INamedTypeSymbol to)
    {
        var key = new PairTypeSymbolKey(from, to);
        if (_types.TryGetValue(key, out var convertMethodName))
            return convertMethodName;
        convertMethodName = GetConvertMethodName(to.SpecialType);
        if (string.IsNullOrEmpty(convertMethodName))
            return null;
        if (!_supportedTypes.Contains(from.SpecialType))
            return null;

        lock (_types)
        {
            if (_types.TryGetValue(key, out var methodName))
                return methodName;
            if (SymbolTypeDescriptor.GetSingleParameterMethod(_methods, convertMethodName!, from) is null)
                return _types[key] = null;
            return _types[key] = convertMethodName;
        }
    }
    /// <summary>
    /// 获取System.Convert类中对应SpecialType的转换方法名称
    /// </summary>
    /// <param name="specialType">要转换的SpecialType</param>
    /// <returns>对应的转换方法名称，如果不支持则返回null</returns>
    public static string? GetConvertMethodName(SpecialType specialType)
    {
        return specialType switch
        {
            SpecialType.System_Boolean => "ToBoolean",
            SpecialType.System_Char => "ToChar",
            SpecialType.System_SByte => "ToSByte",
            SpecialType.System_Byte => "ToByte",
            SpecialType.System_Int16 => "ToInt16",
            SpecialType.System_UInt16 => "ToUInt16",
            SpecialType.System_Int32 => "ToInt32",
            SpecialType.System_UInt32 => "ToUInt32",
            SpecialType.System_Int64 => "ToInt64",
            SpecialType.System_UInt64 => "ToUInt64",
            SpecialType.System_Single => "ToSingle",
            SpecialType.System_Double => "ToDouble",
            SpecialType.System_Decimal => "ToDecimal",
            SpecialType.System_DateTime => "ToDateTime",
            _ => null,
        };
    }
    /// <inheritdoc />
    protected override SystemConverter? CreateNew(in PairTypeSymbolKey key)
    {
        var methodName = GetConvertMethodName(key.Left, key.Right);
        if (methodName is null)
            return null;
        return new SystemConverter(SyntaxFactory.IdentifierName(methodName));
    }
    /// <summary>
    /// 获取System.Convert类中对应来源类型和目标类型的转换方法，如果不支持则返回null
    /// </summary>
    /// <param name="from">来源类型</param>
    /// <param name="to">目标类型</param>
    /// <returns>对应的转换方法，如果不支持则返回null</returns>
    public SystemConverter? Get(INamedTypeSymbol from, INamedTypeSymbol to)
        => Get(new PairTypeSymbolKey(from, to));
}
