using Hand.Arguments;
using Hand.Attributes;
using Hand.Builders;
using Hand.Maping;
using Hand.Members;
using Hand.Naming;
using Hand.Providers;
using Hand.Rule;
using Hand.Sources;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand.GeneratePoco;

/// <summary>
/// Poco生成源
/// </summary>
public abstract class PocoSource(SyntaxGenerator generator, ConvertBuilder convertBuilder, ComplexTypeInfo toInfo, INamedTypeSymbol toSymbol, ComplexTypeInfo fromInfo, INamedTypeSymbol fromSymbol, IRecognizer<string>[] recognizers, AttributeData attribute)
    : IGeneratorSource
{
    /// <summary>
    /// Poco生成源
    /// </summary>
    public PocoSource(TypeDeclarationSyntax type, ConvertBuilder convertBuilder, ComplexTypeInfo toInfo, ComplexTypeInfo fromInfo, AttributeData attribute)
        : this(SyntaxGenerator.Clone(type), convertBuilder, toInfo, toInfo.Symbol, fromInfo, fromInfo.Symbol, ConvertBuilder.CheckRecognizeRules(attribute), attribute)
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
    protected readonly TypeSyntax _thisType = generator.Display(toSymbol);
    /// <summary>
    /// 来源类型
    /// </summary>
    protected readonly TypeSyntax _fromType = generator.Display(fromSymbol);
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
    protected readonly ComplexTypeInfo _toInfo = toInfo;
    /// <summary>
    /// 类型符号
    /// </summary>
    protected readonly INamedTypeSymbol _toSymbol = toSymbol;
    /// <summary>
    /// 来源类型信息
    /// </summary>
    protected readonly ComplexTypeInfo _fromInfo = fromInfo;
    /// <summary>
    /// 来源类型符号
    /// </summary>
    protected readonly INamedTypeSymbol _fromSymbol = fromSymbol;
    /// <summary>
    /// 特性配置
    /// </summary>
    protected readonly AttributeData _attribute = attribute;
    /// <summary>
    /// 来源投影规则
    /// </summary>
    protected readonly IRecognizer<string>[] _fromRecognizers = recognizers;
    /// <summary>
    /// 目标投影规则
    /// </summary>
    protected readonly IRecognizer<string>[] _toRecognizers = System.Array.ConvertAll(recognizers, static recognizer => recognizer.Reverse());
    /// <summary>
    /// 来源成员
    /// </summary>
    protected readonly Dictionary<string, IMemberInfo> _fromSourceMembers = SymbolMember.GetSourceMembers(convertBuilder.TypeBuilder, fromSymbol, true);
    /// <summary>
    /// 目标成员
    /// </summary>
    protected readonly Dictionary<string, IMemberInfo> _toSourceMembers = SymbolMember.GetSourceMembers(convertBuilder.TypeBuilder, toSymbol);
    ///// <summary>
    ///// 源成员
    ///// </summary>
    //protected readonly IDictionary<string, ISymbolMemberInfo> _sourceMembers = GetSourceMembers(convertBuilder.TypeCacher, sourseSymbol, ConvertBuilder.CheckRecognizeRules(attribute));
    ///// <summary>
    ///// 原成员名
    ///// </summary>
    //protected readonly FrozenSet<string> _memberNames = SymbolReflection.GetMembersWithBase(typeSymbol)
    //    .Select(static item => item.Name)
    //    .ToFrozenSet();
    /// <summary>
    /// 成员命名器
    /// </summary>
    protected readonly TypedProvider _naming = TypedProvider.Create(toSymbol);
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
        => _toSymbol;
    /// <inheritdoc />
    public string GenerateFileName
        => $"{_toSymbol.ToDisplayString()}.Poco.g.cs";
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
        var isNullable = sourseInfo.IsNullable || CheckMemberNullAble(propertyName);
        var propertyInfo = sourseInfo.CheckPoco();
        var propertySymbol = propertyInfo.Symbol;
        // 自包含属性转化为目标类型
        if (propertySymbol.Equals(_fromSymbol, SymbolEqualityComparer.Default))
        {
            if (isNullable)
                return (_thisType.Nullable(), _toInfo.GetNullable(_compilation));
            return (_thisType, _toInfo);
        }

        var propertyType = propertyInfo.Display(_generator);
        if (isNullable)
            return (propertyType.CheckNullable(), propertyInfo.GetNullable(_compilation));
        return (propertyType, propertyInfo);
    }
    /// <summary>
    /// 添加来源成员
    /// </summary>
    /// <param name="name"></param>
    /// <param name="member"></param>
    public void SourceMember(string name, IMemberInfo member)
        => _toSourceMembers[name] = member;
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
    /// <param name="fromSources"></param>
    /// <param name="arguments"></param>
    public void CheckConvert(SyntaxGenerator generator, IDictionary<string, IMemberInfo> fromSources, List<MemberArgument> arguments)
    {
        var convertFrom = ConvertBuilder.CheckState(_attribute, "ConvertFrom", true);
        var convertTo = ConvertBuilder.CheckState(_attribute, "ConvertTo", true);
        if (!convertFrom && !convertTo)
            return;
        var members = SymbolMember.GetTargetMembers(_convertBuilder.TypeBuilder, _toSymbol, true);
        // 参考生成规则映射
        arguments = MapFrom(members, fromSources, arguments);
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
        var sourceProvider = SourceProvider.Create(_compilation, _fromSymbol);
        var convertToInfo = sourceProvider.ConvertTo(_toSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, _toSymbol);
        if (convertToMethod is not null)
            return;
        var typeInfo = convertToInfo.TypeInfo;
        _convertBuilder.Save(_fromInfo, _toInfo, convertToInfo);
        var symbolName = convertToInfo.Provider.SymbolName;
        var methodName = convertToInfo.MethodInfo.Name;
        var source = new ComplexSource(_convertBuilder, symbolName, _toInfo, methodName, [.. arguments]);
        _convertBuilder.AddSource(source, typeInfo, _fromSymbol);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="members"></param>
    /// <param name="sourceMembers"></param>
    /// <param name="referenceArguments"></param>
    /// <returns></returns>
    public static List<MemberArgument> MapFrom(IDictionary<string, IMemberInfo> members, IDictionary<string, IMemberInfo> sourceMembers, List<MemberArgument> referenceArguments)
    {
        if (members.Count == 0)
            return referenceArguments;
        return ConvertBuilder.Map(members.Values, sourceMembers, referenceArguments);
    }
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <returns></returns>
    public List<MemberArgument> MapTo(TypeInfoBuilder typeSymbols/*, List<MemberArgument> referenceArguments*/)
    {
        var members = SymbolMember.GetTargetMembers(typeSymbols, _fromSymbol, true);
        var parameterCount = members.Count;
        if (parameterCount == 0)
            return [];
        //投影规则翻转
        //var toRecognizers = System.Array.ConvertAll(_fromRecognizers, static recognizer => recognizer.Reverse());
        var sourceMembers = ConvertBuilder.Recognize(_toSourceMembers, _toRecognizers);
        return ConvertBuilder.Map(members.Values, sourceMembers)
            .ToList();
        //return ConvertBuilder.Map(members.Values, sourceMembers, referenceArguments);
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
        var sourceProvider = SourceProvider.Create(_compilation, _toSymbol);
        var convertToInfo = sourceProvider.ConvertTo(_fromSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, _fromSymbol);
        if (convertToMethod is not null)
            return null;
        var methodName = convertToInfo.MethodInfo.Name;
        var arguments = MapTo(_convertBuilder.TypeBuilder/*, MemberArgument.Reverse(referenceArguments*/);
        if (arguments.Count == 0)
            return null;
        _convertBuilder.Save(_toInfo, _fromInfo, convertToInfo);
        var source = new ComplexSource(convertBuilder, _thisType, _fromInfo, methodName, [.. arguments]);
        return source.CreateMethod(generator);
    }
    #endregion
}
