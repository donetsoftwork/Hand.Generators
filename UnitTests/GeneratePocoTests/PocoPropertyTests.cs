using Hand;
using Hand.Entities;
using Hand.GeneratePoco;

namespace GeneratePocoTests;

public class PocoPropertyTests
{
    [Fact]
    public void Default()
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
    //    public int Id { get; set; }
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name { get; set; }

    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void Init()
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

[GeneratePoco<User>(Initializer = InitializeKind.Init)]
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
        Assert.Contains("init;", code);
    }
    //public record User(int Id, string Name);
    //partial class UserDto
    //{
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id { get; init; }
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name { get; init; }

    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void PropertyWithField()
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

[GeneratePoco<User>(Initializer = InitializeKind.Property | InitializeKind.Field)]
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
    //partial class UserDto
    //{
    //    private int _id;
    //    private string _name;
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
    [Fact]
    public void InitWithField()
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

[GeneratePoco<User>(Initializer = InitializeKind.Init | InitializeKind.Field)]
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
        Assert.Contains("init", code);
    }
    //public record User(int Id, string Name);
    //partial class UserDto
    //{
    //    private int _id;
    //    private string _name;
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id { get => _id; init => _id = value; }
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name { get => _name; init => _name = value; }

    //    public User ToUser() => new(_id, _name);
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

[GeneratePoco<User>(Default = true)]
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
    //    public int Id { get; set; } = default;
    //    public string Name { get; set; } = "";
    //    public User ToUser() => new(Id, Name);
    //}
    [Fact]
    public void PropertyWithFieldUseDefault()
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

[GeneratePoco<User>(Initializer = InitializeKind.Property | InitializeKind.Field, Default = true)]
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
    //partial class UserDto
    //{
    //    private int _id = default;
    //    private string _name = "";
    //    ///<summary>
    //    ///Id标识
    //    ///</summary>
    //    public int Id { get => _id; set => _id = value; }
    //    ///<summary>
    //    ///用户名
    //    ///</summary>
    //    public string Name { get => _name; set => _name = value; }

    //    public User ToUser() => new(_id, _name);
    //}
}
