using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 编译扩展方法
/// </summary>
public static partial class ReflectionServices
{
    #region Type
    /// <summary>
    /// 获取INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetSymbol(this Compilation compilation, string typeFullName)
        => compilation.GetTypeByMetadataName(typeFullName);
    ///// <summary>
    ///// 获取INamedTypeSymbol
    ///// </summary>
    ///// <param name="compilation"></param>
    ///// <param name="typeInfo"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static INamedTypeSymbol? GetSymbol(this Compilation compilation, TypeNameInfo typeInfo)
    //    => compilation.GetTypeByMetadataName(typeInfo.FullName);
    /// <summary>
    /// 把Type转INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetSymbol(this Compilation compilation, Type type)
        => compilation.GetTypeByMetadataName(type.FullName!);
    /// <summary>
    /// 把PredefinedTypeSyntax转INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="predefinedType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static INamedTypeSymbol GetSymbol(this Compilation compilation, PredefinedTypeSyntax predefinedType)
    {
        return predefinedType.Keyword.Kind() switch
        {
            SyntaxKind.BoolKeyword => compilation.GetSpecialType(SpecialType.System_Boolean),
            SyntaxKind.ByteKeyword => compilation.GetSpecialType(SpecialType.System_Byte),
            SyntaxKind.SByteKeyword => compilation.GetSpecialType(SpecialType.System_SByte),
            SyntaxKind.IntKeyword => compilation.GetSpecialType(SpecialType.System_Int32),
            SyntaxKind.UIntKeyword => compilation.GetSpecialType(SpecialType.System_UInt32),
            SyntaxKind.ShortKeyword => compilation.GetSpecialType(SpecialType.System_Int16),
            SyntaxKind.UShortKeyword => compilation.GetSpecialType(SpecialType.System_UInt16),
            SyntaxKind.LongKeyword => compilation.GetSpecialType(SpecialType.System_Int64),
            SyntaxKind.ULongKeyword => compilation.GetSpecialType(SpecialType.System_UInt64),
            SyntaxKind.FloatKeyword => compilation.GetSpecialType(SpecialType.System_Single),
            SyntaxKind.DoubleKeyword => compilation.GetSpecialType(SpecialType.System_Double),
            SyntaxKind.DecimalKeyword => compilation.GetSpecialType(SpecialType.System_Decimal),
            SyntaxKind.StringKeyword => compilation.GetSpecialType(SpecialType.System_String),
            SyntaxKind.CharKeyword => compilation.GetSpecialType(SpecialType.System_Char),
            SyntaxKind.ObjectKeyword => compilation.GetSpecialType(SpecialType.System_Object),
            SyntaxKind.VoidKeyword => compilation.GetSpecialType(SpecialType.System_Void),
            _ => throw new ArgumentException($"Kind of {predefinedType} is Invalid", nameof(predefinedType)),
        };
    }
    /// <summary>
    /// 把NullableTypeSyntax转INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="nullableType"></param>
    /// <returns></returns>
    public static INamedTypeSymbol? GetSymbol(this Compilation compilation, NullableTypeSyntax nullableType)
    {
        var elementSymbol = GetSymbol(compilation, nullableType.ElementType);
        if (elementSymbol is null)
            return null;
        return compilation.GetSpecialType(SpecialType.System_Nullable_T)
            .Construct(elementSymbol);
    }
    /// <summary>
    /// 把ArrayTypeSyntax转INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="arrayType"></param>
    /// <returns></returns>
    public static IArrayTypeSymbol? GetSymbol(this Compilation compilation, ArrayTypeSyntax arrayType)
    {
        var elementSymbol = GetSymbol(compilation, arrayType.ElementType);
        if (elementSymbol is null)
            return null;
        return compilation.CreateArrayTypeSymbol(elementSymbol);
    }
    /// <summary>
    /// 把TypeSyntax转INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ITypeSymbol? GetSymbol(this Compilation compilation, TypeSyntax type)
    {
        if (type is PredefinedTypeSyntax predefinedType)
            return GetSymbol(compilation, predefinedType);
        if (type is NullableTypeSyntax nullableType)
            return GetSymbol(compilation, nullableType);
        if(type is ArrayTypeSyntax arrayType)
            return GetSymbol(compilation, arrayType);
        if (type is GenericNameSyntax genericType)
            return GetSymbol(compilation, genericType);
        if (type is QualifiedNameSyntax qualifiedType && qualifiedType.Right is GenericNameSyntax genericRight)
            return GetSymbol(compilation, genericRight, qualifiedType.Left);
        return compilation.GetTypeByMetadataName(type.ToFullString());
    }
    /// <summary>
    /// 把GenericNameSyntax转INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="genericType"></param>
    /// <param name="qualified"></param>
    /// <returns></returns>
    public static ITypeSymbol? GetSymbol(this Compilation compilation, GenericNameSyntax genericType, NameSyntax? qualified = null)
    {
        var arguments = genericType.TypeArgumentList.Arguments;
        int count = arguments.Count;
        string genericName;
        if (qualified is null)
            genericName = genericType.Identifier.ValueText + "`" + count;
        else
            genericName = qualified.ToFullString() + "." + genericType.Identifier.ValueText + "`" + count;
        var genericSymbol = compilation.GetTypeByMetadataName(genericName);
        if (genericSymbol is null)
            return null;
        var argumentTypes = new ITypeSymbol[count];
        for (int i = 0; i < count; i++)
        {
            var argument = arguments[i];
            if (argument.IsKind(SyntaxKind.OmittedTypeArgument))
                return genericSymbol;
            var argumentType = GetSymbol(compilation, argument);
            if (argumentType is null)
                return genericSymbol;
            argumentTypes[i] = argumentType;
        }
        return genericSymbol.Construct(argumentTypes);
    }
    /// <summary>
    /// bool
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetBoolSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Boolean);
    /// <summary>
    /// byte
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetByteSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Byte);
    /// <summary>
    /// sbyte
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetSByteSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_SByte);
    /// <summary>
    /// int
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetIntSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Int32);
    /// <summary>
    /// uint
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetUIntSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_UInt32);
    /// <summary>
    /// short
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetShortSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Int16);
    /// <summary>
    /// ushort
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetUShortSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_UInt16);
    /// <summary>
    /// long
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetLongSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Int64);
    /// <summary>
    /// ulong
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetULongSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_UInt64);
    /// <summary>
    /// float
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetFloatSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Single);
    /// <summary>
    /// double
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetDoubleSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Double);
    /// <summary>
    /// decimal
    /// </summary>
    /// <param name="compilation"></param>
    public static INamedTypeSymbol GetDecimalSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Decimal);
    /// <summary>
    /// string
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetStringSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_String);
    /// <summary>
    /// char
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetCharSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Char);
    /// <summary>
    /// DateTime
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetDateTimeSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_DateTime);
    /// <summary>
    /// object
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetObjectSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Object);
    /// <summary>
    /// void
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetVoidSymbol(this Compilation compilation)
        => compilation.GetSpecialType(SpecialType.System_Void);
    /// <summary>
    /// 可空类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITypeSymbol GetNullable(this Compilation compilation, ITypeSymbol originalSymbol)
    {
        if (originalSymbol.IsValueType)
        {
            originalSymbol = compilation.GetSpecialType(SpecialType.System_Nullable_T)
                .Construct(originalSymbol);
        }
        return originalSymbol.WithNullableAnnotation(NullableAnnotation.Annotated);
    }
    /// <summary>
    /// 迭代类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetIEnumerable(this Compilation compilation, ITypeSymbol originalSymbol)
        => compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
        .Construct(originalSymbol);
    /// <summary>
    /// 迭代器类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetIEnumerator(this Compilation compilation, ITypeSymbol originalSymbol)
        => compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerator_T)
        .Construct(originalSymbol);
    /// <summary>
    /// 列表类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetIList(this Compilation compilation, ITypeSymbol originalSymbol)
        => compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T)
        .Construct(originalSymbol);
    /// <summary>
    /// 集合类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol GetICollection(this Compilation compilation, ITypeSymbol originalSymbol)
        => compilation.GetSpecialType(SpecialType.System_Collections_Generic_ICollection_T)
        .Construct(originalSymbol);
    /// <summary>
    /// 获取列表类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetListSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Generic.List`1");
    /// <summary>
    /// 列表类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetList(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetListSymbol(compilation)?.Construct(originalSymbol);
    /// <summary>
    /// 获取HashSet类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetHashSetSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Generic.HashSet`1");
    /// <summary>
    /// HashSet类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetHashSet(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetHashSetSymbol(compilation)?.Construct(originalSymbol);
    /// <summary>
    /// 获取Queue类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetQueueSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Generic.Queue`1");
    /// <summary>
    /// Queue类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetQueue(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetQueueSymbol(compilation)?.Construct(originalSymbol);
    /// <summary>
    /// 获取Stack类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetStackSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Generic.Stack`1");
    /// <summary>
    /// Stack类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetStack(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetStackSymbol(compilation)?.Construct(originalSymbol);
    /// <summary>
    /// 获取BlockingCollection类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetBlockingCollectionSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Concurrent.BlockingCollection`1");
    /// <summary>
    /// BlockingCollection类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetBlockingCollection(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetBlockingCollectionSymbol(compilation)?.Construct(originalSymbol);
    /// <summary>
    /// 获取ConcurrentQueue类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetConcurrentQueueSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Concurrent.ConcurrentQueue`1");
    /// <summary>
    /// ConcurrentQueue类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetConcurrentQueue(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetConcurrentQueueSymbol(compilation)?.Construct(originalSymbol);
    /// <summary>
    /// 获取ConcurrentStack类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetConcurrentStackSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Concurrent.ConcurrentStack`1");
    /// <summary>
    /// ConcurrentStack类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetConcurrentStack(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetConcurrentStackSymbol(compilation)?.Construct(originalSymbol);
    /// <summary>
    /// 获取ConcurrentBag类型符号
    /// </summary>
    /// <param name="compilation"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetConcurrentBagSymbol(this Compilation compilation)
        => compilation.GetTypeByMetadataName("System.Collections.Concurrent.ConcurrentBag`1");
    /// <summary>
    /// ConcurrentBag类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="originalSymbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetConcurrentBag(this Compilation compilation, ITypeSymbol originalSymbol)
        => GetConcurrentBagSymbol(compilation)?.Construct(originalSymbol);
    #endregion
    /// <summary>
    /// 检查类型是否partial
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CheckIsPartial(this Compilation compilation, INamedTypeSymbol? symbol)
        => symbol is null || !SymbolEqualityComparer.Default.Equals(compilation.Assembly, symbol.ContainingAssembly) || symbol.IsPartial();
    /// <summary>
    /// 判断是否含partial修饰符
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartial(this ISymbol symbol)
    {
        return symbol.Locations.Length switch
        {
            1 => CheckPartialByReferences(symbol),
            0 => false,
            _ => true,
        };
    }
    /// <summary>
    /// 按引用判断是否含partial修饰符
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    private static bool CheckPartialByReferences(ISymbol symbol)
    {
        var reference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (reference == null) 
            return false;
        if (reference.GetSyntax() is MemberDeclarationSyntax member)
            return member.Modifiers.IsPartial();
        return false;
    }
    #region SpecialType
    /// <summary>
    /// 是否特殊类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="specialType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSpecialType(this ITypeSymbol symbol, SpecialType specialType)
        => symbol.SpecialType == specialType;
    /// <summary>
    /// 是bool类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBool(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Boolean;
    /// <summary>
    /// 是byte类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsByte(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Byte;
    /// <summary>
    /// 是sbyte类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSByte(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_SByte;
    /// <summary>
    /// 是Int类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInt(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Int32;
    /// <summary>
    /// 是uint类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUInt(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_UInt32;
    /// <summary>
    /// 是short类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsShort(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Int16;
    /// <summary>
    /// 是UShort类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsUShort(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_UInt16;
    /// <summary>
    /// 是long类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLong(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Int64;
    /// <summary>
    /// 是ulong类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsULong(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_UInt64;
    /// <summary>
    /// 是float类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFloat(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Single;
    /// <summary>
    /// 是double类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDouble(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Double;
    /// <summary>
    /// 是decimal类型 
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDecimal(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Decimal;
    /// <summary>
    /// 是string类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsString(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_String;
    /// <summary>
    /// 是char类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsChar(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Char;
    /// <summary>
    /// 是DateTime类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool IsDateTime(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_DateTime;
    /// <summary>
    /// 是Object类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsObject(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Object;
    /// <summary>
    /// 是Void类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsVoid(this ITypeSymbol symbol)
        => symbol.SpecialType == SpecialType.System_Void;
    #region IsPrimitiveType
    /// <summary>
    /// 是否为基础类型
    /// </summary>
    /// <param name="specialType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitiveType(this SpecialType specialType)
        => (uint)(specialType - 7) <= 13u;
    /// <summary>
    /// 是否为基础类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPrimitiveType(this ITypeSymbol symbol)
        => symbol.SpecialType.IsPrimitiveType();
    #endregion
    #region IsNumericType
    /// <summary>
    /// 是否数值类型
    /// </summary>
    /// <param name="specialType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumericType(this SpecialType specialType)
        => (uint)(specialType - 9) <= 10u;
    /// <summary>
    /// 是否数值类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumericType(this ITypeSymbol symbol)
        => symbol.SpecialType.IsNumericType();
    #endregion
    #region IsIntegralType
    /// <summary>
    /// 是否整数类型
    /// </summary>
    /// <param name="specialType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIntegralType(this SpecialType specialType)
        => (uint)(specialType - 9) <= 7u;
    /// <summary>
    /// 是否整数类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIntegralType(this ITypeSymbol symbol)
        => symbol.SpecialType.IsIntegralType();
    #endregion
    /// <summary>
    /// 是否枚举类型
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnum(this ITypeSymbol symbol)
        => symbol.TypeKind == TypeKind.Enum;
    #endregion
    /// <summary>
    /// 是否兼容
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    public static bool IsCompatible(this ITypeSymbol source, ITypeSymbol dest)
    {
        if (dest.TypeKind == TypeKind.Interface)
            return IsInterface(source, dest);
        return IsBase(source, dest);
    }
    /// <summary>
    /// 是否基类
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static bool IsBase(this ITypeSymbol type, ITypeSymbol baseType)
    {
        if (type.Equals(baseType, SymbolEqualityComparer.Default))
            return true;
        var @base = type.BaseType;
        if (@base is null)
            return false;
        return IsBase(@base, baseType);
    }
    /// <summary>
    /// 是否接口
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    public static bool IsInterface(this ITypeSymbol type, ITypeSymbol interfaceType)
    {
        if (type.Equals(interfaceType, SymbolEqualityComparer.Default))
            return true;
        foreach (var @interface in type.Interfaces)
        {
            if (IsInterface(@interface, interfaceType))
                return true;
        }
        return false;
    }
    #region IsGenericType
    /// <summary>
    /// 是否泛型定义
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="definitionType">泛型</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsGenericType(this INamedTypeSymbol symbol, INamedTypeSymbol definitionType)
        => symbol.IsGenericType && definitionType.Equals(symbol.ConstructedFrom, SymbolEqualityComparer.Default);
    #endregion
    #region HasGenericType
    /// <summary>
    /// 判断是否包含泛型定义
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="definitionType"></param>
    /// <returns></returns>
    public static bool HasGenericType(this INamedTypeSymbol symbol, INamedTypeSymbol definitionType)
    {
        if (IsGenericType(symbol, definitionType))
            return true;
        foreach (var subType in symbol.Interfaces)
        {
            if (IsGenericType(subType, definitionType))
                return true;
        }
        return false;
    }
    #endregion
    #region GetGenericCloseInterfaces
    /// <summary>
    /// 获取泛型闭合接口
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="definitionType"></param>
    /// <returns></returns>
    public static IEnumerable<INamedTypeSymbol> GetGenericCloseInterfaces(this INamedTypeSymbol symbol, INamedTypeSymbol definitionType)
    {
        if (IsGenericType(symbol, definitionType))
        {
            yield return symbol;
            yield break;
        }
        var interfaces = symbol.Interfaces;
        foreach (var item in interfaces)
        {
            if (IsGenericType(item, definitionType))
                yield return item;
        }
    }
    #endregion
    
    /// <summary>
    /// 展示基础类型
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="specialType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeSyntax Display(this SyntaxGenerator generator, SpecialType specialType)
    {
        return specialType switch
        {
            SpecialType.System_Boolean
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
            SpecialType.System_Byte
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword)),
            SpecialType.System_SByte
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword)),
            SpecialType.System_Int32
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            SpecialType.System_UInt32
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
            SpecialType.System_Int16
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword)),
            SpecialType.System_UInt16
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword)),
            SpecialType.System_Int64
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword)),
            SpecialType.System_UInt64
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword)),
            SpecialType.System_Single
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword)),
            SpecialType.System_Double
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword)),
            SpecialType.System_Decimal
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword)),
            SpecialType.System_String
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
            SpecialType.System_Char
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword)),
            SpecialType.System_DateTime
                => generator.Display("DateTime", "System"),
            _
                => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
        };
    }
}
