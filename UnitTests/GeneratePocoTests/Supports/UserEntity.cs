using Hand.Models;
using Hand.Primitives;

namespace GeneratePocoTests.Supports;

public class UserEntity(UserId id, UserName name)
    : IEntity<UserId>
{
    public UserId Id { get; } = id;
    public UserName Name { get; } = name;
}
/// <summary>
/// 用户Id标识
/// </summary>
/// <param name="Original"></param>
public record struct UserId(long Original) : IEntityId;
/// <summary>
/// 用户名
/// </summary>
/// <param name="Original"></param>
public record struct UserName(string Original) : IEntityProperty<string>;