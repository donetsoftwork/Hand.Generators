using Hand.Members;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Converters;

/// <summary>
/// 实例方法转化器
/// </summary>
/// <param name="methodName"></param>
public class InstanceMethodConverter(SimpleNameSyntax methodName)
     : InstanceMember(methodName), IConverter
{
    /// <inheritdoc />
    public ExpressionSyntax Convert(ExpressionSyntax source)
    {
        return GetMethod(source)
            .Invocation(CreateArguments());
    }
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <returns></returns>
    protected virtual ExpressionSyntax GetMethod(ExpressionSyntax source)
        => source.Access(_memberName);
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<ExpressionSyntax> CreateArguments()
        => [];
}
