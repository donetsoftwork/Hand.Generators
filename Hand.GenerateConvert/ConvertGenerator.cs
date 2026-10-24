using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Hand.Sources;
using Microsoft.CodeAnalysis;

namespace Hand;

/// <summary>
/// 生成类型转化方法
/// </summary>
[Generator(LanguageNames.CSharp)]
public class ConvertGenerator()
    : ValuesGenerator<SourceCollection>(
        Attribute
        , new SyntaxFilter(false)
        , new ConvertTransform()
        , new GeneratorExecutor())
{
    /// <summary>
    /// Attribute标记
    /// </summary>
    public const string Attribute = "Hand.Mapping.GenerateConvertAttribute`1";
}
