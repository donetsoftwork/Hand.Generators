using System.Runtime.Serialization;

namespace GenerateConvertTests.Supports;

/// <summary>
/// 字段类型
/// </summary>
[Flags]
public enum ColumnType : short
{
    /// <summary>
    /// 空
    /// </summary>
    [EnumMember(Value = "E")]
    Empty = 0,
    /// <summary>
    /// 自增列
    /// </summary>
    [EnumMember(Value = "I")]
    Identity = 1,
    /// <summary>
    /// 主键之一
    /// </summary>
    [EnumMember(Value = "K")]
    Key = 1 << 1,
    /// <summary>
    /// 唯一列
    /// </summary>
    [EnumMember(Value = "U")]
    Unique = 1 << 2,
    /// <summary>
    /// 非空
    /// </summary>
    [EnumMember(Value = "N")]
    NotNull = 1 << 3,
    /// <summary>
    /// 计算列
    /// </summary>
    [EnumMember(Value = "C")]
    Computed = 1 << 4
}
