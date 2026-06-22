using GenerateConvertTests.Supports;
using Hand;
using Hand.Enums;

namespace GenerateConvertTests.Enums;

public class FlagEnumBundleTests
{
    [Fact]
    public void GetFieldsByName()
    {
        var source = "var color = MyColor.Red;";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<ColumnType>()
            .Using("System");
        var compilation = driver.Compile(source);
        var type = compilation.GetTypeByMetadataName("GenerateConvertTests.Supports.ColumnType");
        Assert.NotNull(type);
        var builder = new EnumBundleBuilder(compilation);
        var bundle = builder.Get(type);
        Assert.NotNull(bundle);
        var key = bundle.GetFieldByName("Key");
        Assert.NotNull(key);

        if (bundle is FlagEnumBundle enumBundle)
        {
            var flagFlag = enumBundle.GetFieldsByFlag(2);
            Assert.NotNull(flagFlag);
            var fields = enumBundle.GetFieldsByName("Key", "N").ToArray();
            Assert.Equal(2, fields.Length);
            Assert.Equal(6, enumBundle.Fields.Count);
        }
        else
        {
            Assert.False(false);
        }
    }
}
