using System.Runtime.Serialization;

namespace GenerateConvertTests.DTO;

public enum MyColorDTO : int
{
    None = 0,
    [EnumMember(Value = "R")]
    Red = 1,
    [EnumMember(Value = "G")]
    Green = 2,
    [EnumMember(Value = "B")]
    Blue = 4,
}
