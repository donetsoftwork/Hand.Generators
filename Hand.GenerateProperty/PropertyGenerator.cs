using Hand.Executors;
using Hand.Filters;
using Hand.Generators;
using Microsoft.CodeAnalysis;

namespace Hand.GenerateProperty;

/// <summary>
/// 生成实体属性
/// </summary>
[Generator(LanguageNames.CSharp)]
public class PropertyGenerator()
    : ValuesGenerator<PropertySource>(
        "Hand.Entities.GeneratePropertyAttribute", 
        new SyntaxFilter(), 
        PropertyTransform.Instance, 
        new GeneratorExecutor<PropertySource>())
{
}
