using Hand.Executors;
using Hand.Sources;
using Microsoft.CodeAnalysis;

namespace Hand.GeneratePoco;

/// <summary>
/// Poco执行器
/// </summary>
public class PocoExecutor
    : GeneratorExecutor<IGeneratorSource>
    , IGeneratorExecutor<PocoSource>
{
    void IGeneratorExecutor<PocoSource>.Execute(SourceProductionContext context, PocoSource source)
    {
        Execute(context, source);
        var items = source.ConvertBuilder.Sources;
        foreach (var item in items)
            Execute(context, item);
    }
}
