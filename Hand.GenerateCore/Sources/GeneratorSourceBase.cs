using Microsoft.CodeAnalysis;
using System.Text;

namespace Hand.Sources;

/// <summary>
/// 生成器数据源基类
/// </summary>
public abstract class GeneratorSourceBase : IGeneratorSource
{
    /// <inheritdoc />
    public abstract string GenerateFileName { get; }

    /// <inheritdoc />
    public abstract SyntaxGenerator Generate();
    /// <inheritdoc />
    public virtual void Generate(SourceProductionContext context)
        => Generate(context, this);

    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="context"></param>
    /// <param name="source"></param>
    public static void Generate(SourceProductionContext context, GeneratorSourceBase source)
    {
        var cancellation = context.CancellationToken;
        if (cancellation.IsCancellationRequested)
            return;
        var builder = source.Generate();
        var unit = builder.BuildUnit()
            .WithGenerated()
            .NormalizeWhitespace();
        context.AddSource(source.GenerateFileName, unit.GetText(Encoding.UTF8));
    }
}
