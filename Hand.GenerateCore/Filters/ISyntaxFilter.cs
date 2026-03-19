using Microsoft.CodeAnalysis;
using System.Threading;

namespace Hand.Filters;

/// <summary>
/// 节点过滤器
/// </summary>
public interface ISyntaxFilter
{
    /// <summary>
    /// 匹配
    /// </summary>
    /// <param name="node"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    bool Match(SyntaxNode node, CancellationToken cancellation);
}
