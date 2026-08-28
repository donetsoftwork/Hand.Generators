using Hand.Members;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 实例方法转化器
/// </summary>
/// <param name="methodName"></param>
/// <param name="isNullable"></param>
public class InstanceMethodConverter(SimpleNameSyntax methodName, bool isNullable = false)
     : InstanceMember(methodName, isNullable), IConverter
{
    /// <inheritdoc />
    public virtual ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        return GetMethod(generator, source)
            .Invocation(CreateArguments(generator, source));
    }
    /// <summary>
    /// 获取方法
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual ExpressionSyntax GetMethod(SyntaxGenerator generator, ExpressionSyntax source)
        => source.Access(_memberName);
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    protected virtual SeparatedSyntaxList<ArgumentSyntax> CreateArguments(SyntaxGenerator generator, ExpressionSyntax source)
        => [];
}
