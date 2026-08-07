using Hand.Sources;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;

namespace Hand.Executors;

/// <summary>
/// 执行器基类
/// </summary>
/// <typeparam name="TSource"></typeparam>
public class GeneratorExecutor<TSource>
    : IGeneratorExecutor<TSource>, IGeneratorExecutor<IEnumerable<TSource>>
    where TSource : IGeneratorSource
{
    /// <inheritdoc />
    public virtual void Execute(SourceProductionContext context, TSource source)
    {
        var cancellation = context.CancellationToken;
        if (cancellation.IsCancellationRequested)
            return;
        //#if DEBUG
        //        System.Diagnostics.Debugger.Launch();
        //#endif
        var builder = source.Generate();
        var unit = builder.Build()
            .WithGenerated();
        context.AddSource(source.GenerateFileName, unit.GetText(Encoding.UTF8));
    }
    /// <inheritdoc />
    public virtual void Execute(SourceProductionContext context, IEnumerable<TSource> source)
    {
        foreach ( var item in source )
            Execute(context, item);
    }
}
