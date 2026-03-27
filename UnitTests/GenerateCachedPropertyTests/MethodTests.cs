using Hand;
using Hand.Cache;
using Hand.GenerateCachedProperty;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GenerateCachedPropertyTests;

public partial class MethodTests
{
    [GenerateLazy("LazyTime")]
    public DateTime CreateTime()
    {
        return DateTime.Now;
    }
    [Fact]
    public void GenerateLazyTime()
    {
        MethodTests tests = new();
        //tests.LazyTime
        var source = @"
using Hand;
using Hand.Cache;
namespace GenerateCachedPropertyTests;

public partial class MethodTests
{
    [GenerateLazy(""LazyTime"")]
    public DateTime CreateTime()
    {
        return DateTime.Now;
    }
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
    [Fact]
    public void Version()
    {
        Console.WriteLine($"Framework: {RuntimeInformation.FrameworkDescription}");
        //Console.WriteLine($".NET Version: {RuntimeInformation.FrameworkDescription.Split(' ')[1]}");
        Console.WriteLine($".NET Version: {Environment.Version}");
        var versionString = typeof(object).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        Console.WriteLine($".NET Version: {versionString}");
        var majorVersion = GetFrameworkMajorVersion();
        Assert.True(majorVersion > 0);
    }

    public static int GetFrameworkMajorVersion()
    {
        var attribute = typeof(object).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if(attribute is null)
            return 0;
        ReadOnlySpan<char> versionString = attribute.InformationalVersion;
        int plusIndex = versionString.IndexOf('.');
        if (plusIndex >= 0 && int.TryParse(versionString.Slice(0, plusIndex), out var majorVersion))
            return majorVersion;
        return 0;
    }

}
