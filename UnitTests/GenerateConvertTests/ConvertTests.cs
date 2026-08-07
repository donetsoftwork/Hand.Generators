using Hand;
using Hand.Mapping;

namespace GenerateConvertTests;

public class ConvertTests
{
    [Fact]
    public void ToUser()
    {
        var source = @"
using Hand;
using Hand.Mapping;
using Hand.Models;

namespace GenerateConvertTests;

public record User(int Id, string Name);

[GenerateConvert<User>]
public partial record UserDto(int Id, string Name);
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>));
        var result = service.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.LastOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToUser", code);
    }
    [Fact]
    public void ToUserWithMaster()
    {
        var source = @"
using Hand;
using Hand.Mapping;
using Hand.Models;

namespace GenerateConvertTests;

public record User(int Id, string Name, User Master);

[GenerateConvert<User>]
public partial record UserDto(int Id, string Name, UserDto Master);
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>));
        var result = service.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.LastOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToUser", code);
    }
}
