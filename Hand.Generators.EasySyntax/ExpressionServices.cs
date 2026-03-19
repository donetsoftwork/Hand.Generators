using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand;

/// <summary>
/// 表达式扩展方法
/// </summary>
public static partial class GenerateServices
{
    #region ToIdentifierName
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IdentifierNameSyntax ToIdentifierName(this SyntaxToken name)
        => SyntaxFactory.IdentifierName(name);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static IdentifierNameSyntax ToIdentifierName(this VariableDeclaratorSyntax variable)
        => SyntaxFactory.IdentifierName(variable.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static IdentifierNameSyntax ToIdentifierName(this VariableDeclarationSyntax variable)
        => SyntaxFactory.IdentifierName(variable.Variables.First().Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static IdentifierNameSyntax ToIdentifierName(this FieldDeclarationSyntax field)
        => ToIdentifierName(field.Declaration.Variables.First().Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static IdentifierNameSyntax ToIdentifierName(this PropertyDeclarationSyntax property)
        => SyntaxFactory.IdentifierName(property.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static IdentifierNameSyntax ToIdentifierName(this ParameterSyntax parameter)
        => SyntaxFactory.IdentifierName(parameter.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static IdentifierNameSyntax ToIdentifierName(this MethodDeclarationSyntax method)
        => SyntaxFactory.IdentifierName(method.Identifier);
    /// <summary>
    /// 转化为变量名
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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
    public static QualifiedNameSyntax Qualified(this SimpleNameSyntax name, NameSyntax prefix)
        => SyntaxFactory.QualifiedName(prefix, name);
    /// <summary>
    /// 增加限定符
    /// </summary>
    /// <param name="name"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static QualifiedNameSyntax Qualified(this SimpleNameSyntax name, string prefix)
        => SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(prefix), name);
    #endregion
    #region Binary
    /// <summary>
    /// +
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Add(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, left, right);
    /// <summary>
    /// -
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Subtract(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, left, right);
    /// <summary>
    /// *
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Multiply(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression, left, right);
    /// <summary>
    /// /
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Divide(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression, left, right);
    /// <summary>
    /// %
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Modulo(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.ModuloExpression, left, right);
    /// <summary>
    /// &amp;
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax And(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseAndExpression, left, right);
    /// <summary>
    /// |
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Or(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseOrExpression, left, right);
    /// <summary>
    /// ^
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Xor(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.ExclusiveOrExpression, left, right);
    /// <summary>
    /// &lt;&lt;
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax LeftShift(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.BinaryExpression(SyntaxKind.LeftShiftExpression, variable, value);
    /// <summary>
    /// >>
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax RightShift(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.BinaryExpression(SyntaxKind.RightShiftExpression, variable, value);
    /// <summary>
    /// &amp;&amp;
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax LogicalAnd(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);
    /// <summary>
    /// ||
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax LogicalOr(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.LogicalOrExpression, left, right);
    #endregion
    #region PrefixUnaryExpression
    /// <summary>
    /// ++variable
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static PrefixUnaryExpressionSyntax PreIncrement(this ExpressionSyntax variable)
        => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, variable);
    /// <summary>
    /// --variable
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static PrefixUnaryExpressionSyntax PreDecrement(this ExpressionSyntax variable)
        => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, variable);
    /// <summary>
    /// 按位取反
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static PrefixUnaryExpressionSyntax Not(this ExpressionSyntax variable)
        => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.BitwiseNotExpression, variable);
    /// <summary>
    /// 逻辑否
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static PrefixUnaryExpressionSyntax LogicalNot(this ExpressionSyntax variable)
        => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, variable);
    #endregion
    #region PostfixUnaryExpression
    /// <summary>
    /// variable++
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static PostfixUnaryExpressionSyntax PostIncrement(this ExpressionSyntax variable)
        => SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, variable);
    /// <summary>
    /// variable--
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static PostfixUnaryExpressionSyntax PostDecrement(this ExpressionSyntax variable)
        => SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, variable);
    #endregion
    #region Access
    /// <summary>
    /// 定位到成员
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static MemberAccessExpressionSyntax Access(this ExpressionSyntax owner, SimpleNameSyntax member)
        => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, owner, member);
    /// <summary>
    /// 定位到成员
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static MemberAccessExpressionSyntax Access(this ExpressionSyntax owner, string member)
        => SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, owner, SyntaxFactory.IdentifierName(member));
    /// <summary>
    /// 定位到路径
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static ExpressionSyntax Access(this ExpressionSyntax owner, params  string[] path)
    {
        foreach (var item in path)
            owner = Access(owner, SyntaxFactory.IdentifierName(item));
        return owner;
    }
    #endregion
    #region Element
    /// <summary>
    /// 读取索引
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax Element(this ExpressionSyntax owner, params IEnumerable<ArgumentSyntax> arguments)
        => SyntaxFactory.ElementAccessExpression(owner, SyntaxFactory.BracketedArgumentList(SyntaxFactory.SeparatedList(arguments)));
    /// <summary>
    /// 读取索引
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax Element(this ExpressionSyntax owner, IEnumerable<ExpressionSyntax> arguments)
        => Element(owner, arguments.Select(SyntaxFactory.Argument));
    /// <summary>
    /// 读取索引
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    public static ExpressionSyntax Element(this ExpressionSyntax owner, IEnumerable<SyntaxToken> variables)
        => Element(owner, variables.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    /// <summary>
    /// 读取索引
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    public static ExpressionSyntax Element(this ExpressionSyntax owner, IEnumerable<string> variables)
        => Element(owner, variables.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    #endregion
    #region ConditionalAccess
    /// <summary>
    /// 条件定位
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static ConditionalAccessExpressionSyntax ConditionalAccess(this ExpressionSyntax owner, SimpleNameSyntax member)
        => SyntaxFactory.ConditionalAccessExpression(owner, member);
    /// <summary>
    /// 条件定位
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    public static ConditionalAccessExpressionSyntax ConditionalAccess(this ExpressionSyntax owner, string member)
        => SyntaxFactory.ConditionalAccessExpression(owner, SyntaxFactory.MemberBindingExpression(SyntaxFactory.IdentifierName(member)));
    /// <summary>
    /// 条件定位
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static ExpressionSyntax ConditionalAccess(this ExpressionSyntax owner, params string[] path)
    {        
        foreach (var item in path)
            owner = ConditionalAccess(owner, SyntaxFactory.IdentifierName(item));
        return owner;
    }
    #endregion
    /// <summary>
    /// 判等逻辑
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax Equal(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, left, right);
    /// <summary>
    /// 不等判断
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax NotEqual(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);
    /// <summary>
    /// >
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax GreaterThan(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.GreaterThanExpression, left, right);
    /// <summary>
    /// >=
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax GreaterOrEqual(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.GreaterThanOrEqualExpression, left, right);
    /// <summary>
    /// &lt;
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax LessThan(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.LessThanExpression, left, right);
    /// <summary>
    /// &lt;=
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax LessOrEqual(this ExpressionSyntax left, ExpressionSyntax right)
        => SyntaxFactory.BinaryExpression(SyntaxKind.LessThanOrEqualExpression, left, right);
    /// <summary>
    /// 是否为NULL
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax IsNull(this ExpressionSyntax variable)
        => SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, variable, SyntaxGenerator.NullLiteral);
    /// <summary>
    /// 是否不为NULL
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static BinaryExpressionSyntax NotNull(this ExpressionSyntax variable)
        => SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, variable, SyntaxGenerator.NullLiteral);
    /// <summary>
    /// 判断类型
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IsPatternExpressionSyntax IsType(this ExpressionSyntax variable, TypeSyntax type)
        => SyntaxFactory.IsPatternExpression(variable, SyntaxFactory.TypePattern(type));
    /// <summary>
    /// 判断类型
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static IsPatternExpressionSyntax IsType(this ExpressionSyntax variable, TypeSyntax type, SyntaxToken name)
        => SyntaxFactory.IsPatternExpression(variable, SyntaxFactory.DeclarationPattern(type, SyntaxFactory.SingleVariableDesignation(name)));
    #region Invocation
    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="method"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax Invocation(this ExpressionSyntax method, params IEnumerable<ArgumentSyntax> arguments)
        => SyntaxFactory.InvocationExpression(method, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments)));
    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="method"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax Invocation(this ExpressionSyntax method, IEnumerable<ExpressionSyntax> arguments)
        => Invocation(method, arguments.Select(SyntaxFactory.Argument));
    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="method"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    public static ExpressionSyntax Invocation(this ExpressionSyntax method, IEnumerable<SyntaxToken> variables)
        => Invocation(method, variables.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="method"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    public static ExpressionSyntax Invocation(this ExpressionSyntax method, IEnumerable<string> variables)
        => Invocation(method, variables.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    #endregion
    #region ConditionalInvocation
    /// <summary>
    /// 条件调用方法
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static ExpressionSyntax ConditionalInvocation(this ExpressionSyntax owner, SimpleNameSyntax methodName)
        => ConditionalInvocation(owner, methodName, Enumerable.Empty<ArgumentSyntax>());
    /// <summary>
    /// 条件调用方法
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="methodName"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax ConditionalInvocation(this ExpressionSyntax owner, SimpleNameSyntax methodName, IEnumerable<ArgumentSyntax> arguments)
        => SyntaxFactory.ConditionalAccessExpression(owner, SyntaxFactory.InvocationExpression(SyntaxFactory.MemberBindingExpression(methodName), SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments))));
    /// <summary>
    /// 条件调用方法
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="methodName"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ExpressionSyntax ConditionalInvocation(this ExpressionSyntax owner, SimpleNameSyntax methodName, IEnumerable<ExpressionSyntax> arguments)
        => ConditionalInvocation(owner, methodName, arguments.Select(SyntaxFactory.Argument));
    /// <summary>
    /// 条件调用方法
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="methodName"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    public static ExpressionSyntax ConditionalInvocation(this ExpressionSyntax owner, SimpleNameSyntax methodName, IEnumerable<SyntaxToken> variables)
        => ConditionalInvocation(owner, methodName, variables.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    /// <summary>
    /// 条件调用方法
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="methodName"></param>
    /// <param name="variables"></param>
    /// <returns></returns>
    public static ExpressionSyntax ConditionalInvocation(this ExpressionSyntax owner, SimpleNameSyntax methodName, IEnumerable<string> variables)
        => ConditionalInvocation(owner, methodName, variables.Select(name => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name))));
    #endregion
    #region Statement
    /// <summary>
    /// 返回值
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static ReturnStatementSyntax Return(this ExpressionSyntax expression)
        => SyntaxFactory.ReturnStatement(expression);
    /// <summary>
    /// 标签
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static LabeledStatementSyntax Label(this StatementSyntax statement, SyntaxToken name)
        => SyntaxFactory.LabeledStatement(name, statement);
    /// <summary>
    /// 标签
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static LabeledStatementSyntax Label(this StatementSyntax statement, string name)
       => SyntaxFactory.LabeledStatement(name, statement);
    /// <summary>
    /// 标签
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static LabeledStatementSyntax Label(this ExpressionSyntax expression, SyntaxToken name)
        => SyntaxFactory.LabeledStatement(name, SyntaxFactory.ExpressionStatement(expression));
    /// <summary>
    /// 标签
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static LabeledStatementSyntax Label(this ExpressionSyntax expression, string name)
       => SyntaxFactory.LabeledStatement(name, SyntaxFactory.ExpressionStatement(expression));
    #endregion
}
