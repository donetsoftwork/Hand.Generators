using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Microsoft.CodeAnalysis;

namespace Hand.GeneratePoco;

/// <summary>
/// 生成Poco属性
/// </summary>
[Generator(LanguageNames.CSharp)]
public class PocoGenerator()
    : ValuesGenerator<PocoSource>(
        Attribute,
        new SyntaxFilter(),
        PocoTransform.Instance,
        new GeneratorExecutor<PocoSource>())
{
    /// <summary>
    /// Attribute标记
    /// </summary>
    public const string Attribute = "Hand.Entities.GeneratePocoAttribute";
}
