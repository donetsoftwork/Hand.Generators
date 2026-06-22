using Hand.Maping;
using Hand.Members;
using Hand.Sources;
using Hand.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Hand;

/// <summary>
/// 转化自生成源
/// </summary>
public class ConvertFromSource(TypeDeclarationSyntax type, Compilation compilation, INamedTypeSymbol typeSymbol, INamedTypeSymbol fromSymbol, IEnumerable<MemberMapping> mappings)
    : IGeneratorSource
{
    #region 配置
    private readonly TypeDeclarationSyntax _type = type;
    private readonly Compilation _compilation = compilation;
    private readonly INamedTypeSymbol _typeSymbol = typeSymbol;
    private readonly INamedTypeSymbol _fromSymbol = fromSymbol;
    private readonly IEnumerable<MemberMapping> _mappings = mappings;

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
        => $"{_typeSymbol.ToDisplayString()}From{_fromSymbol.ToDisplayString()}.g.cs";
    #endregion
    /// <inheritdoc />
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var source = SyntaxFactory.IdentifierName("source");
        var entity = SyntaxFactory.IdentifierName("entity");
        var returnType = _typeSymbol.ToSyntax();
        var scope = SyntaxGenerator.Scope();

        scope.Declare(_typeSymbol.ToSyntax(), entity);
        foreach (var mapping in _mappings)
        {

            //var source = mapping.Source;
            //if (source is null)
            //    continue;

        }

        return builder;
    }
    /// <summary>
    /// 检查参数
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="arguments"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public IEnumerable<ExpressionSyntax> CheckParameters(IdentifierNameSyntax owner,  IEnumerable<MemberMapping> arguments, ICollection<ParameterSyntax> parameters)
    {
        foreach (var mapping in arguments)
        {
            var memberSymbol = mapping.MemberSymbol;
            var source = mapping.Source;
            if (source is null)
            {
                var parameterName = SyntaxFactory.IdentifierName(mapping.Name);
                var parameterType = memberSymbol.Symbol.ToSyntax(memberSymbol.Category.IsNullable());
                var parameter = parameterType.Parameter(parameterName.Identifier);
                parameters.Add(parameter);
                yield return parameterName;
                continue;
            }
            var sourceMemberSymbol = source.MemberSymbol;
            var member = owner.Access(source.Name);
            //var converter = SymbolReflection.GetConverter(sourceMemberSymbol, memberSymbol);


        }
        //builder.AddParameterDeclaration(name, memberSymbol.ToSyntax());
    }
    /// <summary>
    /// 属性操作器种类
    /// </summary>
    /// <param name="init"></param>
    /// <returns></returns>
    public static SyntaxKind[] ChecAccessorKinds(bool init)
    {
        if (init)
            return [SyntaxKind.GetAccessorDeclaration, SyntaxKind.InitAccessorDeclaration];
        return [SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration];
    }
    /// <summary>
    /// 获取属性字典
    /// </summary>
    /// <param name="type"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    public static IDictionary<string, IPropertySymbol> GetProperties(INamedTypeSymbol type, IRecognizer<string>[] rules)
    {
        IDictionary<string, IPropertySymbol> properties = SymbolReflection.GetPublicPropertiesWithBase(type)
            .ToDictionary(p => p.Name);
        foreach (var rule in rules)
            properties = rule.Recognize(properties);
        return properties;
    }
}
