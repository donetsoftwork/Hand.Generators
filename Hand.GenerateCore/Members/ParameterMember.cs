using Hand.Cachers;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Hand.Members;

/// <summary>
/// 参数成员
/// </summary>
/// <param name="name"></param>
/// <param name="original"></param>
/// <param name="symbolInfo"></param>
public class ParameterMember(string name, IParameterSymbol original, TypeSymbolInfo symbolInfo)
    : SymbolMember(name, original, symbolInfo)
{
    /// <inheritdoc />
    public override MemberKind Kind
        => MemberKind.Parameter;
    /// <summary>
    /// 原始参数
    /// </summary>
    public new IParameterSymbol Original { get; } = original;

    /// <summary>
    /// 获取方法参数成员
    /// </summary>
    /// <param name="typeSymbols"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public static Dictionary<string, SymbolMember> GetMethodParameters(TypeSymbolCacher typeSymbols, IMethodSymbol? method)
    {
        if (method == null)
            return new Dictionary<string, SymbolMember>(StringComparer.OrdinalIgnoreCase);
        var parameters = method.Parameters;
        var members = new Dictionary<string, SymbolMember>(parameters.Length, StringComparer.OrdinalIgnoreCase);
        foreach (var parameter in parameters)
        {
            var symbol = typeSymbols.GetByType(parameter.Type);
            if (symbol is null)
                continue;
            //var name = CamelWordRule.FistToLower(parameter.Name);
            var name = parameter.Name;
            members.Add(name, new ParameterMember(name, parameter, symbol));
        }
        return members;
    }
}
