using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Patterns;

/// <summary>
/// 递归模式构造器
/// </summary>
/// <param name="positional"></param>
/// <param name="property"></param>
/// <param name="type"></param>
/// <param name="name"></param>
public class RecursivePatternBuilder<TPositional>(TPositional positional, PropertyClauseBuilder property, TypeSyntax? type, SyntaxToken? name = null)
    : RecursiveBaseBuilder(type, name)
    where TPositional : PositionalBaseBuilder
{
    #region 配置
    /// <summary>
    /// 位置模式
    /// </summary>
    protected readonly TPositional _positional = positional;
    /// <summary>
    /// 属性模式
    /// </summary>
    protected readonly PropertyClauseBuilder _property = property;
    /// <summary>
    /// 位置模式
    /// </summary>
    public TPositional Positional 
        => _positional;
    /// <summary>
    /// 属性模式
    /// </summary>
    public PropertyClauseBuilder Property 
        => _property;
    #endregion
    /// <inheritdoc />
    public override PositionalPatternClauseSyntax? GetPositional()
        => _positional.Build();
    /// <inheritdoc />
    public override PropertyPatternClauseSyntax? GetProperty()
        => _property.Build();
}
