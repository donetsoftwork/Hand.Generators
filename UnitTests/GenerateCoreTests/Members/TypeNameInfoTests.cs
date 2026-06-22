using Hand;
using Hand.Members;

namespace GenerateCoreTests.Members;

public class TypeNameInfoTests
{
    [Fact]
    public void GetInfo()
    {
        var code = @"var num = 1";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(code);
        var intType = compilation.GetIntSymbol();
        var info = TypeNameInfo.GetInfo(intType);
        Assert.Equal("Int32", info.TypeName);
        Assert.Equal("System", info.Namespace);
        Assert.Equal("System.Int32", info.FullName);
    }
    [Fact]
    public void GetExtensionInfo()
    {
        var code = @"var num = 1";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(code);
        var intType = compilation.GetIntSymbol();
        var info = TypeNameInfo.GetExtensionInfo(intType);
        Assert.Equal("Int32Extensions", info.TypeName);
        Assert.Equal("System", info.Namespace);
        Assert.Equal("System.Int32Extensions", info.FullName);
    }
}
