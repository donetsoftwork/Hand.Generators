using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters;

/// <summary>
/// 扩展方法转化器
/// </summary>
/// <param name="namespace"></param>
/// <param name="method"></param>
/// <param name="isNullable"></param>
public class ExtensionMethodConverter(UsingDirectiveSyntax @namespace, SimpleNameSyntax method, bool isNullable = false)
     : InstanceMethodConverter(method, isNullable)
{
    /// <summary>
    /// 扩展方法转化器
    /// </summary>
    /// <param name="namespace"></param>
    /// <param name="method"></param>
    public ExtensionMethodConverter(string @namespace, SimpleNameSyntax method)
        : this(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@namespace)), method)
    {
    }
    #region 配置
    private readonly UsingDirectiveSyntax _namespace = @namespace;
    #endregion

    /// <inheritdoc />
    public override ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        generator.Using(_namespace);
        return base.Convert(generator, source);
    }
}
