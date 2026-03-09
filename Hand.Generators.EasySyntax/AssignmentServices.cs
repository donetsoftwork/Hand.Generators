using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand;

/// <summary>
/// 赋值表达式
/// </summary>
public static partial class GenerateServices
{
    #region 配置
    private static readonly IdentifierNameSyntax _value = SyntaxFactory.IdentifierName("value");
    #endregion
    /// <summary>
    /// =
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax Assign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, variable, value);
    /// <summary>
    /// +=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax AddAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.AddAssignmentExpression, variable, value);
    /// <summary>
    /// -=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax SubtractAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.SubtractAssignmentExpression, variable, value);
    /// <summary>
    /// *=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax MultiplyAssign(this ExpressionSyntax variable, ExpressionSyntax value)
      => SyntaxFactory.AssignmentExpression(SyntaxKind.MultiplyAssignmentExpression, variable, value);
    /// <summary>
    /// /=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax DivideAssign(this ExpressionSyntax variable, ExpressionSyntax value)
      => SyntaxFactory.AssignmentExpression(SyntaxKind.DivideAssignmentExpression, variable, value);
    /// <summary>
    /// %=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax ModuloAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.ModuloAssignmentExpression, variable, value);
    /// <summary>
    /// &=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax AndAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.AndAssignmentExpression, variable, value);
    /// <summary>
    /// |=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax OrAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.OrAssignmentExpression, variable, value);
    /// <summary>
    /// ^=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax XOrAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.ExclusiveOrAssignmentExpression, variable, value);
    /// <summary>
    /// <<=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax LeftShiftAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.LeftShiftAssignmentExpression, variable, value);
    /// <summary>
    /// >>=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax RightShiftAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.RightShiftAssignmentExpression, variable, value);
    /// <summary>
    /// ??=
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax CoalesceAssign(this ExpressionSyntax variable, ExpressionSyntax value)
        => SyntaxFactory.AssignmentExpression(SyntaxKind.CoalesceAssignmentExpression, variable, value);
    #region AssignValue
    /// <summary>
    /// 给字段赋值
    /// </summary>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax AssignValue(this ExpressionSyntax variable)
        => Assign(variable, _value);
    /// <summary>
    /// 给字段赋值
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax AssignValue(this FieldDeclarationSyntax field)
        => Assign(field.ToIdentifierName(), _value);
    /// <summary>
    /// 给字段赋值
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public static AssignmentExpressionSyntax AssignValue(this SyntaxToken variable)
        => Assign(variable.ToIdentifierName(), _value);
    #endregion
}
