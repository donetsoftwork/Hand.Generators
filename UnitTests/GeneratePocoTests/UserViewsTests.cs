using GeneratePocoTests.Supports;
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;

namespace GeneratePocoTests;

public class UserViewsTests
{
    [Fact]
    public void WithConvertTo()
    {
        var source = @"
using Hand;
using Hand.Entities;
using Hand.GeneratePoco;
using Hand.Models;
using GeneratePocoTests.Supports;

namespace GeneratePocoTests;

[GeneratePoco<UserEntity>(ConvertFrom = true)]
public partial class UserViews;
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GeneratePocoAttribute<>))
            .Reference<UserEntity>();
        var result = service.Generate<PocoGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Id", code);
        Assert.Contains("Name", code);
        Assert.Contains("ToUserEntity", code);
        var syntaxTree2 = result.GeneratedTrees.LastOrDefault();
        Assert.NotNull(syntaxTree2);
        var code2 = syntaxTree2.GetText().ToString();
        Assert.Contains("ToUserViews", code2);
        Assert.Contains("Name = @this.Name.Original", code2);
    }
}

[GeneratePoco<UserEntity>(Rules = ["Prefix User"])]
public partial class UserViews;