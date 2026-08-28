using GenerateConvertTests.DTO;
using Hand;
using Hand.Members;
using Hand.Providers;

namespace GenerateConvertTests.Providers;

public class MethodEmptyProviderTests
{
    [Fact]
    public void GetConvertMethod()
    {
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<UserDTO>();
        var compilation = driver.Compile("");
        var typeSymbol = compilation.GetTypeByMetadataName("GenerateConvertTests.DTO.UserDTO");
        Assert.NotNull(typeSymbol);

        var info = ConvertMethodInfo.Create("UserDTO", "User", true, "To");
        var method = MethodEmptyProvider.Instance.GetConvertMethod(info, typeSymbol);
        Assert.Null(method);
    }
}
