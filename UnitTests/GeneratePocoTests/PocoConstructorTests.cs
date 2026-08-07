using Hand;
using Hand.Entities;
using Hand.GeneratePoco;

namespace GeneratePocoTests;

public class PocoConstructorTests
{
    [Fact]
    public void Constructor()
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

[GeneratePoco<User>(Initializer = InitializeKind.Constructor)]
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
    //partial class UserDto(int id, string name)
    //{
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id { get; } = id;
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name { get; } = name;

    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void ConstructorWithField()
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

[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Field)]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("_id", code);
        Assert.Contains("_name", code);
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }
    //public record User(int Id, string Name);
    //partial class UserDto(int id, string name)
    //{
    //    private readonly int _id = id;
    //    private readonly string _name = name;
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id => _id;
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name => _name;

    //    public User ToUser() => new(_id, _name);
    //}
    [Fact]
    public void ConstructorSetProperty()
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

[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property)]
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
    //partial class UserDto(int id, string name)
    //{
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id { get; set; } = id;
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name { get; set; } = name;

    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void ConstructorSetPropertyWithField()
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

[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property | InitializeKind.Field)]
public partial class UserDto;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>));
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("_id", code);
        Assert.Contains("_name", code);
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
    }

//partial class UserDto(int id, string name)
//{
//    private int _id = id;
//    private string _name = name;
//    ///<summary>
//    ///Id标识
//    ///</summary>
//    public int Id { get => _id; set => _id = value; }
//    ///<summary>
//    ///用户名
//    ///</summary>
//    public string Name { get => _name; set => _name = value; }

//    ///<summary>
//    ///转化为用户
//    ///</summary>
//    public global::GeneratePocoTests.User ToUser() => new(_id, _name);
//}
}
