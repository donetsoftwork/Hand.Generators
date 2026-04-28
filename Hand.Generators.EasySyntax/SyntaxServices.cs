using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 语法扩展方法
/// </summary>
public static partial class GenerateServices
{
    #region ToIdentifierName
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this SyntaxToken name)
        => SyntaxFactory.IdentifierName(name);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this VariableDeclaratorSyntax variable)
        => SyntaxFactory.IdentifierName(variable.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this VariableDeclarationSyntax variable)
        => SyntaxFactory.IdentifierName(variable.Variables.First().Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this FieldDeclarationSyntax field)
        => ToIdentifierName(field.Declaration.Variables.First().Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this PropertyDeclarationSyntax property)
        => SyntaxFactory.IdentifierName(property.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this ParameterSyntax parameter)
        => SyntaxFactory.IdentifierName(parameter.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this MethodDeclarationSyntax method)
        => SyntaxFactory.IdentifierName(method.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IdentifierNameSyntax ToIdentifierName(this TypeDeclarationSyntax type)
        => SyntaxFactory.IdentifierName(type.Identifier);
    #endregion
    #region Qualified
    /// <summary>
    /// 增加限定符
    /// </summary>
    /// <param name="name"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static QualifiedNameSyntax Qualified(this SimpleNameSyntax name, NameSyntax prefix)
        => SyntaxFactory.QualifiedName(prefix, name);
    /// <summary>
    /// 增加限定符
    /// </summary>
    /// <param name="name"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static QualifiedNameSyntax Qualified(this SimpleNameSyntax name, string prefix)
        => SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(prefix), name);
    #endregion    
    ///// <summary>
    ///// 包装表达式
    ///// </summary>
    ///// <param name="attribute"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static CompilationUnitSyntax CreateToUnit(this AttributeSyntax attribute)
    //    => ToCreation(attribute)
    //    .ToUnit([.. attribute.GetUsings()]);
    /// <summary>
    /// 包装表达式
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="usings"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CompilationUnitSyntax ToUnit(this ExpressionSyntax expression, params IReadOnlyCollection<UsingDirectiveSyntax> usings)
        => SyntaxFactory.CompilationUnit()
            .Using(usings)
            .AddMembers(SyntaxFactory.GlobalStatement(SyntaxFactory.ReturnStatement(expression)))
            .NormalizeWhitespace();
}
