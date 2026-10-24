//using Microsoft.CodeAnalysis;

//namespace Hand.Sources;

///// <summary>
///// 递归生成器数据源
///// </summary>
///// <param name="master"></param>
///// <param name="children"></param>
//public sealed class CompositeSource(IGeneratorSource master, ISourceCollection children)
//    : GeneratorSourceBase
//{
//    #region 配置
//    private readonly IGeneratorSource _master = master;
//    /// <summary>
//    /// 子数据源列表
//    /// </summary>
//    private readonly ISourceCollection _children = children;
//    /// <summary>
//    /// 子数据源列表
//    /// </summary>
//    public ISourceCollection Children
//        => _children;
//    /// <inheritdoc />
//    public override string GenerateFileName
//        => _master.GenerateFileName;
//    #endregion

//    /// <inheritdoc />
//    public override void Generate(SourceProductionContext context)
//    {
//        _master.Generate(context);
//        _children.Generate(context);
//    }
//    /// <inheritdoc />
//    public override SyntaxGenerator Generate()
//        => _master.Generate();
//}
