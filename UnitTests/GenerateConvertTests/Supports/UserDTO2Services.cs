namespace GenerateConvertTests.Supports;

public static class UserDTO2Services
{
    public static User ToUser2(UserDTO2 user)
        => new(user.Id, user.Name, user.Sex);
    public static User ToUser3(this UserDTO2 user)
        => new(user.Id, user.Name, user.Sex);
}
