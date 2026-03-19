using Hand;
using Hand.Executors;
using Microsoft.CodeAnalysis;

namespace Hand.GenerateProperty;

/// <summary>
/// 实体属性执行器
/// </summary>
public class PropertyExecutor : IGeneratorExecutor<PropertySource>
{
    /// <inheritdoc />
    public void Execute(SourceProductionContext context, PropertySource source)
    {
        var cancellation = context.CancellationToken;
        if (cancellation.IsCancellationRequested) 
            return;
        //#if DEBUG
        //        System.Diagnostics.Debugger.Launch();
        //#endif
        var builder = source.Generate();
        var code = builder.Build()
            .WithGenerated()
            .ToFullString();
        context.AddSource(source.GenerateFileName, code);
    }    
}
