using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 字典构建器
/// </summary>
public class DictionaryBuilder
{
    private readonly List<InitializerExpressionSyntax> _initializers = [];

    /// <summary>
    /// 添加子项
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public DictionaryBuilder Add(ExpressionSyntax key, ExpressionSyntax value)
    {
        var item = SyntaxFactory.InitializerExpression(SyntaxKind.ComplexElementInitializerExpression, [key, value]);
        _initializers.Add(item);
        return this;
    }
    /// <summary>
    /// 构建初始化器
    /// </summary>
    /// <returns></returns>
    public InitializerExpressionSyntax BuildInitializer()
        => SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression, [.. _initializers]);
    /// <summary>
    /// 构造字典
    /// </summary>
    /// <param name="dictionaryType"></param>
    /// <param name="argumentList"></param>
    /// <returns></returns>
    public ObjectCreationExpressionSyntax Build(TypeSyntax dictionaryType, ArgumentListSyntax? argumentList = null)
        => SyntaxFactory.ObjectCreationExpression(dictionaryType, argumentList, BuildInitializer());
    /// <summary>
    /// 构造字典
    /// </summary>
    /// <param name="argumentList"></param>
    /// <returns></returns>
    public ImplicitObjectCreationExpressionSyntax Build(ArgumentListSyntax argumentList)
        => SyntaxFactory.ImplicitObjectCreationExpression(argumentList, BuildInitializer());
    /// <summary>
    /// 构造字典
    /// </summary>
    /// <returns></returns>
    public ImplicitObjectCreationExpressionSyntax Build()
        => SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList(), BuildInitializer());
}
