using Hand.Builders;
using Hand.Cachers;
using Hand.Maping;
using Hand.Members;
using Hand.Providers;
using Hand.Reflection;
using Hand.Rule;
using Hand.Sources;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace Hand.GeneratePoco;

/// <summary>
/// Poco生成源
/// </summary>
public abstract class PocoSource(SyntaxGenerator generator, ConvertBuilder convertBuilder, ComplexTypeInfo typeInfo, INamedTypeSymbol typeSymbol, ComplexTypeInfo sourseInfo, INamedTypeSymbol sourseSymbol, AttributeData attribute)
    : IGeneratorSource
{
    /// <summary>
    /// Poco生成源
    /// </summary>
    public PocoSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder, ComplexTypeInfo typeInfo, ComplexTypeInfo sourseInfo, AttributeData attribute)
        : this(SyntaxGenerator.Clone(type), convertBuilder, typeInfo, typeInfo.Symbol, sourseInfo, sourseInfo.Symbol, attribute)
    {
    }
    #region 配置
    /// <summary>
    /// 类型
    /// </summary>
    protected readonly SyntaxGenerator _generator = generator;
    /// <summary>
    /// 当前类型
    /// </summary>
    protected readonly TypeSyntax _thisType = generator.Display(typeSymbol);
    /// <summary>
    /// 来源类型
    /// </summary>
    protected readonly TypeSyntax _sourseType = generator.Display(sourseSymbol);
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
    protected readonly ComplexTypeInfo _typeInfo = typeInfo;
    /// <summary>
    /// 类型符号
    /// </summary>
    protected readonly INamedTypeSymbol _typeSymbol = typeSymbol;
    /// <summary>
    /// 来源类型信息
    /// </summary>
    protected readonly ComplexTypeInfo _sourseInfo = sourseInfo;
    /// <summary>
    /// 来源类型符号
    /// </summary>
    protected readonly INamedTypeSymbol _sourseSymbol = sourseSymbol;
    /// <summary>
    /// 特性配置
    /// </summary>
    protected readonly AttributeData _attribute = attribute;
    /// <summary>
    /// 投影规则
    /// </summary>
    protected readonly IRecognizer<string>[] _recognizers = ConvertBuilder.CheckRecognizeRules(attribute);
    ///// <summary>
    ///// 源成员
    ///// </summary>
    //protected readonly IDictionary<string, SymbolMember> _sourceMembers = GetSourceMembers(convertBuilder.TypeCacher, sourseSymbol, ConvertBuilder.CheckRecognizeRules(attribute));
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
    ///// <summary>
    ///// 是否生成convertTo
    ///// </summary>
    //protected readonly bool _convertTo = ConvertBuilder.CheckState(attribute, "ConvertTo", true);
    ///// <summary>
    ///// 是否生成convertFrom
    ///// </summary>
    //protected readonly bool _convertFrom = ConvertBuilder.CheckState(attribute, "ConvertFrom", true);
    /// <summary>
    /// 是否生成默认值
    /// </summary>
    protected readonly bool _useDefault = ConvertBuilder.CheckState(attribute, "Default", false);
    /// <summary>
    /// 标记符号信息缓存
    /// </summary>
    protected readonly AttributeSymbolCacher _attributeCacher = new(convertBuilder.Compilation);

    ///// <summary>
    ///// 类型
    ///// </summary>
    //public TypeDeclarationSyntax Type
    //    => _type;
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
    /// 构造参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public ParameterSyntax CreateParameter(TypeSyntax type, string name, ITypeSymbolInfo info)
    {
        return _useDefault ? type.Parameter(name, DefaultExpressionBuilder.GetParameterDefault(info)) :
            type.Parameter(name);
    }
    /// <summary>
    /// 构造字段
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public FieldDeclarationSyntax CreateField(TypeSyntax type, string name, ITypeSymbolInfo info)
    {
        return _useDefault ? type.Field(name, DefaultExpressionBuilder.GetDefault(info)) :
            type.Field(name);
    }    
    /// <summary>
    /// 处理成员类型
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="sourseInfo"></param>
    /// <returns></returns>
    public (TypeSyntax, ITypeSymbolInfo) CheckMemberType(string propertyName, ITypeSymbolInfo sourseInfo)
    {
        var propertyInfo = sourseInfo.CheckPoco();
        var propertySymbol = propertyInfo.Original;
        // 自包含属性转化为目标类型
        if (propertySymbol.Equals(_sourseSymbol, SymbolEqualityComparer.Default))
            return (_thisType, _typeInfo);

        var propertyType = _generator.Display(propertyInfo);
        if (sourseInfo.IsNullable || CheckMemberNullAble(propertyName))
            return (propertyType.Nullable(), propertyInfo.GetNullable(_compilation));
        return (propertyType, propertyInfo);
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
    ///// <summary>
    ///// 判断成员是否可空
    ///// </summary>
    ///// <param name="propertyName"></param>
    ///// <param name="propertyType"></param>
    ///// <returns></returns>
    //public TypeSyntax CheckMemberNullAble(string propertyName, TypeSyntax propertyType)
    //{
    //    if (_nullableRule is null)
    //        return propertyType;
    //    if (_nullableRule.Validate(propertyName))
    //        return propertyType.CheckNullable();
    //    return propertyType;
    //}
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
    #region CheckConvert
    /// <summary>
    /// 处理转化
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="sourceMembers"></param>
    /// <param name="arguments"></param>
    public void CheckConvert(SyntaxGenerator generator, IDictionary<string, SymbolMember> sourceMembers, List<MemberArgument> arguments)
    {
        var convertFrom = ConvertBuilder.CheckState(_attribute, "ConvertFrom", true);
        var convertTo = ConvertBuilder.CheckState(_attribute, "ConvertTo", true);
        if (!convertFrom && !convertTo)
            return;
        var members = SymbolMember.GetTargetMembers(_convertBuilder.TypeCacher, _typeSymbol, true);
        // 参考生成规则映射
        arguments = MapFrom(members, sourceMembers, arguments);
        if (convertFrom)
            CheckConvertFrom(arguments);
        if (!convertTo)
            return;
        var method = CheckConvertTo(generator, _convertBuilder, arguments);
        if (method is not null)
            generator.AddMethod(method);
    }
    /// <summary>
    /// 处理ConvertFrom
    /// </summary>
    /// <param name="arguments"></param>
    public void CheckConvertFrom(List<MemberArgument> arguments)
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
        var source = new ComplexSource(_convertBuilder, symbolName, _typeInfo, methodName, [.. arguments]);
        _convertBuilder.AddSource(source, typeInfo, _sourseSymbol);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="members"></param>
    /// <param name="sourceMembers"></param>
    /// <param name="referenceArguments"></param>
    /// <returns></returns>
    public static List<MemberArgument> MapFrom(Dictionary<string, SymbolMember> members, IDictionary<string, SymbolMember> sourceMembers, List<MemberArgument> referenceArguments)
    {
        if (members.Count == 0)
            return referenceArguments;
        return ConvertBuilder.Map(members.Values, sourceMembers, referenceArguments);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="referenceArguments"></param>
    /// <returns></returns>
    public List<MemberArgument> MapTo(TypeSymbolCacher typeSymbols, List<MemberArgument> referenceArguments)
    {
        var members = SymbolMember.GetTargetMembers(typeSymbols, _sourseSymbol, true);
        var parameterCount = members.Count;
        if (parameterCount == 0)
            return [];
        //投影规则翻转
        var recognizers = System.Array.ConvertAll(_recognizers, recognizer => recognizer.Reverse());
        var sourceMembers = ConvertBuilder.GetSourceMembers(typeSymbols, _typeSymbol, recognizers);
        return ConvertBuilder.Map(members.Values, sourceMembers, referenceArguments);
    }
    /// <summary>
    /// 处理ConvertTo
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="convertBuilder"></param>
    /// <param name="referenceArguments"></param>
    /// <returns></returns>
    public MethodDeclarationSyntax? CheckConvertTo(SyntaxGenerator generator, ConvertBuilder convertBuilder, List<MemberArgument> referenceArguments)
    {
        var sourceProvider = SourceProvider.Create(_compilation, _typeSymbol);
        var convertToInfo = sourceProvider.ConvertTo(_sourseSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, _sourseSymbol);
        if (convertToMethod is not null)
            return null;
        var methodName = convertToInfo.MethodInfo.Name;
        var arguments = MapTo(_convertBuilder.TypeCacher, MemberArgument.Reverse(referenceArguments));
        if (arguments.Count == 0)
            return null;
        _convertBuilder.Save(_typeInfo, _sourseInfo, convertToInfo);
        var source = new ComplexSource(convertBuilder, _thisType, _sourseInfo, methodName, [.. arguments]);
        return source.CreateMethod(generator);
    }
    #endregion
}
