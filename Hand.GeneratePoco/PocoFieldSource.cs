using Hand.Builders;
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
        var sourceMembers = ConvertBuilder.GetSourceMembers(_convertBuilder.TypeCacher, _sourseSymbol, _recognizers);
        var arguments = new List<MemberArgument>(sourceMembers.Count);
        foreach (var item in sourceMembers)
        {
            var name = item.Key;
            if (_memberNames.Contains(name))
                continue;
            var argument = CheckMember(_generator, name, item.Value);
            arguments.Add(argument);
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
    public MemberArgument CheckMember(SyntaxGenerator generator, string name, SymbolMember sourseMember)
    {
        var (memberType, memberSymbolInfo) = CheckMemberType(name, sourseMember.SymbolInfo);
        var field = CreateField(memberType, name, memberSymbolInfo)
            .Public();
        if (_generateAttribute)
            field = generator.GenerateAttribute(field, _attributeCacher.GetAttributes(sourseMember.Original, AttributeTargets.Field));
        var summary = sourseMember.Element;
        if (summary is not null)
            field = field.WithSummary(summary);
        generator.AddField(field);
        var member = new FieldDeclarationMember(name, memberSymbolInfo, field, () => sourseMember.Summary);
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
