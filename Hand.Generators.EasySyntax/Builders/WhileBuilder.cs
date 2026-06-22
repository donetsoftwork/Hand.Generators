using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// while
/// </summary>
/// <param name="condition"></param>
public class WhileBuilder(ExpressionSyntax condition)
    : ScopeBuilder([])
{
    #region 配置
    /// <summary>
    /// 当前条件
    /// </summary>
    private readonly ExpressionSyntax _condition = condition;
    /// <summary>
    /// 当前条件
    /// </summary>
    public ExpressionSyntax Condition
        => _condition;
    #endregion
    /// <inheritdoc />
    public override StatementSyntax Build()
         => SyntaxFactory.WhileStatement(_condition, Block(_statements));
}
