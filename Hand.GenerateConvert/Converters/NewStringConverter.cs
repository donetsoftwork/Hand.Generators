using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 字符数组转字符串
/// </summary>
public sealed class NewStringConverter
    : IConverter
{
    #region 配置
    private static readonly TypeSyntax _targetType = SyntaxFactory.IdentifierName("String");

    /// <summary>
    /// 目标类型
    /// </summary>
    public static TypeSyntax TargetType
        => _targetType;
    #endregion

    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        generator.UsingSystem();
        return _targetType.New([SyntaxFactory.Argument(source)]);
    }
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly NewStringConverter Instance = new();
}
