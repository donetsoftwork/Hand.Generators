using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 可空合并(替换)转化器
/// </summary>
/// <param name="original"></param>
/// <param name="defaultExpression"></param>
public class NullableConverter(ISyntaxConverter original, ExpressionSyntax defaultExpression)
    : ISyntaxConverter
{
    /// <summary>
    /// 可空转化器
    /// </summary>
    /// <param name="original"></param>
    public NullableConverter(ISyntaxConverter original)
        : this(original, SyntaxGenerator.DefaultLiteral)
    {
    }
    #region 配置
    private readonly ISyntaxConverter _original = original;
    private readonly ExpressionSyntax _defaultExpression = defaultExpression;

    /// <summary>
    /// 原始转化器
    /// </summary>
    public ISyntaxConverter Original
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
}
