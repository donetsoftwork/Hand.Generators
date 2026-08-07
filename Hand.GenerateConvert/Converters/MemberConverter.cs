using Hand.Members;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 按成员转化器
/// </summary>
/// <param name="memberName"></param>
public class MemberConverter(SimpleNameSyntax memberName)
     : InstanceMember(memberName), IConverter
{
    /// <summary>
    /// 按成员转化器
    /// </summary>
    /// <param name="memberName"></param>
    public MemberConverter(string memberName)
        : this(SyntaxFactory.IdentifierName(memberName))
    {
    }
    /// <inheritdoc />
    public ExpressionSyntax Convert(ExpressionSyntax source)
        => source.Access(_memberName);
}
