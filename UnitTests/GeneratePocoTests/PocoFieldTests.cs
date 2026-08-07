using Hand;
using Hand.Entities;
using Hand.GeneratePoco;

namespace GeneratePocoTests;

public class PocoFieldTests
{
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
    }
    //public record User(int Id, string Name);
    //partial class UserDto
    //{
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id;
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name;
    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void UseDefault()
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

[GeneratePoco<User>(Initializer = InitializeKind.Field, Default = true)]
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
    }
    //public record User(int Id, string Name);
    //partial class UserDto
    //{
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id = default;
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name = "";
    //    public User ToUser() => new(Id, Name);
    //}
}
