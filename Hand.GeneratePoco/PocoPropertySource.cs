using Hand.Arguments;
using Hand.Builders;
using Hand.Entities;
using Hand.Fields;
using Hand.Members;
using Hand.Properties;
using Hand.Types;
using Hand.Words;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

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
        //var sourceMembers = ConvertBuilder.GetSourceMembers(_convertBuilder.TypeBuilder, _fromSymbol, _fromRecognizers);
        var sourceMembers = ConvertBuilder.Recognize(_fromSourceMembers, _fromRecognizers);
        var arguments = new List<MemberArgument>(sourceMembers.Count);
        var kind = CheckAccessorKind(_useInit);
        foreach (var item in sourceMembers)
        {
            var name = item.Key;
            // 判断属性是否重名
            if (_naming.TryDeclareProperty(name))
            {
                var argument = CheckMember(_generator, name, item.Value, kind);
                arguments.Add(argument);
            }
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
    /// <param name="propertyName"></param>
    /// <param name="sourseMember"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public MemberArgument CheckMember(SyntaxGenerator generator, string propertyName, IMemberInfo sourseMember, SyntaxKind kind)
    {
        if (_useField)
        {
            var fieldName = UnderWordRule.UnderLower(propertyName);
            // 判断字段是否重名
            if (_naming.TryDeclareField(fieldName))
                return CheckFieldMember(generator, propertyName, fieldName, sourseMember, kind);
        }
        var (memberType, symbolInfo) = CheckMemberType(propertyName, sourseMember.SymbolInfo);
        var property = CreateProperty(memberType, propertyName, kind, symbolInfo)
            .Public();
        if (_generateAttribute)
            property = generator.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Property));
        var summary = sourseMember.XmlElement;
        if (summary is not null)
            property = property.WithSummary(summary);
        generator.AddProperty(property);

        var member = new PropertyDeclarationMember(symbolInfo, property, true, sourseMember.Summary);
        SourceMember(propertyName, member);
        return new(member, sourseMember);
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="propertyName"></param>
    /// <param name="fieldName"></param>
    /// <param name="sourseMember"></param>
    /// <param name="kind"></param>
    /// <returns></returns>
    public MemberArgument CheckFieldMember(SyntaxGenerator generator, string propertyName, string fieldName, IMemberInfo sourseMember, SyntaxKind kind)
    {
        var (memberType, symbolInfo) = CheckMemberType(propertyName, sourseMember.SymbolInfo);
        var fieldExpression = SyntaxFactory.IdentifierName(fieldName);
        var field = CreateField(memberType, fieldName, symbolInfo)
            .Private();

        var getDeclaration = SyntaxGenerator.PropertyGetDeclaration(fieldExpression);
        var accessorDeclaration = SyntaxFactory.AccessorDeclaration(kind)
            .WithExpressionBody(SyntaxGenerator.ExpressionBody(fieldExpression.AssignValue()))
            .WithSemicolonToken();
        var property = memberType.Property(propertyName, getDeclaration, accessorDeclaration)
            .Public();
        if (_generateAttribute)
            property = generator.GenerateAttribute(property, _attributeCacher.GetAttributes(sourseMember, AttributeTargets.Property));
        var summary = sourseMember.XmlElement;
        if (summary is not null)
            property = property.WithSummary(summary);
        generator.AddField(field);
        generator.AddProperty(property);
        var fieldMember = new FieldDeclarationMember(fieldName, symbolInfo, field, false, sourseMember.Summary);
        var propertyMember = new PropertyDeclarationMember(symbolInfo, property, true, sourseMember.Summary);
        SourceMember(propertyName, fieldMember);
        var reversedArgument = new MemberArgument(sourseMember, fieldMember);
        // 源成员映射到字段,需要手动反转为属性映射源成员
        return new MemberReversedArgument(propertyMember, sourseMember, reversedArgument);
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
