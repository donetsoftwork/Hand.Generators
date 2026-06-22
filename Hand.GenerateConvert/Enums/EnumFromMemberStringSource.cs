using Hand.Builders;
using Hand.Extensions;
using Hand.Members;
using Hand.Methods;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enums;

/// <summary>
/// 特殊枚举成员转string
/// </summary>
/// <param name="compilation">编译信息</param>
/// <param name="enumType">枚举类</param>
/// <param name="extensionInfo">扩展类信息</param>
/// <param name="methodName">方法名</param>
/// <param name="members">特殊枚举成员</param>
public class EnumFromMemberStringSource(Compilation compilation, TypeSyntax enumType, TypeNameInfo extensionInfo, string methodName, IEnumField[] members)
    : ExtensionSource(compilation, extensionInfo, true, methodName, SyntaxGenerator.StringType, enumType)
{
    #region 配置
    private readonly EnumParseConverter _parseConverter = new(enumType, true);
    private readonly IEnumField[] _members = members;

    /// <summary>
    /// 特殊枚举成员
    /// </summary>
    public IEnumField[] Members
        => _members;
    #endregion

    /// <inheritdoc />
    protected override MethodDeclarationSyntax BuildBody(MethodBodyBuilder<MethodDeclarationSyntax> builder, ExpressionSyntax @this)
    {
        foreach (var member in _members)
        {
            //if (string.Equals(this, member.Member, System.StringComparison.OrdinalIgnoreCase‌))
            //    return member.Expression;
            builder.If(StringCompareMethods.Equals(@this, SyntaxGenerator.Literal(member.Member), StringCompareMethods.OrdinalIgnoreCase))
                .Return(member.GetExpression(_returnType));
        }
        return builder.Return(_parseConverter.Convert(@this));
    }
}
