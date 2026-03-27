using Hand;
using Hand.Cache;
using Hand.GenerateCachedProperty;

namespace GenerateCachedPropertyTests;

public partial class PropertyTests
{
    [GenerateLazy("LazyTime")]
    public static DateTime Now { get; } = DateTime.Now;

    [Fact]
    public void GenerateLazyNow()
    {
        var source = @"
using Hand;
using Hand.Cache;
namespace GenerateCachedPropertyTests;

public partial class PropertyTests
{
    [GenerateLazy]
    public static DateTime Now { get; } = DateTime.Now;
}
";
        var service = SyntaxTreeScript.CreateDefault()
            .Reference<GenerateLazyAttribute>();
        var result = service.Generate<GenerateLazyGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("LazyNow", code);
    }
    [Fact]
    public void GenerateLazyTime()
    {
        var source = @"
using Hand;
using Hand.Cache;
namespace GenerateCachedPropertyTests;

public partial class PropertyTests
{
    [GenerateLazy(""LazyTime"")]
    public DateTime Now { get; } = DateTime.Now;
}
";
        var service = SyntaxTreeScript.CreateDefault()
            .Reference<GenerateLazyAttribute>();
        var result = service.Generate<GenerateLazyGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("LazyTime", code);
    }
}
