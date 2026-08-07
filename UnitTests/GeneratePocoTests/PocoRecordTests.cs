using Hand;
using Hand.Entities;
using Hand.GeneratePoco;

namespace GeneratePocoTests;

public class PocoRecordTests
{
    [Fact]
    public void Record()
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

[GeneratePoco<User>()]
public partial record UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("record", code);
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    //public record User(int Id, string Name);
    /////<summary>
    /////UserDto
    /////</summary>
    /////<param name = "Id">Id标识</param>
    /////<param name = "Name">用户名</param>
    //partial record UserDto(int Id, string Name)
    //{
    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void RecordUseDefault()
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

[GeneratePoco<User>(Default = true)]
public partial record UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("record", code);
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    //public record User(int Id, string Name);
    //partial record UserDto(int Id = default, string Name = "")
    //{
    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void Field()
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

[GeneratePoco<User>(Initializer = InitializeKind.Field)]
public partial record UserDto;
";
        // record最好不要设置Initializer,相当于record退化为class
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    //public record User(int Id, string Name);
    //partial record UserDto
    //{
    //    public int Id;
    //    public string Name;
    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void Property()
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

[GeneratePoco<User>(Initializer = InitializeKind.Property)]
public partial record UserDto;
";
        // record最好不要设置Initializer,相当于record退化为class
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    //public record User(int Id, string Name);
    //partial record UserDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }

    //    public User ToUser() => new(Id, Name);
    //}
}
