using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 兼容转化器
/// </summary>
/// <param name="compatible"></param>
/// <param name="original"></param>
public class CompatibleConverter(IConverter compatible, IConverter original)
    : IConverter
{
    #region 配置
    private readonly IConverter _compatible = compatible;
    private readonly IConverter _original = original;

    /// <summary>
    /// 兼容转化器
    /// </summary>
    public IConverter Compatible
        => _compatible;
    /// <summary>
    /// 原始转化器
    /// </summary>
    public IConverter Original
        => _original;
    #endregion
    /// <inheritdoc />
    public ExpressionSyntax Convert(ExpressionSyntax source)
        => _original.Convert(_compatible.Convert(source));
}
