using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// for
/// </summary>
/// <param name="declaration"></param>
/// <param name="condition"></param>
/// <param name="incrementors"></param>
public class ForBuilder(VariableDeclarationSyntax declaration, ExpressionSyntax condition, List<ExpressionSyntax> incrementors)
    : ScopeBuilder([])
{
    #region 配置
    private readonly VariableDeclarationSyntax _declaration = declaration;
    private readonly List<VariableDeclaratorSyntax> _variables = [];
    private readonly List<ExpressionSyntax> _initializers = [];
    private readonly ExpressionSyntax _condition = condition;
    private readonly List<ExpressionSyntax> _incrementors = incrementors;
    #endregion
    #region Add
    /// <summary>
    /// 添加变量
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    public ForBuilder AddVariable(VariableDeclaratorSyntax variable)
    {
        _variables.Add(variable);
        return this;
    }
    /// <summary>
    /// 添加初始化
    /// </summary>
    /// <param name="initializer"></param>
    /// <returns></returns>
    public ForBuilder AddInitializer(ExpressionSyntax initializer)
    {
        _initializers.Add(initializer);
        return this;
    }
    /// <summary>
    /// 添加自增加
    /// </summary>
    /// <param name="incrementor"></param>
    /// <returns></returns>
    public ForBuilder AddIncrementor(ExpressionSyntax incrementor)
    {
        _initializers.Add(incrementor);
        return this;
    }
    #endregion
    /// <inheritdoc />
    public override StatementSyntax Build()
        => SyntaxFactory.ForStatement(
            _declaration.AddVariables([.. _variables]),
            SyntaxFactory.SeparatedList(_initializers),
            _condition,
            SyntaxFactory.SeparatedList(_incrementors),
            Block(_statements)
            );
}
