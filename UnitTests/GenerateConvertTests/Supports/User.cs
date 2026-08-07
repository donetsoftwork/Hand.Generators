namespace GeneratePocoTests.Supports;

/// <summary>
/// 用户
/// </summary>
/// <param name="Id">Id标识</param>
/// <param name="Name">用户名</param>
/// <param name="Sex">性别</param>
public record User(long Id, string Name, int Sex);