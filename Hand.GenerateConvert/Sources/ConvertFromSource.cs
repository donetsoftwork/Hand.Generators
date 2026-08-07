using Hand.Converters;
using Hand.Creational;
using Hand.Members;

namespace Hand.Sources;

/// <summary>
/// 转化自生成源
/// </summary>
public class ConvertFromSource(MethodSource original, ConvertSourceInfo info)
    : ExtensionMethodSource(original, info.TypeInfo)
    , ICreator<IConverter>
{
    private readonly ConvertSourceInfo _info = info;

    /// <inheritdoc />
    public IConverter Create()
        => _info.Create();
}
