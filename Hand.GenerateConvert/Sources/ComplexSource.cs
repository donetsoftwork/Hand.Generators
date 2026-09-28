using Hand.Arguments;
using Hand.Builders;
using Hand.Members;
using Hand.Parameters;
using Hand.Syntax;
using Hand.Types;
using Hand.Words;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Sources;

/// <summary>
/// 复杂转化器
/// </summary>
/// <param name="builder"></param>
/// <param name="thisType"></param>
/// <param name="returnInfo"></param>
/// <param name="methodName"></param>
/// <param name="arguments"></param>
public class ComplexSource(ConvertBuilder builder, TypeSyntax thisType, ComplexTypeInfo returnInfo, string methodName, MemberArgument[] arguments)
    : MethodSource(builder.Compilation, methodName, thisType, returnInfo)
{
    /// <summary>
    /// 复杂转化器
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="thisType"></param>
    /// <param name="returnInfo"></param>
    /// <param name="methodName"></param>
    /// <param name="arguments"></param>
    public ComplexSource(ConvertBuilder builder, string thisType, ComplexTypeInfo returnInfo, string methodName, MemberArgument[] arguments)
        : this(builder, SyntaxFactory.IdentifierName(thisType), returnInfo, methodName, arguments)
    {
    }
    #region 配置
    private readonly ConvertBuilder _builder = builder;
    private new readonly ComplexTypeInfo _returnInfo = returnInfo;
    private readonly MemberArgument[] _arguments = arguments;
    #endregion

    /// <summary>
    /// 检查默认值
    /// </summary>
    /// <param name="info"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public static ExpressionSyntax CheckDefault(ITypeSymbolInfo info, IParameterSymbol parameter)
    {
        if (parameter.HasExplicitDefaultValue)
        {
            var value = parameter.ExplicitDefaultValue;
            if (value is not null)
                return SyntaxGenerator.Literal(info.Symbol.SpecialType, value);
        }
        return DefaultExpressionBuilder.GetParameterDefault(info);
    }
    /// <inheritdoc />
    public override MethodDeclarationSyntax BuildBody(SyntaxGenerator generator, MethodDeclarationSyntax method, ExpressionSyntax @this)
    {
        var parameters = new List<ParameterSyntax>();
        var builder = new CreationBuilder();
        var comment = new Comment() { Summary = ConvertBuilder.GetMethodSummary(_returnInfo) };
        foreach (var argument in _arguments)
        {
            var member = argument.Member;
            var memberInfo = member.SymbolInfo;
            var sourceMember = argument.Source;
            ExpressionSyntax memberValue;
            if (sourceMember is null || (_builder.Get(sourceMember.SymbolInfo, memberInfo) is not ISyntaxConverter memberConverter))
            {
                string parameterName;
                ExpressionSyntax defaultValue;
                if (member.Kind == MemberKind.Parameter && member is ParameterMember parameterMember)
                {
                    var parameter = parameterMember.Original;
                    defaultValue = CheckDefault(memberInfo, parameter);
                    parameterName = CamelWordRule.FistToLower(parameter.Name);
                    memberValue = SyntaxFactory.IdentifierName(parameterName);
                    if (parameter.IsOptional)
                        builder.WithArgument(member.Name, memberValue);
                    else
                        builder.WithArgument(memberValue);
                }
                else if (member.Kind == MemberKind.ParameterDeclaration && member is ParameterSyntaxMember parameterDeclaration)
                {
                    defaultValue = DefaultExpressionBuilder.GetParameterDefault(memberInfo);
                    parameterName = CamelWordRule.FistToLower(parameterDeclaration.Original.Identifier.ValueText);
                    memberValue = SyntaxFactory.IdentifierName(parameterName);
                    builder.WithArgument(memberValue);
                }
                else
                {
                    continue;
                }
                // 增加参数
                parameters.Add(memberInfo.Display(generator).Parameter(parameterName, defaultValue));
                comment.AddParam(parameterName, member.Summary);
            }
            else 
            {
                memberValue = memberConverter.Convert(generator, @this.Access(sourceMember.Name));
                if (member.Kind == MemberKind.Parameter && member is ParameterMember parameterMember)
                {
                    var parameter = parameterMember.Original;
                    if (parameter.IsOptional)
                        builder.WithArgument(member.Name, memberValue);
                    else
                        builder.WithArgument(memberValue);
                }
                else if (member.Kind == MemberKind.ParameterDeclaration)
                {
                    builder.WithArgument(memberValue);
                }
                else
                {
                    builder.Initialize(member.Name, memberValue);
                }
            }
        }
        var body = builder.Build();
        return method.AddParameterListParameters([.. parameters])
            .WithExpressionBody(body)
            .WithComment(comment);
    }
}
