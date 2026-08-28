using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Arguments;

/// <summary>
/// 对象构造表达式
/// </summary>
/// <param name="arguments"></param>
/// <param name="items"></param>
public class CreationBuilder(List<ArgumentSyntax> arguments, List<AssignmentExpressionSyntax> items)
    : ArgumentCollection(arguments)
{
    /// <summary>
    /// 对象构造表达式
    /// </summary>
    public CreationBuilder()
        : this([], [])
    {
    }
    private readonly List<AssignmentExpressionSyntax> _items = items;

    /// <summary>
    /// 构造
    /// </summary>
    /// <returns></returns>
    public ImplicitObjectCreationExpressionSyntax Build()
    {
        if(_items.Count == 0)
            return SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList([.._arguments]), default);
        return SyntaxFactory.ImplicitObjectCreationExpression(SyntaxFactory.ArgumentList([.. _arguments]), SyntaxGenerator.Initializer(_items));
    }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public CreationBuilder Initialize(AssignmentExpressionSyntax item)
    {
        _items.Add(item);
        return this;
    }
    /// <summary>
    /// 初始化成员
    /// </summary>
    /// <param name="member"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreationBuilder Initialize(ExpressionSyntax member, ExpressionSyntax value)
        => Initialize(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, member, value));
    /// <summary>
    /// 初始化成员
    /// </summary>
    /// <param name="member"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public CreationBuilder Initialize(string member, ExpressionSyntax value)
        => Initialize(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(member), value));

}
