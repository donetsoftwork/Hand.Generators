namespace SyntaxScriptingTests;

public class ScopeTests
{
    [Fact]
    public void Nested()
    {

    }

    public class User
    {
        public User(UserId id, UserName name)
        {
            Id = id;
            Name = name;
        }
        public UserId Id { get; }
        public UserName Name { get; }

        public record UserId(int Value);
        public record UserName(string Value);
    }
}
