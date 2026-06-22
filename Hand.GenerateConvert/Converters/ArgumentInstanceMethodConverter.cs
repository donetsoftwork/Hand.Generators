using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Converters;

/// <summary>
/// 实例方法转化器
/// </summary>
/// <param name="methodName"></param>
/// <param name="arguments"></param>
public class ArgumentInstanceMethodConverter(SimpleNameSyntax methodName, params ExpressionSyntax[] arguments)
     : InstanceMethodConverter(methodName)
{
    #region 配置
    private readonly ExpressionSyntax[] _arguments = arguments;
    /// <summary>
    /// 参数
    /// </summary>
    public ExpressionSyntax[] Arguments
        => _arguments;
    #endregion
    /// <inheritdoc />
    protected override IEnumerable<ExpressionSyntax> CreateArguments()
        => _arguments;
}
