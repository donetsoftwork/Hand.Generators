using Hand.Executors;
using Hand.Filters;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;

namespace Hand.Generators;

/// <summary>
/// 生成代码基类
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <param name="attributeName"></param>
/// <param name="filter"></param>
/// <param name="transform"></param>
/// <param name="executor"></param>
public class CollectGenerator<TSource>(string attributeName, ISyntaxFilter filter, IGeneratorTransform<TSource> transform, IGeneratorExecutor<ImmutableArray<TSource>> executor)
    : AttributeGenerator<TSource>(attributeName, filter, transform)
{
    #region 配置
    private readonly IGeneratorExecutor<ImmutableArray<TSource>> _executor = executor;
    /// <summary>
    /// 执行
    /// </summary>
    public IGeneratorExecutor<ImmutableArray<TSource>> Executor
        => _executor;
    #endregion
    /// <inheritdoc />
    protected override void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<TSource> provider)
    {
        // 注册输出逻辑
        context.RegisterSourceOutput(provider.Collect(), Execute);
    }
    /// <summary>
    /// 统一异常处理
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    private void Execute(SourceProductionContext context, ImmutableArray<TSource> source)
    {
        try
        {
            _executor.Execute(context, source);
        }
        catch (Exception ex)
        {
            var descriptor = new DiagnosticDescriptor("HAND004", "CollectGenerator Error", $"An error occurred in the CollectGenerator: {ex.Message}", "CollectGenerator", DiagnosticSeverity.Error, true);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None));
        }
    }
}
