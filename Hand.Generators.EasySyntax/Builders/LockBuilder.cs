using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// lock
/// </summary>
/// <param name="expression"></param>
public class LockBuilder(ExpressionSyntax expression)
    : ScopeBuilder([])
{
    #region 配置
    private readonly ExpressionSyntax _expression = expression;
    /// <summary>
    /// 
    /// </summary>
    public ExpressionSyntax Expression => _expression;
    #endregion
    /// <inheritdoc />
    public override StatementSyntax Build()
        => SyntaxFactory.LockStatement(_expression, Block(_statements));
}
