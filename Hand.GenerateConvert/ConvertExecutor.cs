using Hand.Executors;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand;

/// <summary>
/// 转化执行器
/// </summary>
public class ConvertExecutor : GeneratorExecutor<ConvertToSource>
{
    /// <inheritdoc />
    public override void Execute(SourceProductionContext context, ConvertToSource source)
    {
        var builder = source.Builder;
        if (source.Methods.Length > 0)
            Generate(context, source);
        List<IGeneratorSource> items;
        // 在执行过程中可能生成新的生成源
        while ((items = [.. builder.Sources]).Count > 0)
        {
            builder.ClearSource();
            foreach (var item in items)
                Generate(context, item);
        }
    }
}
