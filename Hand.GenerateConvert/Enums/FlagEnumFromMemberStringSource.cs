using Hand.Builders;
using Hand.Extensions;
using Hand.Members;
using Hand.Methods;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.Enums;

/// <summary>
/// 特殊枚举成员转string
/// </summary>
/// <param name="compilation">编译信息</param>
/// <param name="enumType">枚举类</param>
/// <param name="extensionInfo">扩展类信息</param>
/// <param name="methodName">方法名</param>
/// <param name="fields">特殊枚举成员</param>
public class FlagEnumFromMemberStringSource(Compilation compilation, TypeSyntax enumType, TypeNameInfo extensionInfo, string methodName, FlagEnumField[] fields)
    : ExtensionSource(compilation, extensionInfo, true, methodName, SyntaxGenerator.StringType, enumType)
{
    #region 配置
    private readonly FlagEnumField[] _fields = fields;

    /// <summary>
    /// 枚举成员
    /// </summary>
    public FlagEnumField[] Field
        => _fields;
    #endregion

    /// <inheritdoc />
    protected override MethodDeclarationSyntax BuildBody(MethodBodyBuilder<MethodDeclarationSyntax> builder, ExpressionSyntax @this)
    {
        var result = SyntaxFactory.IdentifierName("result");
        // TEnum result = default;
        builder.Declare(_returnType.Variable(result.Identifier, SyntaxGenerator.DefaultLiteral));
        var memberCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var members = new List<IEnumField>(_fields.Length);
        foreach (var field in _fields)
        {
            var name = field.Name;
            memberCheck.Add(name);
            //if (string.Equals(this, member.Name, System.StringComparison.OrdinalIgnoreCase‌))
            //    result |= field.Expression;
            builder.If(StringCompareMethods.Equals(@this, SyntaxGenerator.Literal(name), StringCompareMethods.OrdinalIgnoreCase))
                .AddPatter(result.OrAssign(field.GetExpression(_returnType)))
                .End();
            var member = field.Member;
            if (string.IsNullOrWhiteSpace(member) || memberCheck.Contains(member))
                continue;
            members.Add(field);
        }
        foreach (var field in members)
        {
            var member = field.Member;
            if (memberCheck.Contains(member))
                continue;
            memberCheck.Add(member);
            //if (string.Equals(this, member.Member, System.StringComparison.OrdinalIgnoreCase‌))
            //    result |= field.Expression;
            builder.If(StringCompareMethods.Equals(@this, SyntaxGenerator.Literal(member), StringCompareMethods.OrdinalIgnoreCase))
                .AddPatter(result.OrAssign(field.GetExpression(_returnType)))
                .End();
        }
        // return result;
        return builder.Return(result);
    }
}
