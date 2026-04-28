using GeneratePocoTests.Supports;
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;

namespace GeneratePocoTests;

public class UserDtoTests
{
    [Fact]
    public void Generate()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

public record User(int Id, string Name);

[GeneratePoco(typeof(User))]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<GeneratePocoAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    [Fact]
    public void GenerateWithInit()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

public record User(int Id, string Name);

[GeneratePoco(typeof(User), Init = true)]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<GeneratePocoAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
        Assert.Contains("init;", code);
    }
    [Fact]
    public void GenerateWithExclude()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

public record User(int Id, string Name);

[GeneratePoco(typeof(User), Rules = [""Exclude: Name""])]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<GeneratePocoAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.DoesNotContain("Name", code);
    }
    [Fact]
    public void GenerateWithPrefix()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

public record User(int Id, string Name);

[GeneratePoco(typeof(User), Rules = [""Prefix User""])]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<GeneratePocoAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("UserId", code);
        Assert.Contains("UserName", code);
    }
    [Fact]
    public void GenerateWithCross()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

public record User(int Id, string Name);

[GeneratePoco(typeof(User), Rules = [""Cross: Prefix User""])]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<GeneratePocoAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("UserId", code);
        Assert.Contains("UserName", code);
    }
    [Fact]
    public void GenerateWithRules()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

public record User(int Id, string Name, int Sex);

[GeneratePoco(typeof(User), Rules = [""Exclude: Id"",""Prefix User"", ], NullableRule = ""UserSex"")]
public partial class NewUserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<GeneratePocoAttribute>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.DoesNotContain("UserId", code);
        Assert.Contains("UserName", code);
    }
}


[GeneratePoco(typeof(User), 
    Rules =
    [
        "Exclude: Id",
        "Prefix User"
    ],
    NullableRule = "UserSex"
)]
public partial class UserDto;
