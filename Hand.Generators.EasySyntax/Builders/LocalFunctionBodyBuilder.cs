using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Hand.Builders;

/// <summary>
/// 局部函数构造
/// </summary>
/// <param name="function"></param>
/// <param name="parameters"></param>
public class LocalFunctionBodyBuilder(LocalFunctionStatementSyntax function, List<ParameterSyntax> parameters)
    : BodyBuilder<LocalFunctionStatementSyntax>(function)
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="function"></param>
    public LocalFunctionBodyBuilder(LocalFunctionStatementSyntax function)
        : this(function, [])
    {
    }
    /// <summary>
    /// 参数列表
    /// </summary>
    private List<ParameterSyntax> _parameters = parameters;

    /// <summary>
    /// 添加参数
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public LocalFunctionBodyBuilder AddParameter(ParameterSyntax parameter)
    {
        _parameters.Add(parameter);
        return this;
    }

    /// <inheritdoc />
    protected internal override LocalFunctionStatementSyntax BuildCore()
        => _parent.AddParameterListParameters([.. _parameters]).WithBody(BuildBody());
}
