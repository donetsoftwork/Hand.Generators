using Hand.Builders;
using Hand.Members;
using Hand.Types;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Parameters;

/// <summary>
/// 参数定义成员
/// </summary>
/// <param name="name"></param>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
/// <param name="summary"></param>
public class ParameterSyntaxMember(string name, ParameterSyntax original, ITypeSymbolInfo symbolInfo, string summary)
    : SyntaxMember(name, original, symbolInfo, summary), IMemberInfo
{
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.ParameterDeclaration;
    /// <summary>
    /// 原始参数
    /// </summary>
    public new ParameterSyntax Original { get; } = original;
    /// <inheritdoc />
    public override bool IsPublic => true;
    /// <summary>
    /// 转化参数定义成员
    /// </summary>
    /// <param name="types"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>

    public static ParameterSyntaxMember? Convert(TypeInfoBuilder types, ParameterSyntax parameter)
    {        
        var compilation = types.Compilation;
        var parameterType = parameter.Type;
        if(parameterType is null)
            return null;
        var symbol = compilation.GetSymbol(parameterType);
        if (symbol is null)
            return null;
        var symbolInfo = types.Get(symbol);
        var name = parameter.Identifier.ValueText;
        return new(name, parameter, symbolInfo, name);
    }
}
