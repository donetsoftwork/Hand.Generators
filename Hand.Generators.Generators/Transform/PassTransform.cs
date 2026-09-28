using Hand.Generators;
using System.Threading;

namespace Hand.Transform;

/// <summary>
/// 跳过转化
/// </summary>
public class PassTransform : IGeneratorTransform<AttributeContext>
{
    private PassTransform() { }
    /// <inheritdoc />
    public AttributeContext Transform(AttributeContext context, CancellationToken cancellation = default)
        => context;
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly IGeneratorTransform<AttributeContext> Instance = new PassTransform();
}
