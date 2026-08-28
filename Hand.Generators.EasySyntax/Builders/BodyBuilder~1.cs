using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 函数构造器基类
/// </summary>
public abstract class BodyBuilder<TParent>(TParent parent)
    : StatementBuilder<TParent>(parent, [])
{
    /// <summary>
    /// 打包
    /// </summary>
    /// <returns></returns>
    protected BlockSyntax BuildBody()
        => SyntaxFactory.Block(_statements);
}
