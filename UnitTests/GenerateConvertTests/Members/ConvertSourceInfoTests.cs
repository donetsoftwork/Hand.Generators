using GenerateConvertTests.Supports;
using Hand;
using Hand.Converters;
using Hand.Converters.Methods;
using Hand.Members;
using Hand.Reflection;

namespace GenerateConvertTests.Members;

public class ConvertSourceInfoTests
{
    [Fact]
    public void IsStatic()
    {
        var serviceType = typeof(UserDTO2Services);
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(serviceType);
        var compilation = driver.Compile("");
        var type = compilation.GetTypeByMetadataName(serviceType.FullName!);
        Assert.NotNull(type);
        var method = SymbolReflection.GetMethods(type)
            .FirstOrDefault(m => m.Name == nameof(UserDTO2Services.ToUser2));
        Assert.NotNull(method);
        var converter = ConvertSourceInfo.GetConverter(method);
        Assert.True(converter is StaticMethodConverter);
    }
    [Fact]
    public void IsExtension()
    {
        var serviceType = typeof(UserDTO2Services);
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(serviceType);
        var compilation = driver.Compile("");
        var type = compilation.GetTypeByMetadataName(serviceType.FullName!);
        Assert.NotNull(type);
        var method = SymbolReflection.GetMethods(type)
            .FirstOrDefault(m => m.Name == nameof(UserDTO2Services.ToUser3));
        Assert.NotNull(method);
        var converter = ConvertSourceInfo.GetConverter(method);
        Assert.True(converter is MethodConverter);
    }
    [Fact]
    public void Instance()
    {
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<UserDTO2>();
        var compilation =driver.Compile("");
        var type = compilation.GetTypeByMetadataName(typeof(UserDTO2).FullName!);
        Assert.NotNull(type);
        var method = SymbolReflection.GetMethods(type)
            .FirstOrDefault(m => m.Name == nameof(UserDTO2.ToUser2));
        Assert.NotNull(method);
        var converter = ConvertSourceInfo.GetConverter(method);
        Assert.True(converter is MethodConverter);
    }
}
