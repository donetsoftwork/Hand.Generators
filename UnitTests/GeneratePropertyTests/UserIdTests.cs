using Hand;
using Hand.Entities;
using Hand.GenerateProperty;
using Hand.Models;

namespace GeneratePropertyTests;

public class UserIdTests
{
    //[Fact]
    //public void Test()
    //{
    //    var value = 1L;
    //    var userId = new UserId(value);
    //    Assert.Equal(value, userId.Original);
    //    var userId2 = new UserId(1);
    //    Assert.Equal(userId, userId2);
    //    Assert.True(userId == userId2);
    //}
    //[Fact]
    //public void NotEqual()
    //{
    //    var userId = new UserId(1L);
    //    var userId2 = new UserId(2L);
    //    Assert.NotEqual(userId, userId2);
    //    Assert.True(userId != userId2);
    //    Assert.False(userId == userId2);
    //}
    [Fact]
    public void Generate()
    {
        var source = @"
using Hand.Entities;
using Hand.Models;
namespace GeneratePropertyTests;

[GenerateProperty]
public partial class UserId : IEntityId;
";
        var service = SyntaxTreeScript.Create()
            .Reference<IEntityId>()
            .Reference<GeneratePropertyAttribute>();
        var result = service.Generate<PropertyGenerator>(source, out var diagnostics);
        var syntaxTree = result.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Original", code);
        Assert.Contains("ToString()", code);
        Assert.Contains("GetHashCode()", code);
        Assert.Contains("operator", code);
    }
}

//[GenerateProperty]
//public partial class UserId : IEntityId;