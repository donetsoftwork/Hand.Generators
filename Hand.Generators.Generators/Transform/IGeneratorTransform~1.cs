using Hand.Generators;
using System.Threading;

namespace Hand.Transform;

/// <summary>
/// 转化
/// </summary>
/// <typeparam name="TSource"></typeparam>
public interface IGeneratorTransform<TSource>
{
    /// <summary>
    /// 转化
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    TSource? Transform(AttributeContext context, CancellationToken cancellation = default);
}
