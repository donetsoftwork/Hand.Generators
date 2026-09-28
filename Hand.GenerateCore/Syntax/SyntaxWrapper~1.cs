using Microsoft.CodeAnalysis.CSharp;

namespace Hand.Syntax;

/// <summary>
/// 语法展示包装器
/// </summary>
/// <typeparam name="TSyntax"></typeparam>
/// <param name="original"></param>
public class SyntaxWrapper<TSyntax>(TSyntax original)
    : ISyntaxDisplay<TSyntax>
    where TSyntax : CSharpSyntaxNode
{
    #region 配置
    private readonly TSyntax _original = original;
    /// <summary>
    /// 原始节点
    /// </summary>
    public TSyntax Original 
        => _original;
    #endregion

    /// <inheritdoc />
    public TSyntax Display(SyntaxGenerator generator)
        => _original;
}
