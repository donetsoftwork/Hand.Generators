using Hand.Tidy;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Hand;

/// <summary>
/// 类型整理扩展方法
/// </summary>
public static partial class GenerateServices
{
    /// <summary>
    /// 简化数组
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static ArrayTypeSyntax SimplifyArray(this ITypeSimplifier simplifier, ArrayTypeSyntax array)
    {
        var element = simplifier.Simplify(array.ElementType);
        return SyntaxFactory.ArrayType(element, array.RankSpecifiers);
    }
    /// <summary>
    /// 简化可空类型
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="nullable"></param>
    /// <returns></returns>
    public static NullableTypeSyntax SimplifyNullable(this ITypeSimplifier simplifier, NullableTypeSyntax nullable)
    {
        var element = simplifier.Simplify(nullable.ElementType);
        return SyntaxFactory.NullableType(element);
    }
    /// <summary>
    /// 简化指针类型
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="pointer"></param>
    /// <returns></returns>
    public static PointerTypeSyntax SimplifyPointer(this ITypeSimplifier simplifier, PointerTypeSyntax pointer)
    {
        var element = simplifier.Simplify(pointer.ElementType);
        return SyntaxFactory.PointerType(element);
    }
    /// <summary>
    /// 简化元组类型
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="tuple"></param>
    /// <returns></returns>
    public static TupleTypeSyntax SimplifyTuple(this ITypeSimplifier simplifier, TupleTypeSyntax tuple)
    {
        var elements = tuple.Elements.Select(element => SyntaxFactory.TupleElement(simplifier.Simplify(element.Type), element.Identifier));
        return SyntaxFactory.TupleType([.. elements]);
    }
    /// <summary>
    /// 简化Ref类型
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static RefTypeSyntax SimplifyRef(this ITypeSimplifier simplifier, RefTypeSyntax type)
    {
        var element = simplifier.Simplify(type.Type);
        return SyntaxFactory.RefType(type.RefKeyword, type.ReadOnlyKeyword, element);
    }
    /// <summary>
    /// 简化Scoped类型
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="scoped"></param>
    /// <returns></returns>
    public static ScopedTypeSyntax SimplifyScoped(this ITypeSimplifier simplifier, ScopedTypeSyntax scoped)
    {
        var element = simplifier.Simplify(scoped.Type);
        return SyntaxFactory.ScopedType(scoped.ScopedKeyword, element);
    }
    /// <summary>
    /// 简化函数指针类型
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="pointer"></param>
    /// <returns></returns>
    public static FunctionPointerTypeSyntax SimplifyPointer(this ITypeSimplifier simplifier, FunctionPointerTypeSyntax pointer)
    {
        var parameters = pointer.ParameterList.Parameters.Select(element => SyntaxFactory.FunctionPointerParameter(element.AttributeLists, element.Modifiers, simplifier.Simplify(element.Type)));
        var parameterList = SyntaxFactory.FunctionPointerParameterList([.. parameters]);
        return SyntaxFactory.FunctionPointerType(pointer.DelegateKeyword, pointer.AsteriskToken, pointer.CallingConvention, parameterList);
    }
    /// <summary>
    /// 简化泛型参数
    /// </summary>
    /// <param name="simplifier"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GenericNameSyntax SimplifyArgumentList(this ITypeSimplifier simplifier, GenericNameSyntax name)
    {
        var arguments = name.TypeArgumentList.Arguments.Select(simplifier.Simplify);
        return SyntaxFactory.GenericName(name.Identifier, SyntaxFactory.TypeArgumentList([.. arguments]));
    }
}
