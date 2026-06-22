using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Converters;

/// <summary>
/// 带参数构造函数转化类型
/// </summary>
/// <param name="targetType"></param>
/// <param name="otherArguments"></param>
public class ArgumentConstructorConverter(TypeSyntax targetType, params ExpressionSyntax[] otherArguments)
    : ConstructorConverter(targetType)
{
    #region 配置
    private readonly ExpressionSyntax[] _otherArguments = otherArguments;
    /// <summary>
    /// 其他参数
    /// </summary>
    public ExpressionSyntax[] OtherArguments
        => _otherArguments;
    #endregion
    /// <inheritdoc />
    protected override IEnumerable<ExpressionSyntax> CreateArguments(ExpressionSyntax source)
        => [source, .. _otherArguments];
}
