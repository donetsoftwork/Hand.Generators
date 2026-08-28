using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 跳过转化
/// </summary>
public class PassConverter(ExpressionSyntax defaultExpression, bool isNullable = false)
    : IConverter
{
    #region 配置
    private readonly ExpressionSyntax _defaultExpression = defaultExpression;
    private readonly bool _isNullable = isNullable;
    /// <summary>
    /// 默认表达式
    /// </summary>
    public ExpressionSyntax DefaultExpression
        => _defaultExpression;
    /// <summary>
    /// 是否可空
    /// </summary>
    public bool IsNullable
        => _isNullable;
    #endregion
    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        if (_isNullable)
            return source.NullCoalesce(_defaultExpression);

        return source;
    }
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly PassConverter Default = new(SyntaxGenerator.DefaultLiteral, false);
}
