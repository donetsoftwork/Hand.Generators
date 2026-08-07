using GeneratePocoTests.Supports;
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;

namespace GeneratePocoTests;

public class UserDtoTests
{
    [Fact]
    public void WithExclude()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

/// <summary>
/// 用户
/// </summary>
/// <param name=""Id"">Id标识</param>
/// <param name=""Name"">用户名</param>
public record User(int Id, string Name);

[GeneratePoco<User>(Rules = [""Exclude: Name""])]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.DoesNotContain("Name", code);
    }
    [Fact]
    public void WithPrefix()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

/// <summary>
/// 用户
/// </summary>
/// <param name=""Id"">Id标识</param>
/// <param name=""Name"">用户名</param>
public record User(int Id, string Name);

[GeneratePoco<User>(Rules = [""Prefix User""])]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("UserId", code);
        Assert.Contains("UserName", code);
    }
    [Fact]
    public void WithCross()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

/// <summary>
/// 用户
/// </summary>
/// <param name=""Id"">Id标识</param>
/// <param name=""Name"">用户名</param>
public record User(int Id, string Name);

[GeneratePoco<User>(Rules = [""Cross: Prefix User""])]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("UserId", code);
        Assert.Contains("UserName", code);
    }
    [Fact]
    public void WithId()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

/// <summary>
/// 用户
/// </summary>
/// <param name=""Id"">Id标识</param>
/// <param name=""Name"">用户名</param>
public record User(int? Id, string Name);

[GeneratePoco<User>()]
public partial class UserDto
{
    public string? Id { get; set; }
}
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Name", code);
    }
    //public partial class UserDto
    //{
    //    public string? Id { get; set; }
    //}
    //public record User(int Id, string Name);
    //partial class UserDto
    //{
    //    public string Name { get; set; }
    //    public User ToUser() => new(System.Convert.ToInt32(Id), Name);
    //}
    [Fact]
    public void WithullableRule()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

/// <summary>
/// 用户
/// </summary>
/// <param name=""Id"">Id标识</param>
/// <param name=""Name"">用户名</param>
/// <param name=""Sex"">性别</param>
public record User(int Id, string Name, int Sex);

[GeneratePoco<User>(Rules = [""Exclude: Id"",""Prefix User"", ], NullableRule = ""UserSex"")]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.DoesNotContain("UserId", code);
        Assert.Contains("UserName", code);
    }
    [Fact]
    public void WithMaster()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;

namespace GeneratePocoTests;

/// <summary>
/// 用户
/// </summary>
/// <param name=""Id"">Id标识</param>
/// <param name=""Name"">用户名</param>
/// <param name=""Master"">直属领导</param>
public record User(int Id, string Name, User? Master);

[GeneratePoco<User>()]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
        Assert.Contains("Master", code);
    }
}


[GeneratePoco<User>(
    Rules =
    [
        "Exclude: Id",
        "Prefix User"
    ],
    NullableRule = "UserSex"
)]
public partial class UserDto;
