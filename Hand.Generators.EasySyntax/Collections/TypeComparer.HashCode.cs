using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Hand.Collections;

partial class TypeComparer
{
    private const int _identifierSeed = 386910137;
    private const int _genericSeed = 290182597;
    private const int _qualifiedSeed = 515880193;
    private const int _nullableSeed = 687840301;
    private const int _pointerSeed = 917120411;
    private const int _scopedSeed = 68861641;
    private const int _arraySeed = 91815541;
    private const int _tupleSeed = 122420729;
    private const int _functionPointerSeed = 163227661;
    private const int _refSeed = 217636919;
    private const int _elementSeed = 122420729;
    private const int _stepSeed = 211;

    /// <summary>
    /// 获取SyntaxToken的HashCode
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetTokenHashCode(SyntaxToken token)
        => token.ValueText.GetHashCode();
    /// <summary>
    /// 获取预定义类型HashCode
    /// </summary>
    /// <param name="predefined"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPredefinedHashCode(PredefinedTypeSyntax predefined)
        => predefined.Keyword.ValueText.GetHashCode();
    /// <summary>
    /// 获取简单标识名HashCode
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIdentifierHashCode(IdentifierNameSyntax identifier)
        => identifier.Identifier.ValueText.GetHashCode() ^ _identifierSeed;
    /// <summary>
    /// 获取简单标识名HashCode
    /// </summary>
    /// <param name="generic"></param>
    /// <returns></returns>
    public static int GetGenericHashCode(GenericNameSyntax generic)
    {
        var hashCode = GetTokenHashCode(generic.Identifier);
        var elementSeed = _elementSeed;
        foreach (var argument in generic.TypeArgumentList.Arguments)
            hashCode ^= GetTypeHashCode(argument) ^ (elementSeed += _stepSeed);
        return hashCode ^ _genericSeed;
    }
    /// <summary>
    /// 获取限定标识名HashCode
    /// </summary>
    /// <param name="qualified"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetQualifiedHashCode(QualifiedNameSyntax qualified)
        => GetNameHashCode(qualified.Left) ^ GetTypeHashCode(qualified.Right) ^ _qualifiedSeed;
    /// <summary>
    /// 获取可空类型HashCode
    /// </summary>
    /// <param name="nullable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetNullableHashCode(NullableTypeSyntax nullable)
        => GetTypeHashCode(nullable.ElementType) ^ _nullableSeed;
    /// <summary>
    /// 获取指针类型HashCode
    /// </summary>
    /// <param name="pointer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPointerHashCode(PointerTypeSyntax pointer)
        => GetTypeHashCode(pointer.ElementType) ^ _pointerSeed;
    /// <summary>
    /// 获取可空类型HashCode
    /// </summary>
    /// <param name="scoped"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetScopedHashCode(ScopedTypeSyntax scoped)
        => GetTypeHashCode(scoped.Type) ^ _scopedSeed;
    /// <summary>
    /// 获取数组类型HashCode
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetArrayHashCode(ArrayTypeSyntax array)
        => GetTypeHashCode(array.ElementType) ^ (_arraySeed + array.RankSpecifiers.Count);
    /// <summary>
    /// 获取元组类型HashCode
    /// </summary>
    /// <param name="tuple"></param>
    /// <returns></returns>
    public static int GetTupleHashCode(TupleTypeSyntax tuple)
    {
        var hashCode = _tupleSeed;
        var elementSeed = _elementSeed;
        foreach (var element in tuple.Elements)
            hashCode ^= GetTokenHashCode(element.Identifier) ^ GetTypeHashCode(element.Type) ^ (elementSeed += _stepSeed);
        return hashCode;
    }
    /// <summary>
    /// 获取Ref类型HashCode
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetRefHashCode(RefTypeSyntax type)
        => GetTokenHashCode(type.RefKeyword) ^ GetTokenHashCode(type.ReadOnlyKeyword) ^ GetTypeHashCode(type.Type) ^ _refSeed;
    /// <summary>
    /// 获取函数指针HashCode
    /// </summary>
    /// <param name="pointer"></param>
    /// <returns></returns>
    public static int GetFunctionPointerHashCode(FunctionPointerTypeSyntax pointer)
        => GetFunctionParametersCode(pointer.ParameterList.Parameters) ^ GetFunctionCallingConventionCode(pointer.CallingConvention);
    private static int GetFunctionParametersCode(SeparatedSyntaxList<FunctionPointerParameterSyntax> parameters)
    {
        var hashCode = _functionPointerSeed;
        var elementSeed = _elementSeed;
        foreach (var element in parameters)
            hashCode ^= GetTypeHashCode(element.Type) ^ (elementSeed += _stepSeed);
        return hashCode;
    }
    private static int GetFunctionCallingConventionCode(FunctionPointerCallingConventionSyntax? callingConvention)
    {
        if (callingConvention is null)
            return _functionPointerSeed;
        var hashcode = GetTokenHashCode(callingConvention.ManagedOrUnmanagedKeyword);
        var list = callingConvention.UnmanagedCallingConventionList;
        if (list is null)
            return hashcode;
        var elementSeed = _elementSeed;
        foreach (var item in list.CallingConventions)
            hashcode ^= GetTokenHashCode(item.Name) ^ (elementSeed += _stepSeed);
        return hashcode;
    }
}
