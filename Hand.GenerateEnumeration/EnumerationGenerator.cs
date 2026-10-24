using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Hand.Sources;
using Microsoft.CodeAnalysis;

namespace Hand.GenerateEnumeration;

/// <summary>
/// 生成枚举类
/// </summary>

[Generator(LanguageNames.CSharp)]
public class EnumerationGenerator()
    : ValuesGenerator<IGeneratorSource>(
        Attribute,
        new SyntaxFilter(),
        new EnumerationTransform(),
        new GeneratorExecutor())
{
    /// <summary>
    /// Attribute标记
    /// </summary>
    public const string Attribute = "Hand.Primitives.GenerateEnumerationAttribute`1";
}
