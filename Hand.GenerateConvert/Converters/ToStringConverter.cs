using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 字符串转化器
/// </summary>
public class ToStringConverter()
    : InstanceMethodConverter(_toString)
{
    #region 配置
    private static readonly SimpleNameSyntax _toString = SyntaxFactory.IdentifierName("ToString");
    #endregion
    ///// <summary>
    ///// 使用指定方法转化
    ///// </summary>
    ///// <param name="source"></param>
    ///// <param name="isNullable"></param>
    ///// <returns>调用ToString转化为字符串</returns>
    //public static ExpressionSyntax Convert(ExpressionSyntax source, bool isNullable)
    //    => GetMember(source, isNullable, _toString).Invocation();
    ///// <inheritdoc />
    //public ExpressionSyntax Convert(ExpressionSyntax source)
    //{
    //    if (_isNullable)
    //        return source.ConditionalAccess(_memberName)
    //            .Invocation()
    //            .NullCoalesce(_defaultExpression);

    //    return source.Access(_memberName)
    //        .Invocation();
    //}
    ///// <inheritdoc />
    //public IConverter Nullable(ExpressionSyntax? defaultExpression)
    //    => new ToStringConverter(true)
    //    .CheckNull(defaultExpression);
    /// <summary>
    /// 单例
    /// </summary>
    public static readonly ToStringConverter Instance = new();
}
