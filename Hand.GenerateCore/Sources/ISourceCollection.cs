namespace Hand.Sources;

/// <summary>
/// 数据源集合
/// </summary>
public interface ISourceCollection : IGeneratorSource
{
    /// <summary>
    /// 添加数据源
    /// </summary>
    /// <param name="source"></param>
    void Add(IGeneratorSource source);
    ///// <summary>
    ///// 执行生成
    ///// </summary>
    ///// <param name="context"></param>
    //void Generate(SourceProductionContext context);
}
