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
    : ValuesGenerator<ConvertToSource>(
        Attribute
        , new SyntaxFilter(false)
        , new ConvertTransform()
        , new ConvertExecutor())
{
    /// <summary>
    /// Attribute标记
    /// </summary>
    public const string Attribute = "Hand.Mapping.GenerateConvertAttribute`1";
}
