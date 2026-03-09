using Hand.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    public static VariableDeclaratorSyntax WithInitializer(this VariableDeclaratorSyntax declarator, ExpressionSyntax value)
        => declarator.WithInitializer(SyntaxFactory.EqualsValueClause(value));
    /// <summary>
    /// 赋值
    /// </summary>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static PropertyDeclarationSyntax WithInitializer(this PropertyDeclarationSyntax property, ExpressionSyntax value)
        => property.WithInitializer(SyntaxFactory.EqualsValueClause(value));
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
        else if (declaration is MethodDeclarationSyntax method)
            return (TDeclaration)(CSharpSyntaxNode)method.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is BaseMethodDeclarationSyntax method)
        //    return (TDeclaration)(CSharpSyntaxNode)method.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is EventFieldDeclarationSyntax eventField)
        //    return (TDeclaration)(CSharpSyntaxNode)eventField.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is FieldDeclarationSyntax field)
        //    return (TDeclaration)(CSharpSyntaxNode)field.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is PropertyDeclarationSyntax property)
        //    return (TDeclaration)(CSharpSyntaxNode)property.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is IndexerDeclarationSyntax indexer)
        //    return (TDeclaration)(CSharpSyntaxNode)indexer.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //else if (declaration is OperatorDeclarationSyntax operatorDeclaration)
        //    return (TDeclaration)(CSharpSyntaxNode)operatorDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        return declaration;
    }
    #region ToBuilder
    /// <summary>
    /// 转化为代码构造器
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static MethodBodyBuilder<TMethod> ToBuilder<TMethod>(this TMethod method)
        where TMethod : BaseMethodDeclarationSyntax
        => new(method);
    /// <summary>
    /// 转化为代码构造器
    /// </summary>
    /// <param name="accessor"></param>
    /// <returns></returns>
    public static AccessorBodyBuilder ToBuilder(this AccessorDeclarationSyntax accessor)
        => new(accessor);
    #endregion
    #region AddParameter
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static TMethod AddParameter<TMethod>(this TMethod method, TypeSyntax type, SyntaxToken parameterName)
        where TMethod : BaseMethodDeclarationSyntax
    {
        var parameter = SyntaxFactory.Parameter(parameterName)
            .WithType(type);
        return (TMethod)method.AddParameterListParameters(parameter);
    }
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="method"></param>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    public static TMethod AddParameter<TMethod>(this TMethod method, TypeSyntax type, string parameterName)
        where TMethod : BaseMethodDeclarationSyntax
        => AddParameter(method, type, SyntaxFactory.Identifier(parameterName));
    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="list"></param>
    /// <param name="type"></param>
    /// <param name="parameterName"></param>
    /// <returns></returns>
    public static ParameterSyntax AddParameter(this ParameterListSyntax list, TypeSyntax type, SyntaxToken parameterName)
    {
        var parameter = SyntaxFactory.Parameter(parameterName)
            .WithType(type);
        list.Parameters.Add(parameter);
        return parameter;
    }
    #endregion
}
