using Hand.Entities;

namespace GeneratePocoTests.Supports;

[GeneratePoco(typeof(UserEntity), Rules = ["Prefix User"])]
public partial class UserViewsTests;