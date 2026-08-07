using Hand.Builders;
using Hand.Cachers;
using Hand.Maping;
using Hand.Members;
using Hand.Providers;
using Hand.Reflection;
using Hand.Rule;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace Hand.GeneratePoco;

/// <summary>
/// Poco生成源
/// </summary>
public abstract class PocoSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder, TypeSymbolInfo typeInfo, INamedTypeSymbol typeSymbol, TypeSymbolInfo sourseInfo, INamedTypeSymbol sourseSymbol, AttributeData attribute)
    : IGeneratorSource
{
    /// <summary>
    /// Poco生成源
    /// </summary>
    public PocoSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder, TypeSymbolInfo typeInfo, TypeSymbolInfo sourseInfo, AttributeData attribute)
        : this(type, convertBuilder, typeInfo, typeInfo.Symbol, sourseInfo, sourseInfo.Symbol, attribute)
    {
    }
    #region 配置
    /// <summary>
    /// 类型
    /// </summary>
    protected readonly TypeDeclarationSyntax _type = type;
    /// <summary>
    /// 当前类型
    /// </summary>
    protected readonly TypeSyntax _thisType = SyntaxFactory.IdentifierName(typeSymbol.Name);
    /// <summary>
    /// 编译
    /// </summary>
    protected readonly Compilation _compilation = convertBuilder.Compilation;
    /// <summary>
    /// 类型转化构建器
    /// </summary>
    protected readonly ConvertBuilder _convertBuilder = convertBuilder;
    /// <summary>
    /// 类型信息
    /// </summary>
    protected readonly TypeSymbolInfo _typeInfo = typeInfo;
    /// <summary>
    /// 类型符号
    /// </summary>
    protected readonly INamedTypeSymbol _typeSymbol = typeSymbol;
    /// <summary>
    /// 来源类型信息
    /// </summary>
    protected readonly TypeSymbolInfo _sourseInfo = sourseInfo;
    /// <summary>
    /// 来源类型符号
    /// </summary>
    protected readonly INamedTypeSymbol _sourseSymbol = sourseSymbol;
    /// <summary>
    /// 源成员
    /// </summary>
    protected readonly IDictionary<string, SymbolMember> _sourceMembers = GetSourceMembers(convertBuilder.TypeCacher, sourseSymbol, ConvertBuilder.CheckRecognizeRules(attribute));
    /// <summary>
    /// 原成员名
    /// </summary>
    protected readonly FrozenSet<string> _memberNames = SymbolReflection.GetMembersWithBase(typeSymbol)
        .Select(static item => item.Name)
        .ToFrozenSet();
    /// <summary>
    /// 可空规则
    /// </summary>
    protected readonly IValidation<string>? _nullableRule = CheckNullableRule(attribute);
    /// <summary>
    /// 是否生成特性标记
    /// </summary>
    protected readonly bool _generateAttribute = ConvertBuilder.CheckState(attribute, "GenerateAttribute", false);
    /// <summary>
    /// 是否生成convertTo
    /// </summary>
    protected readonly bool _convertTo = ConvertBuilder.CheckState(attribute, "ConvertTo", true);
    /// <summary>
    /// 是否生成convertFrom
    /// </summary>
    protected readonly bool _convertFrom = ConvertBuilder.CheckState(attribute, "ConvertFrom", true);
    /// <summary>
    /// 是否生成默认值
    /// </summary>
    protected readonly bool _useDefault = ConvertBuilder.CheckState(attribute, "Default", false);
    /// <summary>
    /// 标记符号信息缓存
    /// </summary>
    protected readonly AttributeSymbolCacher _attributeCacher = new(convertBuilder.Compilation);

    /// <summary>
    /// 类型
    /// </summary>
    public TypeDeclarationSyntax Type
        => _type;
    /// <summary>
    /// 反射信息
    /// </summary>
    public INamedTypeSymbol TypeSymbol
        => _typeSymbol;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_typeSymbol.ToDisplayString()}.Poco.g.cs";
    /// <summary>
    /// 类型转化构建器
    /// </summary>
    public ConvertBuilder ConvertBuilder
        => _convertBuilder;
    #endregion
    /// <inheritdoc />
    public abstract SyntaxGenerator Generate();
    /// <summary>
    /// 处理ConvertFrom
    /// </summary>
    /// <param name="convertBuilder"></param>
    /// <param name="generateArguments"></param>
    public void CheckConvertFrom(ConvertBuilder convertBuilder, List<MemberArgument> generateArguments)
    {
        var sourceProvider = SourceProvider.Create(_compilation, _sourseSymbol);
        var convertToInfo = sourceProvider.ConvertTo(_typeSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, _typeSymbol);
        if (convertToMethod is not null)
            return;        
        var typeInfo = convertToInfo.TypeInfo;
        _convertBuilder.Save(_sourseInfo, _typeInfo, convertToInfo);
        var symbolName = convertToInfo.Provider.SymbolName;
        var methodName = convertToInfo.MethodInfo.Name;
        var arguments = MapFrom(_convertBuilder.TypeCacher, _typeSymbol, _sourseSymbol, generateArguments)
            .ToArray();
        var source = new ComplexSource(convertBuilder, symbolName, _typeInfo, methodName, arguments);
        convertBuilder.AddSource(source, typeInfo);
    }
    /// <summary>
    /// 构造参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public ParameterSyntax CreateParameter(TypeSyntax type, string name, TypeSymbolInfo info)
    {
        return _useDefault ? type.Parameter(name, DefaultExpressionBuilder.Default(info, _compilation)) :
            type.Parameter(name);
    }
    /// <summary>
    /// 构造字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public FieldDeclarationSyntax CreateField(TypeSyntax type, string name, TypeSymbolInfo info)
    {
        return _useDefault ? type.Field(name, DefaultExpressionBuilder.Default(info, _compilation)) :
            type.Field(name);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="typeSymbol"></param>
    /// <param name="sourceSymbol"></param>
    /// <param name="generateArguments"></param>
    /// <returns></returns>
    public static List<MemberArgument> MapFrom(TypeSymbolCacher typeSymbols, INamedTypeSymbol typeSymbol, INamedTypeSymbol sourceSymbol, List<MemberArgument> generateArguments)
    {
        var parameters = SymbolMember.GetTargetMembers(typeSymbols, typeSymbol, true);
        var parameterCount = parameters.Count;
        if (parameterCount == 0)
            return generateArguments;

        var arguments = ConvertBuilder.Map(typeSymbols, parameters.Values, sourceSymbol)
            .ToList();
        foreach (var generated in generateArguments)
        {
            var sourceMember = generated.Source;
            if (sourceMember is null)
                continue;
            var argument = arguments.FirstOrDefault(item => item.Member.Equals(generated.Member));
            if (argument is null)
                arguments.Add(generated);
            else
                argument.Source = sourceMember;
        }
        return arguments;
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="typeSymbol"></param>
    /// <param name="sourceSymbol"></param>
    /// <param name="generateArguments"></param>
    /// <returns></returns>
    public static List<MemberArgument> MapTo(TypeSymbolCacher typeSymbols, INamedTypeSymbol typeSymbol, INamedTypeSymbol sourceSymbol, List<MemberArgument> generateArguments)
    {
        var parameters = SymbolMember.GetTargetMembers(typeSymbols, sourceSymbol, true);
        var parameterCount = parameters.Count;
        if (parameterCount == 0)
            return [];
        var arguments = ConvertBuilder.Map(typeSymbols, parameters.Values, typeSymbol)
            .ToList();
        foreach (var generated in generateArguments)
        {
            var sourceMember = generated.Source;
            if (sourceMember is null)
                continue;
            var argument = arguments.FirstOrDefault(item => item.Member.Equals(generated.Member));
            if (argument is null)
                arguments.Add(generated);
            else
                argument.Source = generated.Source;
        }
        return arguments;
    }
    /// <summary>
    /// 处理ConvertTo
    /// </summary>
    /// <param name="convertBuilder"></param>
    /// <param name="generateArguments"></param>
    /// <returns></returns>
    public MethodDeclarationSyntax? CheckConvertTo(ConvertBuilder convertBuilder, List<MemberArgument> generateArguments)
    {
        var sourceProvider = SourceProvider.Create(_compilation, _typeSymbol);
        var convertToInfo = sourceProvider.ConvertTo(_sourseSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, _sourseSymbol);
        if (convertToMethod is not null)
            return null;
        //var typeInfo = convertToInfo.TypeInfo;
        //var symbolName = convertToInfo.Provider.SymbolName;
        var methodName = convertToInfo.MethodInfo.Name;
        var arguments = MapTo(_convertBuilder.TypeCacher, _typeSymbol, _sourseSymbol, MemberArgument.Reverse(generateArguments));
        if (arguments.Count == 0)
            return null;
        _convertBuilder.Save(_typeInfo, _sourseInfo, convertToInfo);
        var source = new ComplexSource(convertBuilder, _thisType, _sourseInfo, methodName, [.. arguments]);
        return source.CreateMethod();
    }
    /// <summary>
    /// 判断成员是否可空
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="propertyType"></param>
    /// <returns></returns>
    public TypeSyntax CheckMemberNullAble(string propertyName, TypeSyntax propertyType)
    {
        if (_nullableRule is null)
            return propertyType;
        if (_nullableRule.Validate(propertyName))
            return propertyType.CheckNullable();
        return propertyType;
    }
    /// <summary>
    /// 处理成员类型
    /// </summary>
    /// <param name="compilation"></param>
    /// <param name="propertyName"></param>
    /// <param name="sourseInfo"></param>
    /// <returns></returns>
    public (TypeSyntax, TypeSymbolInfo) CheckMemberType(Compilation compilation, string propertyName, TypeSymbolInfo sourseInfo)
    {
        var propertySymbol = sourseInfo.CheckPoco();
        TypeSyntax propertyType;
        TypeSymbolKind kind;
        if (propertySymbol.Equals(_sourseSymbol, SymbolEqualityComparer.Default))
        {
            propertySymbol = _typeSymbol;
            propertyType = _thisType;
            kind = sourseInfo.Kind;
        }
        else
        {
            propertyType = propertySymbol.ToSyntax();
            kind = TypeSymbolKind.Primitive;
        }    
        if (sourseInfo.Kind.IsNullable() || CheckMemberNullAble(propertyName))
        {
            return (propertyType.Nullable(),
            new TypeSymbolInfo(compilation.GetNullable(propertySymbol), propertySymbol, kind | TypeSymbolKind.Nullable, null));
        }
        else
        {
            return (propertyType,
            new TypeSymbolInfo(propertySymbol, propertySymbol, kind, null));
        }
    }
    /// <summary>
    /// 判断成员是否可空
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public bool CheckMemberNullAble(string propertyName)
    {
        if (_nullableRule is null)
            return false;
        return _nullableRule.Validate(propertyName);
    }
    /// <summary>
    /// 获取成员字典
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="type"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IDictionary<string, SymbolMember> GetSourceMembers(TypeSymbolCacher typeSymbols, INamedTypeSymbol type, IRecognizer<string>[] rules)
    {
        IDictionary<string, SymbolMember> sourceMembers = SymbolMember.GetSourceMembers(typeSymbols, type);
        foreach (var rule in rules)
            sourceMembers = rule.Recognize(sourceMembers);
        return sourceMembers;
    }
    /// <summary>
    /// 解析可空规则
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static IValidation<string>? CheckNullableRule(AttributeData attribute)
    {
        var argument = SymbolAttributeHelper.GetArgumentConstant(attribute, "NullableRule");
        if (argument is null)
            return null;
        var text = argument.Value.GetPrimitive<string>();
        if (string.IsNullOrEmpty(text))
            return null;
        return MemberRuleParser.Default.Parse(text);
    }
}
