using Hand.Attributes;
using Hand.Cache;
using Hand.Reflection;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Builders;

/// <summary>
/// 类型信息缓存
/// </summary>
/// <param name="compilation"></param>
public class TypeInfoBuilder(Compilation compilation)
    : CacheFactoryBase<ITypeSymbol, ITypeSymbolInfo>(new DictionaryCacher<ITypeSymbol, ITypeSymbolInfo>(new Dictionary<ITypeSymbol, ITypeSymbolInfo>(SymbolEqualityComparer.IncludeNullability)))
{
    #region 配置
    private readonly Compilation _compilation = compilation;
    /// <summary>
    /// 获取编译器
    /// </summary>
    public Compilation Compilation
        => _compilation;
    /// <summary>
    /// 枚举位标记
    /// </summary>
    private readonly Lazy<INamedTypeSymbol?> _flags = new(() => EnumTypeInfo.GetFlagsAttributeType(compilation));
    /// <summary>
    /// 枚举接口
    /// </summary>
    private readonly Lazy<INamedTypeSymbol?> _enumerationInterface = new(() => EnumerationTypeInfo.GetEnumerationInterface(compilation));
    /// <summary>
    /// 位标记枚举接口
    /// </summary>
    private readonly Lazy<INamedTypeSymbol?> _flagEnumerationInterface = new(() => EnumerationTypeInfo.GetFlagEnumerationInterface(compilation));
    /// <summary>
    /// Guid类型符号
    /// </summary>
    private readonly Lazy<INamedTypeSymbol?> _guid = new(() => compilation.GetTypeByMetadataName("System.Guid"));
    #endregion
    /// <inheritdoc />
    protected override ITypeSymbolInfo CreateNew(in ITypeSymbol original)
    {
        if (original.Kind == SymbolKind.TypeParameter)
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
            SpecialType.System_Object
                => new PredefinedTypeInfo(SyntaxKind.ObjectKeyword, original, symbol, isNullable),
            SpecialType.System_String
                => new PredefinedTypeInfo(SyntaxKind.StringKeyword, original, symbol, isNullable),
            SpecialType.System_Boolean
                => new PredefinedTypeInfo(SyntaxKind.BoolKeyword, original, symbol, isNullable),
            SpecialType.System_Char
                => new PredefinedTypeInfo(SyntaxKind.CharKeyword, original, symbol, isNullable),
            SpecialType.System_SByte
                => new PredefinedTypeInfo(SyntaxKind.SByteKeyword, original, symbol, isNullable),
            SpecialType.System_Byte
                => new PredefinedTypeInfo(SyntaxKind.ByteKeyword, original, symbol, isNullable),
            SpecialType.System_Int16
                => new PredefinedTypeInfo(SyntaxKind.ShortKeyword, original, symbol, isNullable),
            SpecialType.System_UInt16
                => new PredefinedTypeInfo(SyntaxKind.UShortKeyword, original, symbol, isNullable),
            SpecialType.System_Int32
                => new PredefinedTypeInfo(SyntaxKind.IntKeyword, original, symbol, isNullable),
            SpecialType.System_UInt32
                => new PredefinedTypeInfo(SyntaxKind.UIntKeyword, original, symbol, isNullable),
            SpecialType.System_Int64
                => new PredefinedTypeInfo(SyntaxKind.LongKeyword, original, symbol, isNullable),
            SpecialType.System_UInt64
                => new PredefinedTypeInfo(SyntaxKind.ULongKeyword, original, symbol, isNullable),
            SpecialType.System_Single
                => new PredefinedTypeInfo(SyntaxKind.FloatKeyword, original, symbol, isNullable),
            SpecialType.System_Double
                => new PredefinedTypeInfo(SyntaxKind.DoubleKeyword, original, symbol, isNullable),
            SpecialType.System_Decimal
                => new PredefinedTypeInfo(SyntaxKind.DecimalKeyword, original, symbol, isNullable),
            SpecialType.System_DateTime
                => new PrimitiveTypeInfo(original, symbol, isNullable),
            SpecialType.System_Collections_Generic_ICollection_T
                or SpecialType.System_Collections_Generic_IEnumerable_T
                or SpecialType.System_Collections_Generic_IList_T
                or SpecialType.System_Collections_Generic_IReadOnlyCollection_T
                or SpecialType.System_Collections_Generic_IReadOnlyList_T
                => new CollectionTypeInfo(original, symbol, isNullable, Get(symbol.TypeArguments[0]), true),
            SpecialType.System_Void => new VoidTypeInfo(original),
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
            TypeKind.Enum => CreateEnum(original, symbol, isNullable, _flags.Value),
            TypeKind.Class or TypeKind.Struct => CheckComplex(original, symbol, isNullable, false),
            TypeKind.Interface => CheckComplex(original, symbol, isNullable, true),
            _ => new UnknowTypeInfo(original, symbol, isNullable),
        };
    }
    /// <summary>
    /// 处理枚举类型
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static EnumTypeInfo CreateEnum(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, INamedTypeSymbol? flags)
    {
        if (flags is null)
            return new EnumTypeInfo(original, symbol, false, isNullable);
        var isFlags = SymbolAttributeHelper.GetAttributesByType(symbol, flags).Any();
        return new EnumTypeInfo(original, symbol, isFlags, isNullable);
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
        // 特殊基础类型
        if (symbol.Equals(_guid.Value, SymbolEqualityComparer.Default))
            return new PrimitiveTypeInfo(original, symbol, isNullable);

        var originalSymbol = SymbolReflection.GetOriginalSymbol(_compilation, symbol);
        if (originalSymbol is not null)
            return new EntityPropertyTypeInfo(original, symbol, isNullable, originalSymbol, isInterface);
        var enumeration = CheckEnumeration(original, symbol, isNullable, isInterface);
        if (enumeration is not null)
            return enumeration;
        return new ComplexTypeInfo(original, symbol, isNullable, isInterface);
    }
    /// <summary>
    /// 检查枚举类
    /// </summary>
    /// <param name="original"></param>
    /// <param name="symbol"></param>
    /// <param name="isNullable"></param>
    /// <param name="isInterface"></param>
    /// <returns></returns>
    public EnumerationTypeInfo? CheckEnumeration(INamedTypeSymbol original, INamedTypeSymbol symbol, bool isNullable, bool isInterface)
    {
        var enumerationInterface = _enumerationInterface.Value;
        var flagEnumerationInterface = _flagEnumerationInterface.Value;
        if (enumerationInterface is null || flagEnumerationInterface is null)
            return null;
        var identifierType = _compilation.GetStringSymbol();
        var originalType = _compilation.GetIntSymbol();
        // 枚举接口
        if (isInterface)
        {
            if(symbol.Equals(flagEnumerationInterface, SymbolEqualityComparer.Default))
                return new(original, symbol, true, isNullable, true, identifierType, originalType);
            if (symbol.Equals(enumerationInterface, SymbolEqualityComparer.Default))
                return new(original, symbol, false, isNullable, true, identifierType, originalType);
            return null;
        }
        if (!symbol.IsInterface(enumerationInterface))
            return null;
        var isFlag = symbol.IsInterface(flagEnumerationInterface);
        var enumeration = new EnumerationTypeInfo(original, symbol, isFlag, isNullable, false, identifierType, originalType);
        return enumeration;
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
