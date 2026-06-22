using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Patterns;

/// <summary>
/// 构造递归模式构造器(基类)
/// </summary>
/// <param name="type">类型</param>
/// <param name="name">命名</param>
public abstract class RecursiveBaseBuilder(TypeSyntax? type, SyntaxToken? name)
{
    #region 配置
    private readonly TypeSyntax? _type = type;
    private readonly VariableDesignationSyntax? _designation = name is null ? null : SyntaxFactory.SingleVariableDesignation(name.Value);
    /// <summary>
    /// 类型
    /// </summary>
    public TypeSyntax? Type 
        => _type;
    /// <summary>
    /// 模式变量
    /// </summary>
    public VariableDesignationSyntax? Designation 
        => _designation;
    #endregion
    /// <summary>
    /// 获取位置模式
    /// </summary>
    /// <returns></returns>
    public abstract PositionalPatternClauseSyntax? GetPositional();
    /// <summary>
    /// 获取属性模式
    /// </summary>
    /// <returns></returns>
    public abstract PropertyPatternClauseSyntax? GetProperty();
    /// <summary>
    /// 构造递归模式
    /// </summary>
    /// <returns></returns>
    public RecursivePatternSyntax Build()
        => SyntaxFactory.RecursivePattern(_type, GetPositional(), GetProperty(), _designation);
}
