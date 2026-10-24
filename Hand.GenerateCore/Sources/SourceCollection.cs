using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Hand.Sources;

/// <summary>
/// 数据源集合
/// </summary>
/// <param name="items"></param>
public class SourceCollection(List<IGeneratorSource> items)
    : ISourceCollection
{
    #region 配置
    /// <summary>
    /// 子数据源列表
    /// </summary>
    protected readonly List<IGeneratorSource> _items = items;
    /// <summary>
    /// 子数据源列表
    /// </summary>
    public IEnumerable<IGeneratorSource> Items
        => _items;
    #endregion

    /// <summary>
    /// 添加数据源
    /// </summary>
    /// <param name="source"></param>
    public void Add(IGeneratorSource source)
        => _items.Add(source);
    /// <summary>
    /// 执行生成
    /// </summary>
    /// <param name="context"></param>
    public void Generate(SourceProductionContext context)
    {
        IGeneratorSource[] items;
        // 在执行过程中可能生成新的生成源
        while (_items.Count > 0)
        {
            items = [.. _items];
            _items.Clear();
            foreach (var item in items)
                item.Generate(context);
        }
    }
}
