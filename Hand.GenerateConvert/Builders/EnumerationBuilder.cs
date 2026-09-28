using Hand.Enums.Builders;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hand.Builders;

/// <summary>
/// 枚举类构造器
/// </summary>
/// <param name="builder"></param>
/// <param name="compilation"></param>
public class EnumerationBuilder(ConvertBuilder builder, Compilation compilation)
{
    #region 配置
    private readonly ConvertBuilder _builder = builder;
    private readonly Compilation _compilation = compilation;
    private readonly EnumBundleBuilder _bundles = new(compilation);
    #endregion
}
