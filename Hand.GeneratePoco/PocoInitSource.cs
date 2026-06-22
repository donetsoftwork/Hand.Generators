using Hand.Maping;
using Hand.Members;
using Hand.Rule;
using Hand.Sources;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hand.GeneratePoco;

/// <summary>
/// 按record生成
/// </summary>
public class PocoInitSource(TypeDeclarationSyntax type, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol sourseSymbol, IRecognizer<string>[] memberRules, IValidation<string>? nullableRule)
    : IGeneratorSource
{
    #region 配置
    private readonly TypeDeclarationSyntax _type = type;
    private readonly Compilation _compilation = compilation;
    private readonly INamedTypeSymbol _typeSymbol = typeSymbol;
    /// <summary>
    /// 基类成员名
    /// </summary>
    protected readonly FrozenSet<string> _baseMemberNames = GetNotPrivateMembersWithBase(typeSymbol.BaseType!)
        .Select(static item => item.Name)
        .ToFrozenSet();
    /// <summary>
    /// 原成员名
    /// </summary>
    protected readonly FrozenSet<string> _memberNames = typeSymbol.GetMembers()
        .Select(static item => item.Name)
        .ToFrozenSet();
    private readonly IDictionary<string, IFieldSymbol> _sourseFields = CheckMembers(SymbolReflection.GetPublicFieldsWithBase(sourseSymbol), memberRules);
    private readonly IDictionary<string, IPropertySymbol> _sourseProperties = CheckMembers(SymbolReflection.GetPublicPropertiesWithBase(sourseSymbol), memberRules);
    private readonly IValidation<string>? _nullableRule = nullableRule;

    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type
        => _type;
    /// <summary>
    /// 反射信息
    /// </summary>
    public INamedTypeSymbol Symbol
        => _typeSymbol;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_typeSymbol.ToDisplayString()}.Poco.g.cs";
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var count = _sourseFields.Count + _sourseProperties.Count;
        var fromArguments = new List<MemberMapping>(count);
        var toArguments = new List<MemberMapping>(count);
        foreach (var item in _sourseFields)
        {
            var filed = item.Value;
            CheckMember(builder, item.Key, filed.Name, filed.Type, SymbolKind.Field, fromArguments, toArguments);
        }            
        foreach (var item in _sourseProperties)
        {
            var property = item.Value;
            CheckMember(builder, item.Key, property.Name, property.Type, SymbolKind.Property, fromArguments, toArguments);
        }

        return builder;
    }
    /// <summary>
    /// 处理成员
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="name0"></param>
    /// <param name="symbol0"></param>
    /// <param name="kind0"></param>
    /// <param name="fromArguments"></param>
    /// <param name="toArguments"></param>
    public void CheckMember(SyntaxGenerator builder, string name, string name0, ITypeSymbol symbol0, SymbolKind kind0, List<MemberMapping> fromArguments, List<MemberMapping> toArguments)
    {
        var property = CreateGetInitProperty(name, name0, symbol0, kind0, fromArguments, toArguments);
        if (property is null)
            return;
        builder.AddProperty(property);
    }

    private PropertyDeclarationSyntax? CreateGetInitProperty(string name, string name0, ITypeSymbol symbol0, SymbolKind kind0, List<MemberMapping> fromArguments, List<MemberMapping> toArguments)
    {
        if (_memberNames.Contains(name) || _baseMemberNames.Contains(name))
            return null;
        if (symbol0 is not INamedTypeSymbol namedTypeSymbol0)
            return null;
        var memberCheckType = MemberSymbolInfo.Create(_compilation, namedTypeSymbol0);
        if (memberCheckType is null)
            return null;
        var toArgument = new MemberMapping(name0, memberCheckType, kind0, new Member(name, memberCheckType));
        var fromArgument = new MemberMapping(name, memberCheckType, SymbolKind.Parameter, toArgument);
        fromArguments.Add(fromArgument);
        toArguments.Add(toArgument);

        var memberType = CheckMemberNullAble(name, memberCheckType.CheckPoco().ToSyntax());
        var property = memberType.GetInitProperty(name);
        return property;
    }

    /// <summary>
    /// 判断是否可空
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="memberType"></param>
    /// <returns></returns>
    public TypeSyntax CheckMemberNullAble(string memberName, TypeSyntax memberType)
    {
        if (_nullableRule is null)
            return memberType;
        if (_nullableRule.Validate(memberName))
            return memberType.CheckNullable();

        return memberType;
    }
    /// <summary>
    /// 识别成员字典
    /// </summary>
    /// <param name="members"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IDictionary<string, TMember> CheckMembers<TMember>(IEnumerable<TMember> members, IRecognizer<string>[] rules)
        where TMember : ISymbol
    {
        IDictionary<string, TMember> dic = members.ToDictionary(p => p.Name);
        foreach (var rule in rules)
            dic = rule.Recognize(dic);
        return dic;
    }
    /// <summary>
    /// 获取非私有成员（字段和属性）包括基类的
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<ISymbol> GetNotPrivateMembersWithBase(INamedTypeSymbol type)
    {
        return SymbolReflection.GetNotPrivateMembersWithBase<IFieldSymbol>(type, SymbolKind.Field)
            .Concat<ISymbol>(SymbolReflection.GetNotPrivateMembersWithBase<IPropertySymbol>(type, SymbolKind.Property));
    }
}
