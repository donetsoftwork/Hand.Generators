using Hand.Builders;
using Hand.Entities;
using Hand.Members;
using Hand.Reflection;
using Hand.Words;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Hand.GeneratePoco;

/// <summary>
/// 按属性生成
/// </summary>
public class PocoPropertySource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder, TypeSymbolInfo typeInfo, TypeSymbolInfo sourseInfo
    , AttributeData attribute, InitializeKind initializer)
    : PocoSource(type, convertBuilder, typeInfo, sourseInfo, attribute)
{
    #region 配置
    private readonly bool _useInit = initializer.HasFlag(InitializeKind.Init);
    private readonly bool _useField = initializer.HasFlag(InitializeKind.Field);
    #endregion

    /// <inheritdoc />
    public override SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var generateArguments = new List<MemberArgument>(_sourceMembers.Count);
        var kind = CheckAccessorKind(_useInit);
        foreach (var item in _sourceMembers)
        {
            var name = item.Key;
            if (_memberNames.Contains(name))
                continue;
            var argument = _useField ? CheckFieldMember(builder, name, item.Value, kind) :
                CheckMember(builder, name, item.Value, kind);
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
        //// 设置Xml备注
        //builder.Apply(type => type.WithSummary(_summary));
        return builder;
    }    
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public MemberArgument CheckFieldMember(SyntaxGenerator builder, string name, SymbolMember sourseMember, SyntaxKind kind)
    {
        var (memberType, memberSymbolInfo) = CheckMemberType(_compilation, name, sourseMember.SymbolInfo);
        var fieldName = UnderWordRule.UnderLower(name);
        var fieldExpression = SyntaxFactory.IdentifierName(fieldName);
        var field = CreateField(memberType, fieldName, memberSymbolInfo)
            .Private();

        var getDeclaration = SyntaxGenerator.PropertyGetDeclaration(fieldExpression);
        var accessorDeclaration = SyntaxFactory.AccessorDeclaration(kind)
            .WithExpressionBody(SyntaxGenerator.ExpressionBody(fieldExpression.AssignValue()))
            .WithSemicolonToken();
        var property = memberType.Property(name, getDeclaration, accessorDeclaration)
            .Public();
        if (_generateAttribute)
            property = builder.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Property));
        var summary = sourseMember.Element;
        if (summary is not null)
            property = property.WithSummary(summary);
        builder.AddField(field);
        builder.AddProperty(property);
        string summaryFunc() => sourseMember.Summary;
        var fieldMember = new FieldDeclarationMember(fieldName, memberSymbolInfo, field, summaryFunc);
        var propertyMember = new PropertyDeclarationMember(memberSymbolInfo, property, summaryFunc);
        var reversedArgument = new MemberArgument(sourseMember, fieldMember);
        // 源成员映射到字段,需要手动反转为属性映射源成员
        return new MemberReversedArgument(propertyMember, sourseMember, reversedArgument);
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public MemberArgument CheckMember(SyntaxGenerator builder, string name, SymbolMember sourseMember, SyntaxKind kind)
    {
        var (memberType, memberSymbolInfo) = CheckMemberType(_compilation, name, sourseMember.SymbolInfo);
        var property = CreateProperty(memberType, name, kind, memberSymbolInfo)
            .Public();
        if (_generateAttribute)
            property = builder.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Property));
        var summary = sourseMember.Element;
        if (summary is not null)
            property = property.WithSummary(summary);
        builder.AddProperty(property);

        var member = new PropertyDeclarationMember(memberSymbolInfo, property, () => sourseMember.Summary);
        return new(member, sourseMember);
    }
    /// <summary>
    /// 构造属性
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="kind"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public PropertyDeclarationSyntax CreateProperty(TypeSyntax type, string name, SyntaxKind kind, TypeSymbolInfo info)
    {
        var property = type.Property(name, SyntaxKind.GetAccessorDeclaration, kind);
        return _useDefault ? property.WithInitializer(DefaultExpressionBuilder.Default(info, _compilation)).WithSemicolonToken() : property;
    }
    /// <summary>
    /// 属性操作器种类
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    public static SyntaxKind CheckAccessorKind(bool init)
    {
        if (init)
            return SyntaxKind.InitAccessorDeclaration;
        return SyntaxKind.SetAccessorDeclaration;
    }
}
