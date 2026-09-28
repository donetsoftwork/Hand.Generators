using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 声明扩展方法
/// </summary>
public static partial class GenerateServices
{
    #region Field
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, SyntaxToken variableName)
        => SyntaxFactory.FieldDeclaration(Variable(type, variableName));
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, string variableName)
        => Field(type, SyntaxFactory.Identifier(variableName));
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, SyntaxToken variableName, ExpressionSyntax value)
        => SyntaxFactory.FieldDeclaration(Variable(type, variableName, value));
    /// <summary>
    /// 定义字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FieldDeclarationSyntax Field(this TypeSyntax type, string variableName, ExpressionSyntax value)
        => Field(type, SyntaxFactory.Identifier(variableName), value);
    #endregion
    #region Property
    #region AccessorDeclarationSyntax
    /// <summary>
    /// 定义属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, string propertyName, params AccessorDeclarationSyntax[] items)
        => Property(propertyType, SyntaxFactory.Identifier(propertyName), items);
    /// <summary>
    /// 定义属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, SyntaxToken propertyName, params AccessorDeclarationSyntax[] items)
        => SyntaxFactory.PropertyDeclaration(default, default, propertyType, default, propertyName, SyntaxFactory.AccessorList([.. items]), default, default, default);
    #endregion
    #region SyntaxKind
    /// <summary>
    /// 定义自动属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="kinds">Get/Set/Init</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, string propertyName, params SyntaxKind[] kinds)
        => Property(propertyType, SyntaxFactory.Identifier(propertyName), kinds);
    /// <summary>
    /// 定义自动属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="kinds">Get/Set/Init</param>
    /// <returns></returns>
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, SyntaxToken propertyName, params SyntaxKind[] kinds)
    {
        var count = kinds.Length;
        var items = new AccessorDeclarationSyntax[count];
        for (int i = 0; i < count; i++)
        {
            // 自动属性(无代码)使用WithSemicolonToken增加;号结束
            items[i] = SyntaxFactory.AccessorDeclaration(kinds[i])
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
        return Property(propertyType, propertyName, items);
    }
    #endregion
    #region expression
    /// <summary>
    /// 定义属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, SyntaxToken propertyName, ExpressionSyntax expression)
        => SyntaxFactory.PropertyDeclaration(default, default, propertyType, default, propertyName, default, SyntaxGenerator.ExpressionBody(expression), default, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    /// <summary>
    /// 定义只读属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax Property(this TypeSyntax propertyType, string propertyName, ExpressionSyntax expression)
        => Property(propertyType, SyntaxFactory.Identifier(propertyName), expression);
    #endregion
    #region PropertyGetOnly
    /// <summary>
    /// 定义只读属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetOnlyProperty(this TypeSyntax propertyType, SyntaxToken propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration);
    /// <summary>
    /// 定义只读属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>

    public static PropertyDeclarationSyntax GetOnlyProperty(this TypeSyntax propertyType, string propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration);
    #endregion
    #region SetOnlyProperty
    /// <summary>
    /// 定义可写属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax SetOnlyProperty(this TypeSyntax propertyType, SyntaxToken propertyName, IdentifierNameSyntax field)
        => SyntaxFactory.PropertyDeclaration(default, default, propertyType, default, propertyName, default, SyntaxGenerator.ExpressionBody(field.AssignValue()), default, SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    /// <summary>
    /// 定义可写属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax SetOnlyProperty(this TypeSyntax propertyType, string propertyName, IdentifierNameSyntax field)
        => SetOnlyProperty(propertyType, SyntaxFactory.Identifier(propertyName), field);
    #endregion
    #region GetSetProperty
    /// <summary>
    /// 定义读写属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetSetProperty(this TypeSyntax propertyType, SyntaxToken propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
    /// <summary>
    /// 定义读写属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetSetProperty(this TypeSyntax propertyType, string propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
    /// <summary>
    /// 定义读写属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetSetProperty(this TypeSyntax propertyType, SyntaxToken propertyName, IdentifierNameSyntax field)
        => propertyType.Property(propertyName, SyntaxGenerator.PropertyGetDeclaration(field), SyntaxGenerator.PropertySetDeclaration(field.AssignValue()));
    /// <summary>
    /// 定义读写属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetSetProperty(this TypeSyntax propertyType, string propertyName, IdentifierNameSyntax field)
        => propertyType.Property(propertyName, SyntaxGenerator.PropertyGetDeclaration(field), SyntaxGenerator.PropertySetDeclaration(field.AssignValue()));
    #endregion
    #region GetInitProperty
    /// <summary>
    /// 定义读初始化属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetInitProperty(this TypeSyntax propertyType, SyntaxToken propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration, SyntaxKind.InitAccessorDeclaration);
    /// <summary>
    /// 定义读初始化属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetInitProperty(this TypeSyntax propertyType, string propertyName)
        => Property(propertyType, propertyName, SyntaxKind.GetAccessorDeclaration, SyntaxKind.InitAccessorDeclaration);
    /// <summary>
    /// 定义读初始化属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetInitProperty(this TypeSyntax propertyType, SyntaxToken propertyName, IdentifierNameSyntax field)
        => propertyType.Property(propertyName, SyntaxGenerator.PropertyGetDeclaration(field), SyntaxGenerator.PropertyInitDeclaration(field.AssignValue()));
    /// <summary>
    /// 定义读初始化属性
    /// </summary>
    /// <param name="propertyType"></param>
    /// <param name="propertyName"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax GetInitProperty(this TypeSyntax propertyType, string propertyName, IdentifierNameSyntax field)
        => propertyType.Property(propertyName, SyntaxGenerator.PropertyGetDeclaration(field), SyntaxGenerator.PropertyInitDeclaration(field.AssignValue()));
    #endregion
    #endregion
    #region Parameter
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, SyntaxToken parameterName)
        => SyntaxFactory.Parameter(default, default, type, parameterName, null);
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, string parameterName)
        => Parameter(type, SyntaxFactory.Identifier(parameterName));
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, SyntaxToken parameterName, ExpressionSyntax defaultValue)
        => SyntaxFactory.Parameter(default, default, type, parameterName, SyntaxFactory.EqualsValueClause(defaultValue));
    /// <summary>
    /// 定义参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ParameterSyntax Parameter(this TypeSyntax type, string parameterName, ExpressionSyntax value)
        => Parameter(type, SyntaxFactory.Identifier(parameterName), value);
    #endregion
    #region Method
    /// <summary>
    /// 定义方法
    /// </summary>
    /// <param name="returnType"></param>
    /// <param name="methodName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodDeclarationSyntax Method(this TypeSyntax returnType, string methodName, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => Method(returnType, SyntaxFactory.Identifier(methodName), parameters);
    /// <summary>
    /// 定义方法
    /// </summary>
    /// <param name="returnType"></param>
    /// <param name="methodName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodDeclarationSyntax Method(this TypeSyntax returnType, SyntaxToken methodName, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => SyntaxFactory.MethodDeclaration(returnType, methodName)
        .WithParameterList(SyntaxFactory.ParameterList(parameters));
    #endregion
    #region LocalFunction
    /// <summary>
    /// 定义局部函数
    /// </summary>
    /// <param name="returnType"></param>
    /// <param name="functionName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LocalFunctionStatementSyntax LocalFunction(this TypeSyntax returnType, string functionName, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => LocalFunction(returnType, SyntaxFactory.Identifier(functionName), parameters);
    /// <summary>
    /// 定义局部函数
    /// </summary>
    /// <param name="returnType"></param>
    /// <param name="functionName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LocalFunctionStatementSyntax LocalFunction(this TypeSyntax returnType, SyntaxToken functionName, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => SyntaxFactory.LocalFunctionStatement(returnType, functionName)
        .WithParameterList(SyntaxFactory.ParameterList(parameters));
    #endregion
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax Constructor(this TypeDeclarationSyntax type, params SeparatedSyntaxList<ParameterSyntax> parameters)
        => SyntaxGenerator.ConstructorDeclaration(type.Identifier, parameters);
    //#region WithNamespace
    ///// <summary>
    ///// 增加命名空间
    ///// </summary>
    ///// <param name="type"></param>
    ///// <param name="namespace"></param>
    ///// <returns></returns>
    //public static TypeDeclarationSyntax WithNamespace(this TypeDeclarationSyntax type, BaseNamespaceDeclarationSyntax @namespace)
    //{
    //    var identifier = type.Identifier;
    //    var item = @namespace.AddMembers(type)
    //        .Members.OfType<TypeDeclarationSyntax>()
    //        .FirstOrDefault(item => item.Identifier.Equals(identifier));
    //    return item ?? type;
    //}
    ///// <summary>
    ///// 增加命名空间
    ///// </summary>
    ///// <param name="type"></param>
    ///// <param name="namespace"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static TypeDeclarationSyntax WithNamespace(this TypeDeclarationSyntax type, NameSyntax @namespace)
    //    => WithNamespace(type, SyntaxFactory.NamespaceDeclaration(@namespace));
    ///// <summary>
    ///// 增加命名空间
    ///// </summary>
    ///// <param name="type"></param>
    ///// <param name="namespace"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static TypeDeclarationSyntax WithNamespace(this TypeDeclarationSyntax type, string @namespace)
    //    => WithNamespace(type, SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(@namespace)));
    //#endregion
    #region Variable
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, SyntaxToken variableName)
        => SyntaxFactory.VariableDeclaration(type, SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(variableName)));
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, string variableName)
        => Variable(type, SyntaxFactory.Identifier(variableName));
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, SyntaxToken variableName, ExpressionSyntax value)
        => SyntaxFactory.VariableDeclaration(type, SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(variableName).WithInitializer(value)));
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclarationSyntax Variable(this TypeSyntax type, string variableName, ExpressionSyntax value)
        => Variable(type, SyntaxFactory.Identifier(variableName), value);
    #endregion
    #region OutArgument
    /// <summary>
    /// 输出实参
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentSyntax OutArgument(this TypeSyntax type, SyntaxToken variableName)
        => SyntaxFactory.Argument(default, _out, SyntaxFactory.DeclarationExpression(type, SyntaxFactory.SingleVariableDesignation(variableName)));
    /// <summary>
    /// 输出实参
    /// </summary>
    /// <param name="type"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArgumentSyntax OutArgument(this TypeSyntax type, string variableName)
        => OutArgument(type, SyntaxFactory.Identifier(variableName));
    #endregion
    #region Catch
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="catchType"></param>
    /// <param name="catchName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CatchDeclarationSyntax Catch(this TypeSyntax catchType, SyntaxToken catchName)
        => SyntaxFactory.CatchDeclaration(catchType, catchName);
    /// <summary>
    /// 定义变量
    /// </summary>
    /// <param name="catchType"></param>
    /// <param name="catchName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CatchDeclarationSyntax Catch(this TypeSyntax catchType, string catchName)
        => Catch(catchType, SyntaxFactory.Identifier(catchName));
    #endregion
}
