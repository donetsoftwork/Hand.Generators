using Hand.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand;

/// <summary>
/// 扩展方法
/// </summary>
public static partial class GenerateServices
{
    #region WithInitializer
    /// <summary>
    /// 赋值
    /// </summary>
    /// <param name="declarator"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VariableDeclaratorSyntax WithInitializer(this VariableDeclaratorSyntax declarator, ExpressionSyntax value)
        => declarator.WithInitializer(SyntaxFactory.EqualsValueClause(value));
    /// <summary>
    /// 赋值
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyDeclarationSyntax WithInitializer(this PropertyDeclarationSyntax property, ExpressionSyntax value)
        => property.WithInitializer(SyntaxFactory.EqualsValueClause(value));
    /// <summary>
    /// 调用构造函数
    /// </summary>
    /// <param name="constructor"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax WithInitializer(this ConstructorDeclarationSyntax constructor, params ExpressionSyntax[] arguments)
        => constructor.WithInitializer(SyntaxFactory.ConstructorInitializer(SyntaxKind.ThisConstructorInitializer, SyntaxGenerator.ArgumentList(arguments)));
    /// <summary>
    /// 调用基类构造函数
    /// </summary>
    /// <param name="constructor"></param>
    /// <param name="baseArguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ConstructorDeclarationSyntax WithBaseInitializer(this ConstructorDeclarationSyntax constructor, params ExpressionSyntax[] baseArguments)
        => constructor.WithInitializer(SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, SyntaxGenerator.ArgumentList(baseArguments)));
    #endregion
    /// <summary>
    /// 增加分号
    /// </summary>
    /// <typeparam name="TDeclaration"></typeparam>
    /// <param name="declaration"></param>
    /// <returns></returns>
    public static TDeclaration WithSemicolonToken<TDeclaration>(this TDeclaration declaration)
        where TDeclaration : CSharpSyntaxNode
    {
        if (declaration is BaseTypeDeclarationSyntax type)
            return (TDeclaration)(CSharpSyntaxNode)type.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        else if (declaration is AccessorDeclarationSyntax accessor)
            return (TDeclaration)(CSharpSyntaxNode)accessor.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is MethodDeclarationSyntax method)
        //    return (TDeclaration)(CSharpSyntaxNode)method.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is OperatorDeclarationSyntax operatorDeclaration)
        //    return (TDeclaration)(CSharpSyntaxNode)operatorDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        else if (declaration is BaseMethodDeclarationSyntax method)
            return (TDeclaration)(CSharpSyntaxNode)method.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is EventFieldDeclarationSyntax eventField)
        //    return (TDeclaration)(CSharpSyntaxNode)eventField.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is FieldDeclarationSyntax field)
        //    return (TDeclaration)(CSharpSyntaxNode)field.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        else if (declaration is BaseFieldDeclarationSyntax field)
            return (TDeclaration)(CSharpSyntaxNode)field.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        else if (declaration is PropertyDeclarationSyntax property)
            return (TDeclaration)(CSharpSyntaxNode)property.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        else if (declaration is IndexerDeclarationSyntax indexer)
            return (TDeclaration)(CSharpSyntaxNode)indexer.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is BasePropertyDeclarationSyntax property)
        //    return (TDeclaration)(CSharpSyntaxNode)property.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        return declaration;
    }
    #region AddBaseTypes
    /// <summary>
    /// 添加基类
    /// </summary>
    /// <typeparam name="TDeclaration"></typeparam>
    /// <param name="declaration"></param>
    /// <param name="baseTypes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclaration AddBaseTypes<TDeclaration>(this TDeclaration declaration, params TypeSyntax[] baseTypes)
        where TDeclaration : BaseTypeDeclarationSyntax
        => (TDeclaration)declaration.AddBaseListTypes(System.Array.ConvertAll(baseTypes, static baseType => SyntaxFactory.SimpleBaseType(baseType)));
    /// <summary>
    /// 添加基类
    /// </summary>
    /// <typeparam name="TDeclaration"></typeparam>
    /// <param name="declaration"></param>
    /// <param name="baseTypes"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclaration AddBaseTypes<TDeclaration>(this TDeclaration declaration, params string[] baseTypes)
        where TDeclaration : BaseTypeDeclarationSyntax
        => (TDeclaration)declaration.AddBaseListTypes(System.Array.ConvertAll(baseTypes, static baseType => SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(baseType))));
    #endregion
    #region AddPrimaryConstructorBaseType
    /// <summary>
    /// 添加主构造基类
    /// </summary>
    /// <typeparam name="TDeclaration"></typeparam>
    /// <param name="declaration"></param>
    /// <param name="baseType"></param>
    /// <param name="baseArguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclaration AddPrimaryConstructorBaseType<TDeclaration>(this TDeclaration declaration, string baseType, params ExpressionSyntax[] baseArguments)
        where TDeclaration : BaseTypeDeclarationSyntax
        => (TDeclaration)declaration.AddBaseListTypes(SyntaxGenerator.PrimaryConstructorBaseType(baseType, baseArguments));
    /// <summary>
    /// 添加主构造基类
    /// </summary>
    /// <typeparam name="TDeclaration"></typeparam>
    /// <param name="declaration"></param>
    /// <param name="baseType"></param>
    /// <param name="baseArguments"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDeclaration AddPrimaryConstructorBaseType<TDeclaration>(this TDeclaration declaration, TypeSyntax baseType, params ExpressionSyntax[] baseArguments)
        where TDeclaration : BaseTypeDeclarationSyntax
        => (TDeclaration)declaration.AddBaseListTypes(SyntaxGenerator.PrimaryConstructorBaseType(baseType, baseArguments));
    #endregion
    #region ToBuilder
    /// <summary>
    /// 转化为代码构造器
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodBodyBuilder<TMethod> ToBuilder<TMethod>(this TMethod method)
        where TMethod : BaseMethodDeclarationSyntax
        => new(method);
    /// <summary>
    /// 转化为代码构造器
    /// </summary>
    /// <param name="accessor"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AccessorBodyBuilder ToBuilder(this AccessorDeclarationSyntax accessor)
        => new(accessor);
    /// <summary>
    /// 转化为代码构造器
    /// </summary>
    /// <param name="function"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LocalFunctionBodyBuilder ToBuilder(this LocalFunctionStatementSyntax function)
        => new(function);
    #endregion
    #region WithExpressionBody
    /// <summary>
    /// 方法增加表达式
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static TMethod WithExpressionBody<TMethod>(this TMethod method, ExpressionSyntax expression)
        where TMethod : BaseMethodDeclarationSyntax
        => (TMethod)method.WithExpressionBody(SyntaxGenerator.ExpressionBody(expression))
        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    /// <summary>
    /// 访问器增加表达式
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static AccessorDeclarationSyntax WithExpressionBody(this AccessorDeclarationSyntax accessor, ExpressionSyntax expression)
        => accessor.WithExpressionBody(SyntaxGenerator.ExpressionBody(expression))
        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    #endregion
    #region AddParameter
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TMethod AddParameter<TMethod>(this TMethod method, TypeSyntax type, SyntaxToken parameterName)
        where TMethod : BaseMethodDeclarationSyntax
        => (TMethod)method.AddParameterListParameters(SyntaxFactory.Parameter(default, default, type, parameterName, null));
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TMethod AddParameter<TMethod>(this TMethod method, TypeSyntax type, string parameterName)
        where TMethod : BaseMethodDeclarationSyntax
        => AddParameter(method, type, SyntaxFactory.Identifier(parameterName));
    #endregion
    #region Using
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="root"></param>
    /// <param name="usings"></param>
    /// <returns></returns>
    public static SyntaxNode Using(this SyntaxNode root, params IReadOnlyCollection<UsingDirectiveSyntax> usings)
    {
        if (root is CompilationUnitSyntax unit)
            return Using(unit, usings);
        if (root is BaseNamespaceDeclarationSyntax ns)
            return Using(ns, usings);
        if (usings.Count == 0)
            return root;
        if (root is MemberDeclarationSyntax member)
        {
            return SyntaxFactory.CompilationUnit()
                .AddUsings([.. usings])
                .AddMembers(member);
        }
        if(root is StatementSyntax statement)
        {
            return SyntaxFactory.CompilationUnit()
                .AddUsings([.. usings])
                .AddMembers(SyntaxFactory.GlobalStatement(statement));
        }
        return root;
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="root"></param>
    /// <param name="usings"></param>
    /// <returns></returns>
    public static CompilationUnitSyntax Using(this CompilationUnitSyntax root, params IReadOnlyCollection<UsingDirectiveSyntax> usings)
    {
        if (usings.Count == 0)
            return root;
        var delta = SyntaxGenerator.Plus(root.Usings, usings);
        if (delta.Count == 0)
            return root;
        return root.AddUsings([.. delta]);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="root"></param>
    /// <param name="usings"></param>
    /// <returns></returns>
    public static BaseNamespaceDeclarationSyntax Using(this BaseNamespaceDeclarationSyntax root, params IReadOnlyCollection<UsingDirectiveSyntax> usings)
    {
        if (usings.Count == 0)
            return root;
        var delta = SyntaxGenerator.Plus(root.Usings, usings);
        if (delta.Count == 0)
            return root;
        return root.AddUsings([.. delta]);
    }
    /// <summary>
    /// 添加Using
    /// </summary>
    /// <param name="root"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public static SyntaxNode Using(this SyntaxNode root, params NameSyntax[] names)
    {
        int count = names.Length;
        if (count == 0)
            return root;
        var list = new UsingDirectiveSyntax[count];
        for (int i = 0; i < count; i++)
            list[i] = SyntaxFactory.UsingDirective(names[i]);
        return Using(root, list);
    }
    #endregion
    /// <summary>
    /// 获取Using
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<UsingDirectiveSyntax> GetUsings(this SyntaxNode node)
        => node.AncestorsAndSelf().SelectMany(GetUsingsCore);
    /// <summary>
    /// 获取Using
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static IEnumerable<UsingDirectiveSyntax> GetUsingsCore(SyntaxNode node)
    {
        if (node is CompilationUnitSyntax unit)
            return unit.Usings;
        if (node is BaseNamespaceDeclarationSyntax ns)
            return ns.Usings;
        return [];
    }
}
