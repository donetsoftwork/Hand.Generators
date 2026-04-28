using Hand.Filters;
using Hand.Transform;
using Microsoft.CodeAnalysis;
using System;

namespace Hand.Generators;

/// <summary>
/// 生成代码基类
/// </summary>
public abstract class AttributeGenerator<TSource>(string attributeName, ISyntaxFilter filter, IGeneratorTransform<TSource> transform)
    : IIncrementalGenerator
{
    #region 配置
    private readonly string _attributeName = attributeName;
    private readonly ISyntaxFilter _filter = filter;
    private readonly IGeneratorTransform<TSource> _transform = transform;
    /// <summary>
    /// 特性标记完整类名
    /// </summary>
    public string AttributeName
        => _attributeName;
    /// <summary>
    /// 匹配
    /// </summary>
    public ISyntaxFilter Filter
        => _filter;
    /// <summary>
    /// 转化
    /// </summary>
    public IGeneratorTransform<TSource> Transform
        => _transform;
    #endregion
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //#if DEBUG
        //        System.Diagnostics.Debugger.Launch();
        //#endif
        var provider = GenerateProvider.CreateByAttribute(context, _attributeName, _filter, _transform);
        Initialize(context, provider);
    }
    /// <summary>
    /// 处理数据
    /// </summary>
    /// <param name="context"></param>
    /// <param name="provider"></param>
    protected abstract void Initialize(IncrementalGeneratorInitializationContext context, IncrementalValuesProvider<TSource> provider);
}
