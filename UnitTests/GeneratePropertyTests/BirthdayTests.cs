using Hand;
using Hand.Entities;
using Hand.GenerateProperty;
using Hand.Models;

namespace GeneratePropertyTests;

public class BirthdayTests
{
    //[Fact]
    //public void Test()
    //{
    //    var value = new DateOnly(1988, 8, 8);
    //    var birthday = new Birthday { Original = value };
    //    Assert.Equal(value, birthday.Original);
    //    var birthday2 = new Birthday { Original = value };
    //    Assert.Equal(birthday, birthday2);
    //    //Assert.True(birthday == birthday2);
    //}
    [Fact]
    public void Generate()
    {
        var source = @"
using Hand.Entities;
using Hand.Models;
namespace GeneratePropertyTests;

[GenerateProperty("")]
public readonly partial struct Birthday : IEntityProperty<DateOnly>
{
}
";
        var service = SyntaxTreeScript.CreateDefault()
            .Reference<IEntityProperty<DateOnly>>()
            .Reference<GeneratePropertyAttribute>();
        var result = service.Generate<PropertyGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Original", code);
    }
}
//[GenerateProperty("")]
//public readonly partial struct Birthday : IEntityProperty<DateOnly>
//{
//}