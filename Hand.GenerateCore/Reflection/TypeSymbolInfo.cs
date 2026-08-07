using Hand.Cachers;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace Hand.Reflection;

/// <summary>
/// 类型信息
/// </summary>
/// <param name="original"></param>
/// <param name="symbol"></param>
/// <param name="kind"></param>
/// <param name="elementSymbol"></param>
/// <param name="summary"></param>
public class TypeSymbolInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, TypeSymbolKind kind, INamedTypeSymbol? elementSymbol, Lazy<string> summary)
    : IEquatable<TypeSymbolInfo>
{
    /// <summary>
    /// 成员类型信息
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="kind"></param>
    /// <param name="elementSymbol"></param>
    public TypeSymbolInfo(INamedTypeSymbol original, INamedTypeSymbol symbol, TypeSymbolKind kind, INamedTypeSymbol? elementSymbol)
        :this(original, symbol, kind, elementSymbol,new Lazy<string>(()=> SummaryCacher.GetSummary(symbol)))
    {
    }

    #region 配置
    private readonly INamedTypeSymbol _original = original;
    private readonly INamedTypeSymbol _symbol = symbol;
    private readonly TypeSymbolKind _kind = kind;
    private readonly INamedTypeSymbol? _elementSymbol = elementSymbol;
    private readonly Lazy<string> _summary = summary;

    /// <summary>
    /// 原始类型
    /// </summary>
    public INamedTypeSymbol Original
        => _original;
    /// <summary>
    /// 类型
    /// </summary>
    public INamedTypeSymbol Symbol 
        => _symbol;
    /// <summary>
    /// 成员类别
    /// </summary>
    public TypeSymbolKind Kind 
        => _kind;
    /// <summary>
    /// 子类型
    /// </summary>
    public INamedTypeSymbol? Element 
        => _elementSymbol;
    /// <summary>
    /// 备注
    /// </summary>
    public string Summary
        => _summary.Value;
    #endregion
    /// <summary>
    /// 解构
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="kind"></param>
    /// <param name="elementSymbol"></param>
    public void Deconstruct(out INamedTypeSymbol original, out INamedTypeSymbol symbol, out TypeSymbolKind kind, out INamedTypeSymbol? elementSymbol)
    {
        original = _original;
        symbol = _symbol;
        kind = _kind;
        elementSymbol = _elementSymbol;
    }
    /// <summary>
    /// 简化类型
    /// </summary>
    /// <returns></returns>
    public INamedTypeSymbol CheckPoco()
    {
        if ((_kind & TypeSymbolKind.Entity) == TypeSymbolKind.Entity)
            return _elementSymbol ?? _symbol;
        return _symbol;
    }
    /// <inheritdoc />
    public bool Equals(TypeSymbolInfo? other)
        => _original.Equals(other?._original, SymbolEqualityComparer.Default);

    /// <inheritdoc />
    public override int GetHashCode()
        => SymbolEqualityComparer.Default.GetHashCode(_original);
    /// <summary>
    /// 构造成员类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public static TypeSymbolInfo? Create(Compilation compilation, ITypeSymbol original)
    {
        if(original is not INamedTypeSymbol namedType)
            return null;
        return Create(compilation, namedType);
    }
    /// <summary>
    /// 构造成员类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="original"></param>
    /// <returns></returns>
    public static TypeSymbolInfo? Create(Compilation compilation, INamedTypeSymbol original)
    {
        var symbol = original;
        var kind = TypeSymbolKind.Primitive;
        if (original.IsGenericType(SpecialType.System_Nullable_T) && original.TypeArguments[0] is INamedTypeSymbol namedType)
        {
            kind = TypeSymbolKind.Nullable;
            symbol = namedType;
        }
        else if (original.NullableAnnotation == NullableAnnotation.Annotated)
        {
            kind = TypeSymbolKind.Nullable;
            symbol = original.ConstructedFrom;
        }
        return CheckSpecialType(compilation, original, symbol, kind);
    }
    /// <summary>
    /// 获取类型备注
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string GetSummary(INamedTypeSymbol symbol)
        => SummaryCacher.GetSummary(symbol, symbol.ToDisplayString());
    /// <summary>
    /// 按SpecialType处理
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    private static TypeSymbolInfo? CheckSpecialType(Compilation compilation, INamedTypeSymbol original, INamedTypeSymbol symbol, TypeSymbolKind kind)
    {          
        return symbol.SpecialType switch
        {
            SpecialType.System_Object or SpecialType.System_String or SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_SByte or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal or SpecialType.System_DateTime
                => new(original, symbol, kind, null, new Lazy<string>(() => string.Empty)),
            _ => CheckTypeKind(compilation, original, symbol, kind),
        };
    }
    /// <summary>
    /// 按TypeKind处理
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    private static TypeSymbolInfo? CheckTypeKind(Compilation compilation, INamedTypeSymbol original, INamedTypeSymbol symbol, TypeSymbolKind kind)
    {
        return symbol.TypeKind switch
        {
            TypeKind.Enum => new(original, symbol, kind |= TypeSymbolKind.Enum, symbol.EnumUnderlyingType),
            TypeKind.Array => CheckArray(original, symbol, kind),
            TypeKind.Class or TypeKind.Struct => CheckComplex(compilation, original, symbol, kind),
            _ => null,
        };
    }
    /// <summary>
    /// 处理数组类型
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    private static TypeSymbolInfo CheckArray(INamedTypeSymbol original, INamedTypeSymbol symbol, TypeSymbolKind kind)
    {
        var elementSymbol = (INamedTypeSymbol)symbol.TypeArguments[0];
        return new(original, symbol, kind | TypeSymbolKind.Array, elementSymbol, new Lazy<string>(() => GetSummary(elementSymbol)));
    }
    /// <summary>
    /// 检查复杂类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    private static TypeSymbolInfo CheckComplex(Compilation compilation, INamedTypeSymbol original, INamedTypeSymbol symbol, TypeSymbolKind kind)
    {
        var enumerable = symbol.GetGenericCloseInterfaces(SpecialType.System_Collections_Generic_IEnumerable_T)
            .FirstOrDefault();
        if (enumerable is not null)
        {
            var elementSymbol = (INamedTypeSymbol)enumerable.TypeArguments[0];
            return new(original, symbol, kind | TypeSymbolKind.Collection, elementSymbol, new Lazy<string>(() => GetSummary(elementSymbol)));
        }
        kind |= TypeSymbolKind.Complex;
        var originalSymbol = SymbolReflection.GetOriginalSymbol(compilation, symbol);
        if (originalSymbol is not null)
            return new(original, symbol, kind |= TypeSymbolKind.Entity, originalSymbol);
        return new (original, symbol, kind, null);
    }
}
