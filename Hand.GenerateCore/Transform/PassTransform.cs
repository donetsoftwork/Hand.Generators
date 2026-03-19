using Microsoft.CodeAnalysis;
using System.Threading;

namespace Hand.Transform;

/// <summary>
/// 跳过转化
/// </summary>
public class PassTransform : IGeneratorTransform<GeneratorAttributeSyntaxContext>
{
    private PassTransform() { }
    /// <inheritdoc />
    public GeneratorAttributeSyntaxContext Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellation)
        => context;
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly IGeneratorTransform<GeneratorAttributeSyntaxContext> Instance = new PassTransform();
}
