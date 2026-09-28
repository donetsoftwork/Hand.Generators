using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Converters.Collections;

/// <summary>
/// 单个转集合
/// </summary>
public class ElementToCollectionConverter : ISyntaxConverter
{
    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => SyntaxGenerator.Collection(source);

    /// <summary>
    /// 默认实例
    /// </summary>
    public static readonly ElementToCollectionConverter Instance = new();
}
