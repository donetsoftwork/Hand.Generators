using Hand.Sources;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Hand.Executors;

/// <summary>
/// 执行器基类
/// </summary>
/// <typeparam name="TSource"></typeparam>
public class GeneratorExecutor<TSource> : IGeneratorExecutor<TSource>
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
        var compilationUnit = builder.Build()
            .WithGenerated();
        context.AddSource(source.GenerateFileName, compilationUnit.GetText(Encoding.UTF8));
    }
}
