namespace Hand.Sources;

/// <summary>
/// 生成器数据源
/// </summary>
public interface IGeneratorSource
{
    /// <summary>
    /// 生成文件名
    /// </summary>
    string GenerateFileName { get; }
    /// <summary>
    /// 生成
    /// </summary>
    /// <returns></returns>
    SyntaxGenerator Generate();
}
