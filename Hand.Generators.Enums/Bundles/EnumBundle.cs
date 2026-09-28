using Hand.Enums.Fields;
using System.Collections.Generic;

namespace Hand.Enums.Bundles;

/// <summary>
/// 枚举配置
/// </summary>
/// <param name="fields"></param>
public class EnumBundle(List<EnumField> fields)
    : EnumBundleBase<EnumField>(fields)
{
}
