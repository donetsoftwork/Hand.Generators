using Hand.Builders;
using Hand.Members;
using Hand.Reflection;
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
    , TypeSymbolInfo typeInfo, TypeSymbolInfo sourseInfo
    , AttributeData attribute)
    : PocoSource(type, convertBuilder, typeInfo, sourseInfo, attribute)
{
    /// <inheritdoc />
    public override SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var generateArguments = new List<MemberArgument>(_sourceMembers.Count);
        foreach (var item in _sourceMembers)
        {
            var name = item.Key;
            if (_memberNames.Contains(name))
                continue;
            var argument = CheckMember(builder, name, item.Value);
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
        return builder;
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <returns></returns>
    public MemberArgument CheckMember(SyntaxGenerator builder, string name, SymbolMember sourseMember)
    {
        var (memberType, memberSymbolInfo) = CheckMemberType(_compilation, name, sourseMember.SymbolInfo);
        var field = CreateField(memberType, name, memberSymbolInfo)
            .Public();
        if (_generateAttribute)
            field = builder.GenerateAttribute(field, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Field));
        var summary = sourseMember.Element;
        if (summary is not null)
            field = field.WithSummary(summary);
        builder.AddField(field);
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
