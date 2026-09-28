using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 定义
/// </summary>
public partial class SyntaxGenerator
{
    #region Namespace
    /// <summary>
    /// 声明命名空间
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NamespaceDeclarationSyntax NamespaceDeclaration(string name)
        => SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(name));
    /// <summary>
    /// 声明文件作用域命名空间
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileScopedNamespaceDeclarationSyntax FileScopedNamespaceDeclaration(string name)
        => SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName(name));
    #endregion
    #region RecordDeclaration
    /// <summary>
    /// 定义记录类型
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordDeclaration(SyntaxToken recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName);
    /// <summary>
    /// 定义记录类型
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordDeclaration(string recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), recordName);
    #endregion
    #region RecordStructDeclaration
    /// <summary>
    /// 定义记录结构体
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordStructDeclaration(SyntaxToken recordName)
        => SyntaxFactory.RecordDeclaration(SyntaxKind.RecordStructDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.RecordKeyword), SyntaxFactory.Token(SyntaxKind.StructKeyword), recordName, default, default, default, default, default, default, default, default);
    /// <summary>
    /// 定义记录结构体
    /// </summary>
    /// <param name="recordName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecordDeclarationSyntax RecordStructDeclaration(string recordName)
        => RecordStructDeclaration(SyntaxFactory.Identifier(recordName));
    #endregion
    #region TypeDeclaration
    /// <summary>
    /// 定义类型
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="isRecord"></param>
    /// <param name="isValueType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TypeDeclarationSyntax TypeDeclaration(string typeName, bool isRecord, bool isValueType)
        => TypeDeclaration(SyntaxFactory.Identifier(typeName), isRecord, isValueType);
    /// <summary>
    /// 定义类型
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="isRecord"></param>
    /// <param name="isValueType"></param>
    /// <returns></returns>
    public static TypeDeclarationSyntax TypeDeclaration(SyntaxToken typeName, bool isRecord, bool isValueType)
    {
        if (isRecord)
        {
            if (isValueType)
                return RecordStructDeclaration(typeName);
            return RecordDeclaration(typeName);
        }
        if (isValueType)
            return SyntaxFactory.StructDeclaration(typeName);
        return SyntaxFactory.ClassDeclaration(typeName);
    }
    #endregion
    #region ParenthesizedVariable
    /// <summary>
    /// 括号变量(用于解构)
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParenthesizedVariableDesignationSyntax ParenthesizedVariable(params SeparatedSyntaxList<VariableDesignationSyntax> variables)
        => SyntaxFactory.ParenthesizedVariableDesignation(variables);
    /// <summary>
    /// 括号变量(用于解构)
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParenthesizedVariableDesignationSyntax ParenthesizedVariable(params IEnumerable<SyntaxToken> variables)
        => ParenthesizedVariable([.. variables.Select(static name => SyntaxFactory.SingleVariableDesignation(name))]);
    /// <summary>
    /// 括号变量(用于解构)
    /// </summary>
    /// <param name="variables"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParenthesizedVariableDesignationSyntax ParenthesizedVariable(params IEnumerable<string> variables)
        => ParenthesizedVariable(variables.Select(static name => SyntaxFactory.Identifier(name)));
    #endregion
    #region ConstructorDeclaration
    /// <summary>
    /// 定义构造函数
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax ConstructorDeclaration(string typeName, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => SyntaxFactory.ConstructorDeclaration(default, default, SyntaxFactory.Identifier(typeName), SyntaxFactory.ParameterList(parameters), default, default, default, default);
    /// <summary>
    /// 定义构造函数
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax ConstructorDeclaration(SyntaxToken typeName, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => SyntaxFactory.ConstructorDeclaration(default, default, typeName, SyntaxFactory.ParameterList(parameters), default, default, default, default);
    #endregion
    #region PrimaryConstructorBaseType
    /// <summary>
    /// 主构造基类
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="baseArguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(TypeSyntax baseType, params ExpressionSyntax[] baseArguments)
        => SyntaxFactory.PrimaryConstructorBaseType(baseType, ArgumentList(baseArguments));
    /// <summary>
    /// 主构造基类
    /// </summary>
    /// <param name="baseType"></param>
    /// <param name="baseArguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PrimaryConstructorBaseTypeSyntax PrimaryConstructorBaseType(string baseType, params ExpressionSyntax[] baseArguments)
        => SyntaxFactory.PrimaryConstructorBaseType(SyntaxFactory.IdentifierName(baseType), ArgumentList(baseArguments));
    #endregion
    #region OperatorDeclaration
    /// <summary>
    /// 运算符重载定义
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="returnType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax OperatorDeclaration(SyntaxKind kind, TypeSyntax returnType, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => SyntaxFactory.OperatorDeclaration(default, SyntaxFactory.TokenList(GenerateServices._public, GenerateServices._static), returnType, default, SyntaxFactory.Token(SyntaxKind.OperatorKeyword), default, SyntaxFactory.Token(kind), SyntaxFactory.ParameterList(parameters), default, default, default);
    /// <summary>
    /// 含a、b参数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax EqualOperatorDeclaration(TypeSyntax type)
        => EqualOperatorDeclaration(type.Parameter("a"), type.Parameter("b"));
    /// <summary>
    /// 重载==
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax EqualOperatorDeclaration(ParameterSyntax a, ParameterSyntax b)
        => OperatorDeclaration(SyntaxKind.EqualsEqualsToken, BoolType, a, b);
    /// <summary>
    /// 含a、b参数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax NotEqualOperatorDeclaration(TypeSyntax type)
        => NotEqualOperatorDeclaration(type.Parameter("a"), type.Parameter("b"));
    /// <summary>
    /// 重载!=
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperatorDeclarationSyntax NotEqualOperatorDeclaration(ParameterSyntax a, ParameterSyntax b)
        => OperatorDeclaration(SyntaxKind.ExclamationEqualsToken, BoolType, a, b);
    #endregion

    #region DeclareAccessor
    /// <summary>
    /// 属性Get处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyGetDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
    /// <summary>
    /// 属性Get处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyGetDeclaration(ExpressionSyntax expression)
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.GetKeyword), default, ExpressionBody(expression), SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    /// <summary>
    /// 属性Set处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertySetDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
    /// <summary>
    /// 属性Set处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertySetDeclaration(ExpressionSyntax expression)
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.SetKeyword), default, ExpressionBody(expression), SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    /// <summary>
    /// 属性Init处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyInitDeclaration()
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration);
    /// <summary>
    /// 属性Init处理器
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorDeclarationSyntax PropertyInitDeclaration(ExpressionSyntax expression)
        => SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration, default, default, SyntaxFactory.Token(SyntaxKind.InitKeyword), default, ExpressionBody(expression), SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    #endregion
    /// <summary>
    /// 重写Equals
    /// </summary>
    /// <param name="type"></param>
    public static MethodDeclarationSyntax ObjectEqualsDeclaration(TypeSyntax type)
    {
        // (object? obj)
        var parameter = ObjectType.Nullable().Parameter("obj");
        var methodName = SyntaxFactory.Identifier(nameof(Equals));
        var other = SyntaxFactory.IdentifierName("other");
        // obj is T other
        var checkType = parameter.ToIdentifierName().IsType(type, other.Identifier);
        // Equals(other)
        var checkEquals = methodName.ToIdentifierName().Invocation([other]);
        // public override bool Equals(object? obj) =>
        //     obj is T other && Equals(other);
        return BoolType.Method(methodName, parameter)
            .Public()
            .Override()
            .ToBuilder()
            .Return(checkType.LogicalAnd(checkEquals));
    }
    /// <summary>
    /// 生成判等运算符重载
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nullCondition"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax BuildEqualOperator(TypeSyntax type, bool nullCondition)
    {
        var a = SyntaxFactory.IdentifierName("a");
        var b = SyntaxFactory.IdentifierName("b");
        // public static bool operator ==(T a, T b)")
        var builder = EqualOperatorDeclaration(type)
            .ToBuilder();
        if (nullCondition)
        {
            return builder
                // {
                .Block()
                // if(a is null) return false;
                .If(a.IsNull()).ReturnFalse()
                // return a.Equals(b);
                .Return(a.Access("Equals").Invocation([b]))
                .End();
        }
        else
        {
            // return a.Equals(b);
            return builder.Return(a.Access("Equals").Invocation([b]));
        }
    }
    /// <summary>
    /// 生成判等运算符重载
    /// </summary>
    /// <param name="type"></param>
    /// <param name="nullCondition"></param>
    /// <returns></returns>
    public static OperatorDeclarationSyntax BuildNotEqualOperator(TypeSyntax type, bool nullCondition)
    {
        var a = SyntaxFactory.IdentifierName("a");
        var b = SyntaxFactory.IdentifierName("b");
        // public static bool operator !=(T a, T b)")
        var builder = NotEqualOperatorDeclaration(type)
            .ToBuilder();
        if (nullCondition)
        {
            return builder
                // {
                .Block()
                // if(a is null) return true;
                .If(a.IsNull()).ReturnTrue()
                // return !a.Equals(b);
                .Return(a.Access("Equals").Invocation([b]).LogicalNot())
                .End();
        }
        else
        {
            // return !a.Equals(b);
            return builder.Return(a.Access("Equals").Invocation([b]).LogicalNot());
        }
    }
}
