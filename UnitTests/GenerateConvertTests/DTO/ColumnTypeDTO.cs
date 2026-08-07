namespace GenerateConvertTests.DTO;

/// <summary>
/// 字段类型
/// </summary>
[Flags]
public enum ColumnTypeDTO : int
{
    /// <summary>
    /// 空
    /// </summary>
    Empty = 0,
    /// <summary>
    /// 自增列
    /// </summary>
    Identity = 1,
    /// <summary>
    /// 主键之一
    /// </summary>
    Key = 1 << 1,
    /// <summary>
    /// 唯一列
    /// </summary>
    Unique = 1 << 2,
    /// <summary>
    /// 非空
    /// </summary>
    NotNull = 1 << 3,
    /// <summary>
    /// 计算列
    /// </summary>
    Computed = 1 << 4
}
