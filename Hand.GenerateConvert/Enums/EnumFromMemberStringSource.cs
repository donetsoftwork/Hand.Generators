using Hand.Builders;
using Hand.Methods;
using Hand.Reflection;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enums;

/// <summary>
/// 特殊枚举成员转string
/// </summary>
/// <param name="compilation">编译信息</param>
/// <param name="enumInfo">枚举信息</param>
/// <param name="enumType">枚举类</param>
/// <param name="methodName">方法名</param>
/// <param name="members">特殊枚举成员</param>
public class EnumFromMemberStringSource(Compilation compilation, TypeSymbolInfo enumInfo, TypeSyntax enumType, string methodName, IEnumField[] members)
    : MethodSource(compilation, methodName, SyntaxGenerator.StringType, enumType)
{
    /// <summary>
    /// 特殊枚举成员转string
    /// </summary>
    /// <param name="compilation">编译信息</param>
    /// <param name="enumInfo">枚举信息</param>
    /// <param name="methodName">方法名</param>
    /// <param name="members">特殊枚举成员</param>
    public EnumFromMemberStringSource(Compilation compilation, TypeSymbolInfo enumInfo, string methodName, IEnumField[] members)
        : this(compilation, enumInfo, enumInfo.Symbol.ToSyntax(), methodName, members)
    {
    }
    #region 配置
    private readonly EnumParseConverter _parseConverter = new(enumType, true);
    private readonly TypeSymbolInfo _enumInfo = enumInfo;
    private readonly IEnumField[] _members = members;

    /// <summary>
    /// 特殊枚举成员
    /// </summary>
    public IEnumField[] Members
        => _members;
    #endregion

    /// <inheritdoc />
    public override MethodDeclarationSyntax BuildBody(MethodDeclarationSyntax method, ExpressionSyntax @this)
    {
        return BuildBody(method.ToBuilder(), @this)
            .WithSummary(ConvertBuilder.GetMethodSummary(_enumInfo));
    }
    /// <summary>
    /// 构造方法主体
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="builder"></param>
    /// <param name="this"></param>
    public TMethod BuildBody<TMethod>(BodyBuilder<TMethod> builder, ExpressionSyntax @this)
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
