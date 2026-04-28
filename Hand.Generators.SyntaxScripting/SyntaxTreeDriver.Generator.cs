using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Hand;

public partial class SyntaxTreeDriver
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
        return CreateGeneratorDriver(generator)
            .RunGenerators(compilation);
    }
    #endregion
    #region CreateGeneratorDriver
    /// <summary>
    /// 构造执行器驱动
    /// </summary>
    /// <typeparam name="TGenerator"></typeparam>
    /// <returns></returns>
    public CSharpGeneratorDriver CreateGeneratorDriver<TGenerator>()
        where TGenerator : IIncrementalGenerator, new()
        => CreateGeneratorDriver(new TGenerator());
    /// <summary>
    /// 构造执行器驱动
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    public CSharpGeneratorDriver CreateGeneratorDriver(IIncrementalGenerator generator)
         => CreateGeneratorDriver(generator.AsSourceGenerator());
    /// <summary>
    /// 构造执行器驱动
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    public CSharpGeneratorDriver CreateGeneratorDriver(ISourceGenerator generator)
    {
        // 参看: https://www.thinktecture.com/en/net/roslyn-source-generators-analyzers-code-fixes-testing/
        var driverOptions = new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, true, _path);
        return CSharpGeneratorDriver.Create([generator], parseOptions: _options, driverOptions: driverOptions);
    }
    #endregion
}
