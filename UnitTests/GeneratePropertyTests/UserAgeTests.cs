using Hand;
using Hand.Entities;
using Hand.GenerateProperty;
using Hand.Models;

namespace GeneratePropertyTests;

public class UserAgeTests
{
    [Fact]
    public void Generate()
    {
        var source = @"
using Hand.Entities;
using Hand.Models;
namespace GeneratePropertyTests;

[GenerateProperty(""Exclude: Operator"")]
public partial class UserAge(int? original) : IEntityProperty<int?>
{
    public int? Original { get; } = original;
}
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<IEntityProperty<int?>>()
            .Reference<GeneratePropertyAttribute>();
        var result = service.Generate<PropertyGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        // 已经含属性或构造函数,不再生成
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Equals", code);
        Assert.Contains("ToString", code);
    }
}



[GenerateProperty("Exclude: Operator")]
public partial class UserAge(int? original) : IEntityProperty<int?>
{
    public int? Original { get; } = original;
}
