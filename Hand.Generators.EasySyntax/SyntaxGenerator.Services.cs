using Hand.Scopes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 类语法树构造器扩展方法
/// </summary>
public static partial class GenerateServices
{
    /// <summary>
    /// 增加基类
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddBaseType(this ITypeGenerator generator, TypeSyntax baseType)
        => generator.AddBaseType(SyntaxFactory.SimpleBaseType(baseType));
    /// <summary>
    /// 增加主构造函数参数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ListParameter(this ITypeGenerator generator, TypeSyntax type, SyntaxToken name)
        => generator.AddListParameter(type.Parameter(name));
    /// <summary>
    /// 添加using System
    /// </summary>
    /// <param name="scope"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UsingSystem(this ISyntaxScope scope)
        => scope.Using(SyntaxGenerator.SystemDirective);
    /// <summary>
    /// 添加using System.Collections.Generic
    /// </summary>
    /// <param name="scope"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UsingCollections(this ISyntaxScope scope)
        => scope.Using(SyntaxGenerator.CollectionDirective);
}
