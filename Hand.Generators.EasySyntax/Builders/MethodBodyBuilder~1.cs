using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 函数构造器
/// </summary>
/// <param name="method"></param>
public class MethodBodyBuilder<TMethod>(TMethod method)
    : BodyBuilder<TMethod>(method)
    where TMethod : BaseMethodDeclarationSyntax
{
    /// <inheritdoc />
    protected internal override TMethod BuildCore()
        => (TMethod)_parent.WithBody(BuildBody());
}
