using GenerateConvertTests.Supports;
using Hand;
using Hand.Enums;

namespace GenerateConvertTests.Enums;

public class EnumBundleTests
{
    [Fact]
    public void GetFieldByName()
    {
        var source = "var color = ConsoleColor.Red;";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enum>()
            .Reference<ConsoleColor>()
            .Using("System");
        var compilation = driver.Compile(source);
        var type = compilation.GetTypeByMetadataName("System.ConsoleColor");
        Assert.NotNull(type);
        var builder = new EnumBundleBuilder(compilation);
        var bundle = builder.Get(type);
        Assert.NotNull(bundle);
        var red = bundle.GetFieldByName("red");
        Assert.NotNull(red);

        if (bundle is EnumBundle enumBundle)
        {
            Assert.Equal(16, enumBundle.Fields.Count);
        }
        else
        {
            Assert.False(false);
        }
    }
    [Fact]
    public void GetFieldByMemberName()
    {
        var source = "var color = MyColor.Red;";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enum>()
            .Reference<MyColor>()
            .Using("System");
        var compilation = driver.Compile(source);
        var type = compilation.GetTypeByMetadataName("GenerateConvertTests.Supports.MyColor");
        Assert.NotNull(type);
        var builder = new EnumBundleBuilder(compilation);
        var bundle = builder.Get(type);
        Assert.NotNull(bundle);
        var red = bundle.GetFieldByMemberName("r");
        Assert.NotNull(red);

        if (bundle is EnumBundle enumBundle)
        {
            Assert.Equal(4, enumBundle.Fields.Count);
        }
        else
        {
            Assert.False(false);
        }
    }
}
