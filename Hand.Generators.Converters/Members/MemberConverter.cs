using Hand.Members;
using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Members;

/// <summary>
/// 按成员转化器
/// </summary>
/// <param name="memberName"></param>
public class MemberConverter(SimpleNameSyntax memberName)
     : InstanceMember(memberName), ISyntaxConverter
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
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => source.Access(_memberName);

    /// <summary>
    /// 转化为Original属性
    /// </summary>
    public static MemberConverter Original
        => Inner.Original;

    /// <summary>
    /// 内部延迟加载
    /// </summary>
    class Inner
    {
        /// <summary>
        /// 转化为Original属性
        /// </summary>
        public static readonly MemberConverter Original = new(SyntaxFactory.IdentifierName("Original"));
    }
}
