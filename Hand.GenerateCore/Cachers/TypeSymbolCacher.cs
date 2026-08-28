using Hand.Cache;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Cachers;

/// <summary>
/// 类型信息缓存
/// </summary>
/// <param name="compilation"></param>
public class TypeSymbolCacher(Compilation compilation)
    : CacheFactoryBase<ITypeSymbol, ITypeSymbolInfo>(new DictionaryCacher<ITypeSymbol, ITypeSymbolInfo>(new Dictionary<ITypeSymbol, ITypeSymbolInfo>(SymbolEqualityComparer.IncludeNullability)))
{
    #region 配置
    private readonly Compilation _compilation = compilation;
    /// <summary>
    /// 获取编译器
    /// </summary>
    public Compilation Compilation
        => _compilation;
    #endregion
    /// <inheritdoc />
    protected override ITypeSymbolInfo CreateNew(in ITypeSymbol original)
    {
        if(original.Kind == SymbolKind.TypeParameter)
            return new ParameterTypeInfo(original);
        var symbol = original;
        var isNullable = false;
        if (original.NullableAnnotation == NullableAnnotation.Annotated)
        {
            isNullable = true;
            symbol = original.WithNullableAnnotation(NullableAnnotation.None);
        }
        // 数组处理
        if (symbol is IArrayTypeSymbol symbolArray && original is IArrayTypeSymbol originalArray)
            return CreateArray(originalArray, symbolArray, isNullable);
        // 未知类型
        if (original is not INamedTypeSymbol originalNamed)
            return new UnknowTypeInfo(original, symbol, isNullable);
        // 结构体可空类型
        if (originalNamed.IsGenericType(SpecialType.System_Nullable_T))
        {
            isNullable = true;
            symbol = originalNamed.TypeArguments[0];
        }
        // 未知类型
        if (symbol is not INamedTypeSymbol symbolNamed)
            return new UnknowTypeInfo(original, symbol, isNullable);
        return CheckSpecialType(originalNamed, symbolNamed, isNullable);
    }
    /// <summary>
    /// 按SpecialType处理
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    private ITypeSymbolInfo CheckSpecialType(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable)
    {
        return symbol.SpecialType switch
        {
            SpecialType.System_Object or SpecialType.System_String or SpecialType.System_Boolean or SpecialType.System_Char or SpecialType.System_SByte or SpecialType.System_Byte or SpecialType.System_Int16 or SpecialType.System_UInt16 or SpecialType.System_Int32 or SpecialType.System_UInt32 or SpecialType.System_Int64 or SpecialType.System_UInt64 or SpecialType.System_Single or SpecialType.System_Double or SpecialType.System_Decimal or SpecialType.System_DateTime
                => new PrimitiveTypeInfo(symbol.SpecialType, original, symbol, isNullable),
            SpecialType.System_Collections_Generic_ICollection_T
                    or SpecialType.System_Collections_Generic_IEnumerable_T
                    or SpecialType.System_Collections_Generic_IList_T
                    or SpecialType.System_Collections_Generic_IReadOnlyCollection_T
                    or SpecialType.System_Collections_Generic_IReadOnlyList_T
                => new CollectionTypeInfo(original, symbol, isNullable, Get(symbol.TypeArguments[0]), true),
            SpecialType.None => CheckTypeKind(original, symbol, isNullable),
            _ => new UnknowTypeInfo(original, symbol, isNullable),
        };
    }
    /// <summary>
    /// 按TypeKind处理
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    private ITypeSymbolInfo CheckTypeKind(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable)
    {
        return symbol.TypeKind switch
        {
            TypeKind.Enum => new EnumTypeInfo(original, symbol, isNullable),
            TypeKind.Class or TypeKind.Struct => CheckComplex(original, symbol, isNullable, false),
            TypeKind.Interface => CheckComplex(original, symbol, isNullable, true),
            _ => new UnknowTypeInfo(original, symbol, isNullable),
        };
    }
    /// <summary>
    /// 检查复杂类型
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <param name="isInterface"></param>
    /// <returns></returns>
    private ITypeSymbolInfo CheckComplex(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, bool isInterface)
    {
        if (symbol.IsGenericType)
        {
            var collection = CheckCollection(original, symbol, isNullable, isInterface);
            if (collection is not null)
                return collection;
            var typeArguments = symbol.TypeArguments;
            var count = typeArguments.Length;
            var elements = new ITypeSymbolInfo[count];
            for (var i = 0; i < count; i++)
                elements[i] = Get(typeArguments[i]);
            return new GenericTypeInfo(original, symbol, symbol.ConstructedFrom, isNullable, isInterface, elements);
        }
        var originalSymbol = SymbolReflection.GetOriginalSymbol(_compilation, symbol);
        if (originalSymbol is not null)
            return new EntityTypeInfo(original, symbol, isNullable, originalSymbol, isInterface);
        return new ComplexTypeInfo(original, symbol, isNullable, isInterface);
    }
    /// <summary>
    /// 检查集合类型
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <param name="isInterface"></param>
    /// <returns></returns>
    private CollectionTypeInfo? CheckCollection(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, bool isInterface)
    {
        var enumerable = symbol.GetGenericCloseInterfaces(SpecialType.System_Collections_Generic_IEnumerable_T)
            .FirstOrDefault();
        if (enumerable is null)
            return null;
        var elementInfo = Get(enumerable.TypeArguments[0]);
        return new CollectionTypeInfo(original, symbol, isNullable, elementInfo, isInterface);
    }

    /// <summary>
    /// 处理数组
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    private ArrayTypeInfo CreateArray(IArrayTypeSymbol original, IArrayTypeSymbol symbol, bool isNullable)
    {
        var elementInfo = Get(symbol.ElementType);
        return new(original, symbol, isNullable, elementInfo);
    }
}
