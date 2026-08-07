using GeneratePocoTests.Supports;
using Hand.Entities;

namespace GeneratePocoTests;

[GeneratePoco<User>(Rules = ["Prefix User"])]
public partial class UserViewsTests;