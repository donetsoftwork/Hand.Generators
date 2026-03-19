using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hand;

public partial class SyntaxTreeScript
{
    #region Generate
    /// <summary>
    /// 增量生成
    /// </summary>
    /// <typeparam name="TGenerator"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public GeneratorDriver Generate<TGenerator>(string source)
        where TGenerator : IIncrementalGenerator, new()
        => Generate(new TGenerator(), source);
    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public GeneratorDriver Generate(IIncrementalGenerator generator, string source)
        => Generate(generator.AsSourceGenerator(), source);
    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public GeneratorDriver Generate(ISourceGenerator generator, string source)
    {
        var compilation = Compile(source);
        return CreateDriver(generator)
            .RunGenerators(compilation);
        //var runResult = driver.GetRunResult();
        //diagnostics = runResult.Diagnostics;
        //return runResult.GeneratedTrees;
    }
    #endregion
    #region CreateDriver
    /// <summary>
    /// 构造执行器驱动
    /// </summary>
    /// <typeparam name="TGenerator"></typeparam>
    /// <returns></returns>
    public CSharpGeneratorDriver CreateDriver<TGenerator>()
        where TGenerator : IIncrementalGenerator, new()
        => CreateDriver(new TGenerator());
    /// <summary>
    /// 构造执行器驱动
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    public CSharpGeneratorDriver CreateDriver(IIncrementalGenerator generator)
         => CreateDriver(generator.AsSourceGenerator());
    /// <summary>
    /// 构造执行器驱动
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    public CSharpGeneratorDriver CreateDriver(ISourceGenerator generator)
    {
        // 参看: https://www.thinktecture.com/en/net/roslyn-source-generators-analyzers-code-fixes-testing/
        var driverOptions = new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, true, _path);
        return CSharpGeneratorDriver.Create([generator], parseOptions: _options, driverOptions: driverOptions);
    }
    #endregion
}
