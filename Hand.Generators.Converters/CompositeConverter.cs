using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Hand.Converters;

/// <summary>
/// 组合转化器
/// </summary>
/// <param name="converters"></param>
public class CompositeConverter(params List<ISyntaxConverter> converters)
    : ISyntaxConverter
{
    #region 配置
    private readonly List<ISyntaxConverter> _converters = converters;

    /// <summary>
    /// 转化器
    /// </summary>
    public IEnumerable<ISyntaxConverter> Converters 
        => _converters;
    #endregion

    /// <summary>
    /// 添加转化器
    /// </summary>
    /// <param name="converter"></param>
    public void Add(ISyntaxConverter converter)
        => _converters.Add(converter);
    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
    {
        foreach (var converter in _converters)
            source = converter.Convert(generator, source);
        return source;
    }
}
