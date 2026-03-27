using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hand.GenerateCachedProperty;

/// <summary>
/// 生成延迟缓存属性
/// </summary>
[Generator(LanguageNames.CSharp)]
public class GenerateLazyGenerator()
    : ValuesGenerator<GenerateLazySource>(
        Attribute,
        new SyntaxFilter(false, SyntaxKind.PropertyDeclaration, SyntaxKind.MethodDeclaration),
        GenerateLazyTransform.Instance,
        new GeneratorExecutor<GenerateLazySource>())
{
    /// <summary>
    /// Attribute标记
    /// </summary>
    public const string Attribute = "Hand.Cache.GenerateLazyAttribute";
}
