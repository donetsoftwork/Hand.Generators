using Hand.Models;

namespace GenerateConvertTests.Supports;

public class UserEntity(UserId id, UserName name)
    : IEntity<UserId>
{
    public UserId Id { get; } = id;
    public UserName Name { get; } = name;
}

//public record struct UserId(long Original) : IEntityId;
public record struct UserName(string Original) : IEntityProperty<string>;