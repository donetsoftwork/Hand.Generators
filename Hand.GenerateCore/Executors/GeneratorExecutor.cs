using Hand.Sources;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Hand.Executors;

/// <summary>
/// 执行器
/// </summary>
public class GeneratorExecutor : IGeneratorExecutor<IGeneratorSource>
{
    /// <inheritdoc />
    public void Execute(SourceProductionContext context, IGeneratorSource source)
        => source.Generate(context);
}
