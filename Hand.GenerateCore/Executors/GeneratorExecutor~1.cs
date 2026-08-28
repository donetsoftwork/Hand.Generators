using Hand.Sources;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Hand.Executors;

/// <summary>
/// 执行器基类
/// </summary>
/// <typeparam name="TSource"></typeparam>
public class GeneratorExecutor<TSource>
    : IGeneratorExecutor<TSource>, IGeneratorExecutor<TSource[]>
    where TSource : IGeneratorSource
{
    /// <inheritdoc />
    public virtual void Execute(SourceProductionContext context, TSource source)
    {
        //#if DEBUG
        //        System.Diagnostics.Debugger.Launch();
        //#endif
        Generate(context, source);
    }
    /// <inheritdoc />
    public virtual void Execute(SourceProductionContext context, TSource[] source)
    {
        foreach ( var item in source )
            Execute(context, item);
    }
    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    public static void Generate(SourceProductionContext context, IGeneratorSource source)
    {
        var cancellation = context.CancellationToken;
        if (cancellation.IsCancellationRequested)
            return;
        var builder = source.Generate();
        var unit = builder.Build()
            .WithGenerated();
        context.AddSource(source.GenerateFileName, unit.GetText(Encoding.UTF8));
    }
}
