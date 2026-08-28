using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Converters;

/// <summary>
/// 实例方法转化器
/// </summary>
/// <param name="methodName"></param>
/// <param name="arguments"></param>
public class ArgumentInstanceMethodConverter(SimpleNameSyntax methodName, IEnumerable<ArgumentSyntax> arguments)
     : InstanceMethodConverter(methodName)
{
    /// <summary>
    /// 实例方法转化器
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="arguments"></param>
    public ArgumentInstanceMethodConverter(SimpleNameSyntax methodName, IEnumerable<ExpressionSyntax> arguments)
        : this(methodName, arguments.Select(SyntaxFactory.Argument))
    {
    }
    #region 配置
    private readonly IEnumerable<ArgumentSyntax> _arguments = arguments;
    /// <summary>
    /// 参数
    /// </summary>
    public IEnumerable<ArgumentSyntax> Arguments
        => _arguments;
    #endregion
    /// <inheritdoc />
    protected override SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [.._arguments];
}
