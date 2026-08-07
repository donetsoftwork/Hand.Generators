using Hand.Builders;
using Hand.Cachers;
using Hand.Converters;
using Hand.Generators;
using Hand.Maping;
using Hand.Members;
using Hand.Providers;
using Hand.Reflection;
using Hand.Sources;
using Hand.Transform;
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
public class ConvertTransform : IGeneratorTransform<IEnumerable<IGeneratorSource>>
{
    /// <inheritdoc />
    public IEnumerable<IGeneratorSource> Transform(AttributeContext context, CancellationToken cancellation = default)
    {
        if (cancellation.IsCancellationRequested)
            return [];
        if (context.TargetNode is not TypeDeclarationSyntax type)
            return [];
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
            return [];
        var attributes = context.Attributes;
        var count = attributes.Length;
        if (count == 0)
            return [];
        var compilation = context.SemanticModel.Compilation;
        var typeCacher = new TypeSymbolCacher(compilation);
        var typeInfo = typeCacher.Get(typeSymbol);
        if (typeInfo is null || typeInfo.Kind != TypeSymbolKind.Complex)
            return [];
        var sourceProvider = SourceProvider.Create(compilation, typeSymbol);
        var sourceMembers = SymbolMember.GetSourceMembers(typeCacher, typeSymbol, false);
        var methods = new List<ComplexSource>(count);
        var typeSymbols = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        var builder = new ConvertBuilder(compilation, typeCacher, SystemConvertProvider.Create(compilation), new(compilation), [] );
        var thisType = SyntaxFactory.IdentifierName(typeSymbol.Name);
        var list = new List<IGeneratorSource>(count + 1);
        foreach (var attribute in attributes)
        {
            var targetSymbol = ConvertBuilder.CheckToSymbol(attribute);
            if (targetSymbol is null || typeSymbols.Contains(targetSymbol) || targetSymbol.Equals(typeSymbol, SymbolEqualityComparer.Default))
                continue;
            var targetInfo = typeCacher.Get(targetSymbol);
            if (targetInfo is null || typeInfo.Kind != TypeSymbolKind.Complex)
                continue;
            typeSymbols.Add(targetSymbol);

            var convertToInfo = sourceProvider.ConvertTo(targetSymbol.Name);
            var convertToMethod = sourceProvider.GetConvertMethod(convertToInfo.MethodInfo, targetSymbol);
            if (convertToMethod is not null)
                continue;
            var rules = ConvertBuilder.CheckRecognizeRules(attribute);
            var arguments = MapTo(typeCacher, sourceMembers, rules, targetSymbol);
            if (arguments.Count == 0)
                continue;
            builder.Save(typeInfo, targetInfo, convertToInfo);
            var method = new ComplexSource(builder, thisType, targetInfo, convertToInfo.MethodInfo.Name, [.. arguments]);
            methods.Add(method);
            var convertFrom = ConvertBuilder.CheckState(attribute, "ConvertFrom", true);
            if (convertFrom)
            {
                var fromProvider = SourceProvider.Create(compilation, targetSymbol);
                var fromInfo = fromProvider.ConvertTo(typeSymbol.Name);
                var fromMethod = fromProvider.GetConvertMethod(fromInfo.MethodInfo, typeSymbol);
                if (fromMethod is not null)
                    continue;
                var fromArguments = MapFrom(typeCacher, typeSymbol, targetSymbol, MemberArgument.Reverse(arguments));
                if (fromArguments.Count == 0)
                    continue;
                builder.Save(targetInfo, typeInfo, fromInfo);
                var fromMethodSource = new ComplexSource(builder, SyntaxFactory.IdentifierName(targetSymbol.Name), typeInfo, fromInfo.MethodInfo.Name, [.. fromArguments]);
                list.Add(builder.AddSource(fromMethodSource, fromInfo.TypeInfo));
            }
        }
        list.Add(new ConvertToSource(type, typeSymbol, [.. methods]));
        return list;
    }    
    /// <summary>
    /// 映射
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="sourceMembers"></param>
    /// <param name="rules"></param>
    /// <param name="returnSymbol"></param>
    /// <returns></returns>
    public static List<MemberArgument> MapTo(TypeSymbolCacher typeSymbols, IDictionary<string, SymbolMember> sourceMembers, IRecognizer<string>[] rules, INamedTypeSymbol returnSymbol)
    {
        var targetMembers = SymbolMember.GetTargetMembers(typeSymbols, returnSymbol);
        var count = targetMembers.Count;
        if (count == 0)
            return [];
        foreach (var rule in rules)
            sourceMembers = rule.Recognize(sourceMembers);

        var list = new List<MemberArgument>(count);
        foreach (var targetMember in targetMembers.Values)
        {
            var name = targetMember.Name;
            if (sourceMembers.TryGetValue(name, out var sourceMember))
                list.Add(new MemberArgument(targetMember, sourceMember));
            else if (targetMember.Kind == MemberKind.Parameter)
                list.Add(new MemberArgument(targetMember, null));
        }
        return list;
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
}
