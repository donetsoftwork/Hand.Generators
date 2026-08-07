using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Converters;

/// <summary>
/// 带参数构造函数转化类型
/// </summary>
/// <param name="targetType"></param>
/// <param name="otherArguments"></param>
public class ArgumentConstructorConverter(TypeSyntax targetType, IEnumerable<ArgumentSyntax> otherArguments)
    : ConstructorConverter(targetType)
{
    /// <summary>
    /// 带参数构造函数转化类型
    /// </summary>
    /// <param name="targetType"></param>
    /// <param name="otherArguments"></param>
    public ArgumentConstructorConverter(TypeSyntax targetType, params IEnumerable<ExpressionSyntax> otherArguments)
        : this(targetType, otherArguments.Select(SyntaxFactory.Argument))
    {
    }
    #region 配置
    private readonly IEnumerable<ArgumentSyntax> _otherArguments = otherArguments;
    /// <summary>
    /// 其他参数
    /// </summary>
    public IEnumerable<ArgumentSyntax> OtherArguments
        => _otherArguments;
    #endregion
    /// <inheritdoc />
    protected override IEnumerable<ArgumentSyntax> CreateArguments(ExpressionSyntax source)
        => [SyntaxFactory.Argument(source), .. _otherArguments];
}
