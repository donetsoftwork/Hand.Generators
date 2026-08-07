using Hand.Models;

namespace GenerateConvertTests.Supports;

/// <summary>
/// 用户Id标识
/// </summary>
/// <param name="Original"></param>
public record UserId(long Original) : IEntityId;
