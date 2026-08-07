using Hand.Builders;
using Hand.Cachers;
using Hand.Members;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.GeneratePoco;

/// <summary>
/// 按record生成
/// </summary>
public class PocoRecordSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder
    , TypeSymbolInfo typeInfo, TypeSymbolInfo sourseInfo
    , AttributeData attribute)
    : PocoSource(type, convertBuilder, typeInfo, sourseInfo, attribute)
{
    /// <inheritdoc />
    public override SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var generateArguments = new List<MemberArgument>(_sourceMembers.Count);
        var comment = new Comment() { Summary = SummaryCacher.GetSummary(_typeSymbol, _typeSymbol.Name) };
        foreach (var item in _sourceMembers)
        {
            var name = item.Key;
            if (_memberNames.Contains(name))
                continue;
            var argument = CheckMember(builder, name, item.Value, comment);
            generateArguments.Add(argument);
        }        
        if (_convertTo)
        {
            var method = CheckConvertTo(_convertBuilder, generateArguments);
            if (method is not null)
                builder.AddMethod(method);
        }
        if (_convertFrom)
            CheckConvertFrom(_convertBuilder, generateArguments);
        // 设置Xml备注
        builder.Apply(type => type.WithComment(comment));
        return builder;
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    public MemberArgument CheckMember(SyntaxGenerator builder, string name, SymbolMember sourseMember, Comment comment)
    {
        var symbolInfo = sourseMember.SymbolInfo;
        var memberType = CheckMemberNullAble(name, symbolInfo.CheckPoco().ToSyntax());
        var parameter = CreateParameter(memberType, name, symbolInfo);
        if (_generateAttribute)
            parameter = builder.GenerateAttribute(parameter, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Parameter));
        comment.AddParam(name, sourseMember.Summary);
        builder.AddParameter(parameter);
        string summaryFunc() => sourseMember.Summary;
        var member = new ParameterSyntaxMember(name, parameter,symbolInfo, summaryFunc);
        return new(member, sourseMember);
    }
}
