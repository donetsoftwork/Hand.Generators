using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Hand.Sources;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand;

/// <summary>
/// 生成类型转化方法
/// </summary>
[Generator(LanguageNames.CSharp)]
public class ConvertGenerator()
    : ValuesGenerator<IEnumerable<IGeneratorSource>>(
        Attribute
        , new SyntaxFilter()
        , new ConvertTransform()
        , new GeneratorExecutor<IGeneratorSource>())
{
    /// <summary>
    /// Attribute标记
    /// </summary>
    public const string Attribute = "Hand.Mapping.GenerateConvertAttribute`1";
}
