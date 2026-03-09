using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Builders;

/// <summary>
/// 属性处理器构造
/// </summary>
/// <param name="accessor"></param>
public class AccessorBodyBuilder(AccessorDeclarationSyntax accessor)
    : BodyBuilder<AccessorDeclarationSyntax>(accessor)
{
    /// <inheritdoc />
    protected internal override AccessorDeclarationSyntax BuildCore()
        => _parent.WithBody(BuildBody());
}
