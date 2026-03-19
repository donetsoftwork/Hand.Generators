using System;

namespace GeneratePropertyTests.Sources;

/// <summary>
/// 生成代码块
/// </summary>
public class BlockScope : IDisposable
{
    private readonly SourceTextBuilder _builder;
    /// <summary>
    /// 生成代码块
    /// </summary>
    /// <param name="builder"></param>
    public BlockScope(SourceTextBuilder builder)
    {        
        _builder = builder;
        _builder.BuildBeigin();
    }
    /// <summary>
    /// 代码块结束
    /// </summary>
    public void Dispose()
    {
        _builder.BuildEnd();
    }
}
