using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Hand.Collections;

partial class TypeComparer
{
    /// <summary>
    /// 比较字符片段
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    internal static bool TokenEqual(SyntaxToken x, SyntaxToken y)
        => string.Equals(x.ValueText, y.ValueText, System.StringComparison.Ordinal);
    /// <summary>
    /// 比较可空类型
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool NullableEquals(NullableTypeSyntax x, NullableTypeSyntax y)
        => Equal(x.ElementType, y.ElementType);
    /// <summary>
    /// 比较标识名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IdentifierEqual(IdentifierNameSyntax x, IdentifierNameSyntax y)
        => TokenEqual(x.Identifier, y.Identifier);
    /// <summary>
    /// 比较标识名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PredefinedEqual(PredefinedTypeSyntax x, PredefinedTypeSyntax y)
        => TokenEqual(x.Keyword, y.Keyword);
    /// <summary>
    /// 比较限定名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool QualifiedEqual(QualifiedNameSyntax x, QualifiedNameSyntax y)
        => Equal(x.Left, y.Left) && Equal(x.Right, y.Right);
    /// <summary>
    /// 比较别名
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool AliasQualifiedEqual(AliasQualifiedNameSyntax x, AliasQualifiedNameSyntax y)
        => Equal(x.Alias, y.Alias) && Equal(x.Name, y.Name);
    /// <summary>
    /// 比较数组
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ArrayEqual(ArrayTypeSyntax x, ArrayTypeSyntax y)
        => x.RankSpecifiers.Count == y.RankSpecifiers.Count && Equal(x.ElementType, y.ElementType);
    /// <summary>
    /// 比较元组
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool TupleEqual(TupleTypeSyntax x, TupleTypeSyntax y)
    {
        var xElements = x.Elements;
        var yElements = y.Elements;
        var count = xElements.Count;
        if (count != yElements.Count)
            return false;
        for (var i = 0; i < count; i++)
        {
            if (!TupleElementEqual(xElements[i], yElements[i]))
                return false;
        }
        return true;
    }
    /// <summary>
    /// 比较元组
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool TupleElementEqual(TupleElementSyntax x, TupleElementSyntax y)
        => TokenEqual(x.Identifier, y.Identifier) && Equal(x.Type, y.Type);
    /// <summary>
    /// 比较指针
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PointerEqual(PointerTypeSyntax x, PointerTypeSyntax y)
        => /*TokenEqual(x.AsteriskToken, y.AsteriskToken) && */Equal(x.ElementType, y.ElementType);
    #region FunctionPointer
    /// <summary>
    /// 比较函数指针
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool FunctionPointerEqual(FunctionPointerTypeSyntax x, FunctionPointerTypeSyntax y)
    {
        var xParameters = x.ParameterList.Parameters;
        var yParameters = y.ParameterList.Parameters;
        var count = xParameters.Count;
        if (count != yParameters.Count)
            return false;
        for (var i = 0; i < count; i++)
        {
            if (!Equal(xParameters[i].Type, yParameters[i].Type))
                return false;
        }
        return CallingConventionEqual(x.CallingConvention, y.CallingConvention);
    }
    internal static bool CallingConventionEqual(FunctionPointerCallingConventionSyntax? x, FunctionPointerCallingConventionSyntax? y)
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;
        return TokenEqual(x.ManagedOrUnmanagedKeyword, y.ManagedOrUnmanagedKeyword) && CallingConventionListEqual(x.UnmanagedCallingConventionList, y.UnmanagedCallingConventionList);
    }
    internal static bool CallingConventionListEqual(FunctionPointerUnmanagedCallingConventionListSyntax? x, FunctionPointerUnmanagedCallingConventionListSyntax? y)
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;
        var xList = x.CallingConventions;
        var yList = y.CallingConventions;
        var count = xList.Count;
        if (count != yList.Count)
            return false;
        for (var i = 0; i < count; i++)
        {
            if (!TokenEqual(xList[i].Name, yList[i].Name))
                return false;
        }
        return true;
    }
    ///// <summary>
    ///// 比较函数指针参数
    ///// </summary>
    ///// <param name="x"></param>
    ///// <param name="y"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //internal static bool FunctionPointerParameterEqual(FunctionPointerParameterSyntax x, FunctionPointerParameterSyntax y)
    //    => /*TokenEquals(x.Modifiers, y.Modifiers) && */Equal(x.Type, y.Type);
    #endregion
    /// <summary>
    /// 比较引用
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool RefEqual(RefTypeSyntax x, RefTypeSyntax y)
        => TokenEqual(x.RefKeyword, y.RefKeyword) && TokenEqual(x.ReadOnlyKeyword, y.ReadOnlyKeyword) && Equal(x.Type, y.Type);
    /// <summary>
    /// 限制引用
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ScopedEqual(ScopedTypeSyntax x, ScopedTypeSyntax y)
        => /*TokenEquals(x.ScopedKeyword, y.ScopedKeyword) && */Equal(x.Type, y.Type);
    /// <summary>
    /// 比较泛型
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool GenericEqual(GenericNameSyntax x, GenericNameSyntax y)
    {
        if (!TokenEqual(x.Identifier, y.Identifier))
            return false;
        var xArguments = x.TypeArgumentList.Arguments;
        var yArguments = y.TypeArgumentList.Arguments;
        var count = xArguments.Count;
        if (count != yArguments.Count)
            return false;
        for (var i = 0; i < count; i++)
        {
            if (!Equal(xArguments[i], yArguments[i]))
                return false;
        }
        return true;
    }
}
