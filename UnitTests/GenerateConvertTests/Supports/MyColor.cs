using System.Runtime.Serialization;

namespace GenerateConvertTests.Supports;

/// <summary>
/// MyColor
/// </summary>
[Flags]
public enum MyColor : int
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Red
    /// </summary>
    [EnumMember(Value = "R")]
    Red = 1,
    /// <summary>
    /// Green
    /// </summary>
    [EnumMember(Value = "G")]
    Green = 2,
    /// <summary>
    /// Blue
    /// </summary>
    [EnumMember(Value = "B")]
    Blue = 4,
}
