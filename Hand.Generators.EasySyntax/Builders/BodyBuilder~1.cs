using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 函数构造器基类
/// </summary>
public abstract class BodyBuilder<TParent>(TParent parent)
    : StatementBuilder<TParent>(parent)
{
    ///// <summary>
    ///// 构造
    ///// </summary>
    //public TParent Build()
    //{
    //    var result = BuildCore();
    //    _statements.Clear();
    //    return result;
    //}
    ///// <summary>
    ///// 返回
    ///// </summary>
    ///// <param name="variable"></param>
    //public TParent Return(SyntaxToken variable)
    //    => Return(SyntaxFactory.IdentifierName(variable));
    ///// <summary>
    ///// 返回
    ///// </summary>
    ///// <param name="expression"></param>
    //public TParent Return(ExpressionSyntax expression)
    //{
    //    _statements.Add(expression.Return());
    //    return Build();
    //}
    ///// <summary>
    ///// 返回
    ///// </summary>
    //public TParent Return()
    //    => Build();
    /// <summary>
    /// 打包
    /// </summary>
    /// <returns></returns>
    protected BlockSyntax BuildBody()
        => SyntaxFactory.Block(SyntaxGenerator.List(_statements));
}
