using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 可空合并(替换)转化器
/// </summary>
/// <param name="original"></param>
/// <param name="defaultExpression"></param>
public class NullableConverter(IConverter original, ExpressionSyntax defaultExpression)
    : IConverter
{
    /// <summary>
    /// 可空转化器
    /// </summary>
    /// <param name="original"></param>
    public NullableConverter(IConverter original)
        : this(original, SyntaxGenerator.DefaultLiteral)
    {
    }
    #region 配置
    private readonly IConverter _original = original;
    private readonly ExpressionSyntax _defaultExpression = defaultExpression;

    /// <summary>
    /// 原始转化器
    /// </summary>
    public IConverter Original
        => _original;
    /// <summary>
    /// 默认值
    /// </summary>
    public ExpressionSyntax DefaultExpression
        => _defaultExpression;
    #endregion
    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => source.IsNull()
        .Conditional(_defaultExpression, _original.Convert(generator, source));
    ///// <inheritdoc />
    //public IConverter Nullable(ExpressionSyntax? defaultExpression)
    //    => new NullCoalesceConverter(_original, defaultExpression ?? _defaultExpression);
    ///// <summary>
    ///// 检查可空
    ///// </summary>
    ///// <param name="sourceNullable"></param>
    ///// <param name="targetNullable"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static bool CheckNullable(bool sourceNullable, bool targetNullable)
    //    => sourceNullable && !targetNullable;
    ///// <summary>
    ///// 检查可空
    ///// </summary>
    ///// <param name="original"></param>
    ///// <param name="sourceNullable"></param>
    ///// <param name="targetNullable"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static IConverter CheckNullable(IConverter original, bool sourceNullable, bool targetNullable)
    //    => CheckNullable(sourceNullable, targetNullable) ? new NullCoalesceConverter(original) : original;
    ///// <summary>
    ///// 检查可空
    ///// </summary>
    ///// <param name="original"></param>
    ///// <param name="nullable"></param>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static IConverter CheckNullable(IConverter original, bool nullable)
    //    => nullable ? new NullCoalesceConverter(original) : original;
}
