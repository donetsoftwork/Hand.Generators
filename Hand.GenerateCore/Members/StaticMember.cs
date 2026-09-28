using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Members;

/// <summary>
/// 静态成员
/// </summary>
/// <param name="type"></param>
/// <param name="memberName"></param>
public class StaticMember(ISyntaxDisplay<TypeSyntax> type, SimpleNameSyntax memberName)
    : ISyntaxDisplay<ExpressionSyntax>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="memberName"></param>
    public StaticMember(ISyntaxDisplay<TypeSyntax> type, string memberName)
        : this(type , SyntaxFactory.IdentifierName(memberName))
    {
    }
    /// <inheritdoc />
    public ExpressionSyntax Display(SyntaxGenerator generator)
        => type.Display(generator).Access(memberName);
}
