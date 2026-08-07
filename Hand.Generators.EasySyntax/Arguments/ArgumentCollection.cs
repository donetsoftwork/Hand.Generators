using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Arguments;

/// <summary>
/// 参数集合
/// </summary>
/// <param name="arguments"></param>
public class ArgumentCollection(List<ArgumentSyntax> arguments)
    : IArgumentCollection
{
    /// <summary>
    /// 参数集合
    /// </summary>
    public ArgumentCollection()
        : this([]) 
    { 
    }
    /// <summary>
    /// 参数集合
    /// </summary>
    protected readonly List<ArgumentSyntax> _arguments = arguments;

    /// <inheritdoc />
    public void Add(ArgumentSyntax argument)
        => _arguments.Add(argument);
    /// <inheritdoc />
    public IEnumerable<ArgumentSyntax> GetArguments()
        => _arguments;
}
