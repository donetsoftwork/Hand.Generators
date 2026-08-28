using Hand.Builders;
using Hand.Entities;
using Hand.Members;
using Hand.Types;
using Hand.Words;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.GeneratePoco;

/// <summary>
/// 生成构造函数
/// </summary>
public class PocoConstructorSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder, ComplexTypeInfo typeInfo, ComplexTypeInfo sourseInfo
    , AttributeData attribute, InitializeKind initializer)
    : PocoSource(type, convertBuilder, typeInfo, sourseInfo, attribute)
{
    #region 配置
    private readonly bool _useField = initializer.HasFlag(InitializeKind.Field);
    private readonly bool _setProperty = initializer.HasFlag(InitializeKind.Property);
    #endregion
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
            var argument = _useField ? CheckFieldMember(_generator, name, item.Value) : 
                CheckMember(_generator, name, item.Value);
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
        var parameterName = CamelWordRule.FistToLower(name);
        var parameter = CreateParameter(memberType, parameterName, memberSymbolInfo);

        var property = CreatePropertyByParameter(memberType, name, SyntaxFactory.IdentifierName(parameterName));
        var summary = sourseMember.Element;
        if (summary is not null)
            property = property.WithSummary(summary);
        if (_generateAttribute)
        {
            parameter = generator.GenerateAttribute(parameter, _attributeCacher.GetAttributes(sourseMember.Original, AttributeTargets.Parameter));
            property = generator.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember.Original, AttributeTargets.Property));
        }
        generator.AddParameter(parameter);
        generator.AddProperty(property);
        string summaryFunc() => sourseMember.Summary;
        var parameterMember = new ParameterSyntaxMember(parameterName, parameter, memberSymbolInfo, summaryFunc);
        var propertyMember = new PropertyDeclarationMember(memberSymbolInfo, property, summaryFunc);
        var reversedArgument = new MemberArgument(sourseMember, propertyMember);
        // 源成员映射到参数,需要手动反转为属性映射源成员
        return new MemberReversedArgument(parameterMember, sourseMember, reversedArgument);
    }
    /// <summary>
    /// 使用参数构造属性
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public PropertyDeclarationSyntax CreatePropertyByParameter(TypeSyntax type, string name, IdentifierNameSyntax parameter)
    {
        var property = _setProperty ? type.GetSetProperty(name) : 
            type.GetOnlyProperty(name);
        return property.WithInitializer(parameter)
            .Public()
            .WithSemicolonToken();
    }
    /// <summary>
    /// 构造字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public FieldDeclarationSyntax CreateField(TypeSyntax type, string name, IdentifierNameSyntax parameter)
    {
        var field = type.Field(name, parameter).Private();
        return _setProperty ? field : field.ReadOnly();
    }
    /// <summary>
    /// 使用字段构造属性
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public PropertyDeclarationSyntax CreatePropertyByField(TypeSyntax type, string name, IdentifierNameSyntax field)
    {
        var property = _setProperty ? type.GetSetProperty(name, field) :
            type.Property(name, field);
        return property.Public();
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <returns></returns>
    public MemberArgument CheckFieldMember(SyntaxGenerator generator, string name, SymbolMember sourseMember)
    {
        var (memberType, memberSymbolInfo) = CheckMemberType(name, sourseMember.SymbolInfo);
        var parameterName = CamelWordRule.FistToLower(name);
        var parameter = memberType.Parameter(parameterName);
        var fieldName = UnderWordRule.UnderLower(name);
        var field = CreateField(memberType, fieldName, SyntaxFactory.IdentifierName(parameterName));
        var property = CreatePropertyByField(memberType, name, SyntaxFactory.IdentifierName(fieldName));
        var summary = sourseMember.Element;
        if (summary is not null)
            property = property.WithSummary(summary);
        if (_generateAttribute)
        {
            parameter = generator.GenerateAttribute(parameter, _attributeCacher.GetAttributes(sourseMember.Original, AttributeTargets.Parameter));
            property = generator.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember.Original, AttributeTargets.Property));
        }
        generator.AddParameter(parameter);
        generator.AddField(field);
        generator.AddProperty(property);
        string summaryFunc() => sourseMember.Summary;
        var parameterMember = new ParameterSyntaxMember(parameterName, parameter, memberSymbolInfo, summaryFunc);
        var fieldMember = new FieldDeclarationMember(fieldName, memberSymbolInfo, field, summaryFunc);
        //var propertyMember = new PropertyDeclarationMember(memberSymbolInfo, property);
        var reversedArgument = new MemberArgument(sourseMember, fieldMember);
        // 源成员映射到参数,需要手动反转为属性映射源成员
        return new MemberReversedArgument(parameterMember, sourseMember, reversedArgument);
    }
}
