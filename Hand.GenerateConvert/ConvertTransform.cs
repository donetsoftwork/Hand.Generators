using Hand.Builders;
using Hand.Cachers;
using Hand.Converters;
using Hand.Generators;
using Hand.Members;
using Hand.Providers;
using Hand.Reflection;
using Hand.Sources;
using Hand.Symbols;
using Hand.Transform;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hand;

/// <summary>
/// 转化
/// </summary>
public class ConvertTransform : IGeneratorTransform<ConvertToSource>
{
    /// <inheritdoc />
    public ConvertToSource? Transform(AttributeContext context, CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
            return null;
        if (context.TargetNode is not TypeDeclarationSyntax type)
            return null;
        if (context.TargetSymbol is not INamedTypeSymbol targetSymbol)
            return null;
        var attributes = context.Attributes;
        var count = attributes.Length;
        if (count == 0)
            return null;
        var compilation = context.SemanticModel.Compilation;
        var typeCacher = new TypeSymbolCacher(compilation);
        if (typeCacher.Get(targetSymbol) is not ComplexTypeInfo targetInfo)
            return null;
        var sourceMembers = SymbolMember.GetSourceMembers(typeCacher, targetSymbol, true);
        var targetMembers = GetTargetMembers(typeCacher, targetSymbol);
        var sourceProvider = SourceProvider.Create(compilation, targetSymbol);
        
        var methods = new List<ComplexSource>(count);
        var toSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var builder = new ConvertBuilder(compilation, typeCacher, SystemConvertProvider.Create(compilation), new(compilation), []);
        var thisType = SyntaxFactory.IdentifierName(targetSymbol.Name);
        var list = new List<IGeneratorSource>(count + 1);
        foreach (var attribute in attributes)
        {
            var toSymbol = ConvertBuilder.CheckToSymbol(attribute);
            if (toSymbol is null || toSymbols.Contains(toSymbol) || toSymbol.Equals(targetSymbol, SymbolEqualityComparer.Default))
                continue;
            toSymbols.Add(toSymbol);
            var convertFrom = ConvertBuilder.CheckState(attribute, "ConvertFrom", false);
            var convertTo = ConvertBuilder.CheckState(attribute, "ConvertTo", true);
            if (!convertFrom && !convertTo)
                continue;
            if (typeCacher.Get(toSymbol) is not ComplexTypeInfo toInfo)
                continue;

            var toTargetMembers = GetTargetMembers(typeCacher, toSymbol);
            var toSourceMembers = SymbolMember.GetSourceMembers(typeCacher, toSymbol, true);
            var recognizers = ConvertBuilder.CheckRecognizeRules(attribute);
            var arguments = ConvertBuilder.ReversedMap(ConvertBuilder.Recognize(sourceMembers, recognizers), toTargetMembers.Values, toSourceMembers)
                 .ToList();
            if (convertTo)
            {
                var method = CheckConvertTo(builder, targetInfo, toInfo, arguments);
                if (method is not null)
                    methods.Add(method);
            }
            if (convertFrom)
            {
                //投影规则翻转
                recognizers = System.Array.ConvertAll(recognizers, recognizer => recognizer.Reverse());
                CheckConvertFrom(builder, targetInfo, toInfo, ConvertBuilder.Recognize(toSourceMembers, recognizers), arguments);
            }
        }
        var result = new ConvertToSource(builder, type, targetSymbol.MetadataName, [.. methods]);
        return result;
    }
    /// <summary>
    /// 获取目标成员
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Dictionary<string, SymbolMember> GetTargetMembers(TypeSymbolCacher typeSymbols, INamedTypeSymbol type)
    {
        if (type.IsPartial())
            return SymbolMember.GetTargetMembers(typeSymbols, type, false);
        return SymbolMember.GetTargetMembers(typeSymbols, type, true);
    }
    #region CheckConvert
    /// <summary>
    /// 处理ConvertTo
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="targetInfo"></param>
    /// <param name="toInfo"></param>
    /// <param name="arguments"></param>
    public static ComplexSource? CheckConvertTo(ConvertBuilder builder, ComplexTypeInfo targetInfo, ComplexTypeInfo toInfo, List<MemberArgument> arguments)
    {
        var targetSymbol = targetInfo.Symbol;
        var toSymbol = toInfo.Symbol;
        var sourceProvider = SourceProvider.Create(builder.Compilation, targetSymbol);
        var convertToInfo = sourceProvider.ConvertTo(toSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, toSymbol);
        if (convertToMethod is not null)
            return null;
        builder.Save(targetInfo, toInfo,  convertToInfo);
        var source = new ComplexSource(builder, targetSymbol.Name, toInfo, convertToInfo.MethodInfo.Name, [.. arguments]);
        if (targetSymbol.IsPartial())
            return source;
        builder.AddSource(source, convertToInfo.TypeInfo, targetSymbol);
        return null;
    }
    /// <summary>
    /// 处理ConvertFrom
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="targetInfo"></param>
    /// <param name="toInfo"></param>
    /// <param name="sourceMembers"></param>
    /// <param name="referenceArguments"></param>
    /// <returns></returns>
    public static void CheckConvertFrom(ConvertBuilder builder, ComplexTypeInfo targetInfo, ComplexTypeInfo toInfo, IDictionary<string, SymbolMember> sourceMembers, List<MemberArgument> referenceArguments)
    {
        var targetSymbol = targetInfo.Symbol;
        var toSymbol = toInfo.Symbol;
        var sourceProvider = SourceProvider.Create(builder.Compilation, toSymbol);
        var convertToInfo = sourceProvider.ConvertTo(targetSymbol.Name);
        var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, targetSymbol);
        if (convertToMethod is not null)
            return;
        var targetMembers = SymbolMember.GetTargetMembers(builder.TypeCacher, targetSymbol, true);
        var arguments = ConvertBuilder.Map(targetMembers.Values, sourceMembers, MemberArgument.Reverse(referenceArguments));
        if (arguments.Count == 0)
            return;
        builder.Save(toInfo, targetInfo, convertToInfo);
        var source = new ComplexSource(builder, toSymbol.Name, targetInfo, convertToInfo.MethodInfo.Name, [.. arguments]);
        builder.AddSource(source, convertToInfo.TypeInfo, toSymbol);
    }
    #endregion
}
