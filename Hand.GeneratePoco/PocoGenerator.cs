using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Hand.Sources;
using Microsoft.CodeAnalysis;
//using System.Text.Json.SourceGeneration;

namespace Hand.GeneratePoco;

/// <summary>
/// 生成Poco属性
/// </summary>
[Generator(LanguageNames.CSharp)]
//[GeneratorDependency(typeof(JsonSourceGenerator))]
public class PocoGenerator()
    : ValuesGenerator<SourceCollection>(
        Attribute,
        new SyntaxFilter(),
        new PocoTransform(),
        new GeneratorExecutor())
{
    /// <summary>
    /// Attribute标记
    /// </summary>
    public const string Attribute = "Hand.Entities.GeneratePocoAttribute`1";
}
