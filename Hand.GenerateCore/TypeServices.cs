using Hand.Members;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 编译扩展方法
/// </summary>
public static partial class GenerateCoreServices
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
    /// <summary>
    /// 获取INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetSymbol(this Compilation compilation, TypeNameInfo typeInfo)
        => compilation.GetTypeByMetadataName(typeInfo.FullName);
    /// <summary>
    /// 把Type转INamedTypeSymbol
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static INamedTypeSymbol? GetSymbol(this Compilation compilation, Type type)
        => compilation.GetTypeByMetadataName(type.FullName);
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
    public static INamedTypeSymbol GetNullable(this Compilation compilation, INamedTypeSymbol originalSymbol)
    {
        if (originalSymbol.IsValueType)
            return compilation.GetSpecialType(SpecialType.System_Nullable_T)
                .Construct(originalSymbol);
        return (INamedTypeSymbol)originalSymbol.WithNullableAnnotation(NullableAnnotation.Annotated);
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
    #endregion
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
    public static bool CheckPartialByReferences(ISymbol symbol)
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
}
