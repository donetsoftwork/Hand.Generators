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
        var service = SyntaxTreeScript.Create()
            .Reference<IEntityProperty<int?>>()
            .Reference<GeneratePropertyAttribute>();
        var result = service.Generate<PropertyGenerator>(source, out var diagnostics);
        var syntaxTree = result.FirstOrDefault();
        // 已经含属性或构造函数,不再生成
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Equals", code);
        Assert.Contains("ToString", code);
    }
}



//[GenerateProperty("Exclude: Operator")]
//public partial class UserAge(int? original) : IEntityProperty<int?>
//{
//    public int? Original { get; } = original;
//}
