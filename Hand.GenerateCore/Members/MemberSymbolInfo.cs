using Hand.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace Hand.Members;

/// <summary>
/// 成员类型信息
/// </summary>
/// <param name="symbol"></param>
/// <param name="category"></param>
/// <param name="elementSymbol"></param>
public class MemberSymbolInfo(INamedTypeSymbol symbol, MemberSymbolCategory category, INamedTypeSymbol? elementSymbol)
{
    #region 配置
    private readonly INamedTypeSymbol _symbol = symbol;
    private readonly MemberSymbolCategory _category = category;
    private readonly INamedTypeSymbol? _elementSymbol = elementSymbol;
    /// <summary>
    /// 类型
    /// </summary>
    public INamedTypeSymbol Symbol 
        => _symbol;
    /// <summary>
    /// 成员类别
    /// </summary>
    public MemberSymbolCategory Category 
        => _category;
    /// <summary>
    /// 子类型
    /// </summary>
    public INamedTypeSymbol? ElementSymbol 
        => _elementSymbol;
    #endregion
    /// <summary>
    /// 解构
    /// </summary>
    /// <param name="fromSymbol"></param>
    /// <param name="fromCategory"></param>
    /// <param name="fromElement"></param>
    public void Deconstruct(out INamedTypeSymbol fromSymbol, out MemberSymbolCategory fromCategory, out INamedTypeSymbol? fromElement)
    {
        fromSymbol = _symbol;
        fromCategory = _category;
        fromElement = _elementSymbol;
    }
    /// <summary>
    /// 简化类型
    /// </summary>
    /// <returns></returns>
    public INamedTypeSymbol CheckPoco()
    {
        if ((_category & MemberSymbolCategory.Entity) == MemberSymbolCategory.Entity)
            return _elementSymbol ?? _symbol;
        return _symbol;
    }
    /// <summary>
    /// 构造成员类型信息
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static MemberSymbolInfo Create(Compilation compilation, INamedTypeSymbol symbol)
    {
        var category = MemberSymbolCategory.Primitive;
        if (symbol.IsNullable())
        {
            symbol = symbol.TypeArguments[0] as INamedTypeSymbol
                ?? throw new NotSupportedException("不支持匿名类型");
            category = MemberSymbolCategory.Nullable;
        }
        return CheckSpecialType(compilation, symbol, category);
    }
    /// <summary>
    /// 按SpecialType处理
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    private static MemberSymbolInfo CheckSpecialType(Compilation compilation, INamedTypeSymbol symbol, MemberSymbolCategory category)
    {
        var enumerable = symbol.GetGenericCloseInterfaces(SpecialType.System_Collections_Generic_IEnumerable_T)
            .FirstOrDefault();
        if (enumerable is not null)
            return new(symbol, category | MemberSymbolCategory.Collection, enumerable.TypeParameters[0] as INamedTypeSymbol);

        return symbol.SpecialType switch
        {
            SpecialType.System_Object or SpecialType.System_String or SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_SByte or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal or SpecialType.System_DateTime
                => new(symbol, category, null),
            _ => CheckTypeKind(compilation, symbol, category),
        };
    }
    /// <summary>
    /// 按TypeKind处理
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    private static MemberSymbolInfo CheckTypeKind(Compilation compilation, INamedTypeSymbol symbol, MemberSymbolCategory category)
    {
        return symbol.TypeKind switch
        {
            TypeKind.Enum => new(symbol, category |= MemberSymbolCategory.Enum, symbol.EnumUnderlyingType),
            TypeKind.Array => new(symbol, category |= MemberSymbolCategory.Array, symbol.TypeArguments[0] as INamedTypeSymbol),
            TypeKind.Class or TypeKind.Struct => CheckComplex(compilation, symbol, category),
            _ => throw new NotSupportedException("不支持的类型"),
        };
    }
    /// <summary>
    /// 检查复杂类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    private static MemberSymbolInfo CheckComplex(Compilation compilation, INamedTypeSymbol symbol, MemberSymbolCategory category)
    {
        var entityProperty = SymbolReflection.GetOriginalSymbol(compilation, symbol);
        if (entityProperty is not null)
            return new(symbol, category |= MemberSymbolCategory.Entity, entityProperty.TypeParameters[0] as INamedTypeSymbol);
        return new (symbol, category |= MemberSymbolCategory.Complex, null);
    }
}
