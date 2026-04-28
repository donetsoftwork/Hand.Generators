using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;

namespace Hand.Filters;

/// <summary>
/// 过滤节点
/// </summary>
/// <param name="isPartial"></param>
/// <param name="kinds"></param>
public class SyntaxFilter(bool isPartial, params SyntaxKind[] kinds)
    : ISyntaxFilter
{
    #region 配置
    private readonly bool _isPartial = isPartial;
    /// <summary>
    /// 节点类型
    /// </summary>
    private readonly SyntaxKind[] _kinds = kinds;
    #endregion
    /// <summary>
    /// 过滤节点
    /// </summary>
    /// <param name="isPartial"></param>
    public SyntaxFilter(bool isPartial = true)
        : this(isPartial, SyntaxKind.RecordStructDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.ClassDeclaration)
    {
    }
    /// <inheritdoc />
    public bool Match(SyntaxNode node, CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
            return false;
        return CheckNode(node);
    }
    /// <summary>
    /// 检查节点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool CheckNode(SyntaxNode node)
    {
        if (CheckKind(node))
        {
            if (_isPartial && node is MemberDeclarationSyntax member)
                return member.Modifiers.IsPartial();
            return true;
        }
        
        return false;
    }
    /// <summary>
    /// 检查节点类型
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public virtual bool CheckKind(SyntaxNode node)
    {
        var kind = node.Kind();
        foreach (var item in _kinds)
        {
            if (item == kind)
                return true;
        }
        return false;
    }
}
