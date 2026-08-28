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
/// 按属性生成
/// </summary>
public class PocoPropertySource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder, ComplexTypeInfo typeInfo, ComplexTypeInfo sourseInfo
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
        //var generator = SyntaxGenerator.Clone(_type);
        var sourceMembers = ConvertBuilder.GetSourceMembers(_convertBuilder.TypeCacher, _sourseSymbol, _recognizers);
        var arguments = new List<MemberArgument>(sourceMembers.Count);
        var kind = CheckAccessorKind(_useInit);
        foreach (var item in sourceMembers)
        {
            var name = item.Key;
            if (_memberNames.Contains(name))
                continue;
            var argument = _useField ? CheckFieldMember(_generator, name, item.Value, kind) :
                CheckMember(_generator, name, item.Value, kind);
            arguments.Add(argument);
        }
        CheckConvert(_generator, sourceMembers, arguments);
        //// 设置Xml备注
        //builder.Apply(type => type.WithSummary(_summary));
        return _generator;
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public MemberArgument CheckFieldMember(SyntaxGenerator generator, string name, SymbolMember sourseMember, SyntaxKind kind)
    {
        var (memberType, memberSymbolInfo) = CheckMemberType(name, sourseMember.SymbolInfo);
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
            property = generator.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember.Original, AttributeTargets.Property));
        var summary = sourseMember.Element;
        if (summary is not null)
            property = property.WithSummary(summary);
        generator.AddField(field);
        generator.AddProperty(property);
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
    /// <param name="generator"></param>
    /// <param name="name"></param>
    /// <param name="sourseMember"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public MemberArgument CheckMember(SyntaxGenerator generator, string name, SymbolMember sourseMember, SyntaxKind kind)
    {
        var (memberType, memberSymbolInfo) = CheckMemberType(name, sourseMember.SymbolInfo);
        var property = CreateProperty(memberType, name, kind, memberSymbolInfo)
            .Public();
        if (_generateAttribute)
            property = generator.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember.Original, AttributeTargets.Property));
        var summary = sourseMember.Element;
        if (summary is not null)
            property = property.WithSummary(summary);
        generator.AddProperty(property);

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
    public PropertyDeclarationSyntax CreateProperty(TypeSyntax type, string name, SyntaxKind kind, ITypeSymbolInfo info)
    {
        var property = type.Property(name, SyntaxKind.GetAccessorDeclaration, kind);
        if (_useDefault)
            return property.WithInitializer(DefaultExpressionBuilder.GetDefault(info)).WithSemicolonToken();
        return property;
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
