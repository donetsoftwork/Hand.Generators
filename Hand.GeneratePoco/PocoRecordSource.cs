using Hand.Arguments;
using Hand.Builders;
using Hand.Cachers;
using Hand.Documentation;
using Hand.Members;
using Hand.Parameters;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.GeneratePoco;

/// <summary>
/// 按record生成
/// </summary>
public class PocoRecordSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder
    , ComplexTypeInfo typeInfo, ComplexTypeInfo sourseInfo
    , AttributeData attribute)
    : PocoSource(type, convertBuilder, typeInfo, sourseInfo, attribute)
{
    /// <inheritdoc />
    public override SyntaxGenerator Generate()
    {
        //var generator = SyntaxGenerator.Clone(_type);
        //var sourceMembers = ConvertBuilder.GetSourceMembers(_convertBuilder.TypeBuilder, _fromSymbol, _fromRecognizers);
        var sourceMembers = ConvertBuilder.Recognize(_fromSourceMembers, _fromRecognizers);
        var arguments = new List<MemberArgument>(sourceMembers.Count);
        var comment = new Comment() { Summary = SummaryCacher.GetSummary(_toSymbol, _toSymbol.Name) };
        foreach (var item in sourceMembers)
        {
            var name = item.Key;
            // 判断属性是否重名
            if (_naming.TryDeclareProperty(name))
            {
                var argument = CheckMember(_generator, name, item.Value, comment);
                arguments.Add(argument);
            }
        }
        CheckConvert(_generator, sourceMembers, arguments);
        // 设置Xml备注
        _generator.Apply(type => type.WithComment(comment));
        return _generator;
    }

    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    public MemberArgument CheckMember(SyntaxGenerator generator, string name, IMemberInfo sourseMember, Comment comment)
    {
        var (memberType, symbolInfo) = CheckMemberType(name, sourseMember.SymbolInfo);
        //var memberType = CheckMemberNullAble(name, symbolInfo.CheckPoco().ToSyntax());
        var parameter = CreateParameter(memberType, name, symbolInfo);
        if (_generateAttribute)
            parameter = generator.GenerateAttribute(parameter, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Parameter));
        comment.AddParam(name, sourseMember.Summary);
        generator.AddParameter(parameter);
        var member = new ParameterSyntaxMember(name, parameter, symbolInfo, sourseMember.Summary);
        return new(member, sourseMember);
    }
}
