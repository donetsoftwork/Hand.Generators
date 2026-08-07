using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 局部函数构造
/// </summary>
/// <param name="function"></param>
public class LocalFunctionBodyBuilder(LocalFunctionStatementSyntax function)
    : BodyBuilder<LocalFunctionStatementSyntax>(function)
{
    /// <inheritdoc />
    protected internal override LocalFunctionStatementSyntax BuildCore()
        => _parent.WithBody(BuildBody());
}
