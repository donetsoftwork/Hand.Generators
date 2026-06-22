using Hand;
using Hand.Members;

namespace GenerateConvertTests.Members;

public class ConvertMethodInfoTests
{
    [Fact]
    public void ToInt64()
    {
        var code = @"var num = (long)1";
        var driver = SyntaxTreeDriver.CreateDriver();
        var compilation = driver.Compile(code);
        var intTypeName = compilation.GetIntSymbol().Name;
        Assert.Equal("Int32", intTypeName);
        var longTypeName = compilation.GetLongSymbol().Name;
        Assert.Equal("Int64", longTypeName);
        var info = ConvertMethodInfo.Create(longTypeName, intTypeName, true, "To");
        Assert.Equal("ToInt64", info.Name);
    }

    [Fact]
    public void ToDTO()
    {
        var info = ConvertMethodInfo.Create("UserDTO", "User", true, "To");
        Assert.Equal("ToDTO", info.Name);
        if(info is ConvertMethodAliasInfo aliasInfo)
        {
            Assert.Equal("ToUserDTO", aliasInfo.Alias);
        }
        else
        {
            Assert.Fail();
        }
    }
    [Fact]
    public void FromDTO()
    {
        var info = ConvertMethodInfo.Create("UserDTO", "User", true, "From");
        Assert.Equal("FromDTO", info.Name);
        if (info is ConvertMethodAliasInfo aliasInfo)
        {
            Assert.Equal("FromUserDTO", aliasInfo.Alias);
        }
        else
        {
            Assert.Fail();
        }
    }
    [Fact]
    public void ToUser()
    {
        var info = ConvertMethodInfo.Create("User", "UserDTO", true, "To");
        Assert.Equal("ToUser", info.Name);
    }
    [Fact]
    public void FromUser()
    {
        var info = ConvertMethodInfo.Create("User", "UserDTO", true, "From");
        Assert.Equal("FromUser", info.Name);
    }
}
