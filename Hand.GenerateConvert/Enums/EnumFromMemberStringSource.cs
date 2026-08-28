using Hand.Builders;
using Hand.Methods;
using Hand.Sources;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enums;

/// <summary>
/// 特殊枚举成员转string
/// </summary>
/// <param name="compilation">编译信息</param>
/// <param name="enumInfo">枚举信息</param>
/// <param name="methodName">方法名</param>
/// <param name="members">特殊枚举成员</param>
public class EnumFromMemberStringSource(Compilation compilation, EnumTypeInfo enumInfo, string methodName, IEnumField[] members)
    : MethodSource(compilation, methodName, SyntaxGenerator.StringType, enumInfo)
{
    #region 配置
    private readonly IEnumField[] _members = members;

    /// <summary>
    /// 特殊枚举成员
    /// </summary>
    public IEnumField[] Members
        => _members;
    #endregion

    /// <inheritdoc />
    public override MethodDeclarationSyntax BuildBody(SyntaxGenerator generator, MethodDeclarationSyntax method, ExpressionSyntax @this)
    {
        return BuildBody(generator, method.ToBuilder(), @this)
            .WithSummary(ConvertBuilder.GetMethodSummary(_returnInfo));
    }
    /// <summary>
    /// 构造方法主体
    /// </summary>
    /// <typeparam name="TMethod"></typeparam>
    /// <param name="generator"></param>
    /// <param name="builder"></param>
    /// <param name="this"></param>
    public TMethod BuildBody<TMethod>(SyntaxGenerator generator, BodyBuilder<TMethod> builder, ExpressionSyntax @this)
    {
        var enumType = generator.Display(_returnInfo);
        var parseConverter = new EnumParseConverter(enumType, true);
        foreach (var member in _members)
        {
            //if (string.Equals(this, member.Member, System.StringComparison.OrdinalIgnoreCase‌))
            //    return member.Expression;
            builder.If(StringCompareMethods.Equals(@this, SyntaxGenerator.Literal(member.Member), StringCompareMethods.OrdinalIgnoreCase))
                .Return(member.GetExpression(enumType));
        }
        return builder.Return(parseConverter.Convert(generator, @this));
    }
}
