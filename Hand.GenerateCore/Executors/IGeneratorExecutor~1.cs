using Microsoft.CodeAnalysis;

namespace Hand.Executors;

/// <summary>
/// 执行器
/// </summary>
public interface IGeneratorExecutor<in TSource>
{
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    void Execute(SourceProductionContext context, TSource source);
}
