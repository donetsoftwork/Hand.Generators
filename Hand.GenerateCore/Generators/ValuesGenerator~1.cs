using Hand.Executors;
using Hand.Filters;
using Hand.Transform;
using Microsoft.CodeAnalysis;

namespace Hand.Generators;

/// <summary>
/// 生成代码基类
/// </summary>
/// <typeparam name="TSource"></typeparam>
/// <param name="attributeName"></param>
/// <param name="filter"></param>
/// <param name="transform"></param>
/// <param name="executor"></param>
public class ValuesGenerator<TSource>(string attributeName, ISyntaxFilter filter, IGeneratorTransform<TSource> transform, IGeneratorExecutor<TSource> executor)
    : AttributeGenerator<TSource>(attributeName, filter, transform)
{
    #region 配置
    private readonly IGeneratorExecutor<TSource> _executor = executor;
    /// <summary>
    /// 执行
    /// </summary>
    public IGeneratorExecutor<TSource> Executor 
        => _executor;
    #endregion
    /// <inheritdoc />
    protected override void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<TSource> provider)
    {
        // 注册输出逻辑
        context.RegisterSourceOutput(provider, _executor.Execute);
    }
}
