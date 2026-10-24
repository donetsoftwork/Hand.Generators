using Hand.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hand.Enumerations.Converters;

/// <summary>
/// 枚举值转化为枚举类
/// </summary>
/// <param name="provider"></param>
public class OriginalConverter(EnumerationProvider provider)
    : ISyntaxConverter
{
    #region 配置
    private readonly EnumerationProvider _provider = provider;
    /// <summary>
    /// 枚举提供器
    /// </summary>
    public EnumerationProvider Provider
        => _provider;
    #endregion

    /// <inheritdoc />
    public ExpressionSyntax Convert(SyntaxGenerator generator, ExpressionSyntax source)
        => _provider.GetByOriginal(generator, source);
}