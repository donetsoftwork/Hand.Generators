using Hand.Members;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 按成员转化器
/// </summary>
public class MemberConverter(SimpleNameSyntax memberName)
     : InstanceMember(memberName), IConverter
{
    /// <inheritdoc />
    public ExpressionSyntax Convert(ExpressionSyntax source)
        => source.Access(_memberName);
}
