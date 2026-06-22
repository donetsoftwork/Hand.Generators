using Hand.Converters;
using Hand.Members;
using Hand.Sources;
using System.Collections.Generic;

namespace Hand.Builders;

/// <summary>
/// 提供转化器接口
/// </summary>
public interface IConvertBuilder
{
    /// <summary>
    /// 获取转化器
    /// </summary>
    /// <param name="fromInfo"></param>
    /// <param name="toInfo"></param>
    /// <param name="generators"></param>
    /// <returns></returns>
    public IConverter? GetConverter(MemberSymbolInfo fromInfo, MemberSymbolInfo toInfo, ICollection<IGeneratorSource> generators);
}
