using Hand.Arguments;
using Hand.Builders;
using Hand.Fields;
using Hand.Members;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.GeneratePoco;

/// <summary>
/// 按属性生成
/// </summary>
public class PocoFieldSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder
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
        foreach (var item in sourceMembers)
        {
            var name = item.Key;
            // 判断属性是否重名
            if (_naming.TryDeclareProperty(name))
            {
                var argument = CheckMember(_generator, name, item.Value);
                arguments.Add(argument);
            }
        }
        CheckConvert(_generator, sourceMembers, arguments);
        return _generator;
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <returns></returns>
    public MemberArgument CheckMember(SyntaxGenerator generator, string name, IMemberInfo sourseMember)
    {
        var (memberType, symbolInfo) = CheckMemberType(name, sourseMember.SymbolInfo);
        var field = CreateField(memberType, name, symbolInfo)
            .Public();
        if (_generateAttribute)
            field = generator.GenerateAttribute(field, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Field));
        var summary = sourseMember.XmlElement;
        if (summary is not null)
            field = field.WithSummary(summary);
        generator.AddField(field);
        var member = new FieldDeclarationMember(name, symbolInfo, field, true, sourseMember.Summary);
        SourceMember(name, member);
        return new(member, sourseMember);
    }
    /// <summary>
    /// 属性操作器种类
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    public static SyntaxKind[] ChecAccessorKinds(bool init)
    {
        if (init)
            return [SyntaxKind.GetAccessorDeclaration, SyntaxKind.InitAccessorDeclaration];
        return [SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration];
    }
}
