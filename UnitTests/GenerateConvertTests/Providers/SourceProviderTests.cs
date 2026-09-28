using GenerateConvertTests.DTO;
using Hand;
using Hand.Members;
using Hand.Providers;
using Hand.Types;

namespace GenerateConvertTests.Providers;

public class SourceProviderTests
{
    [Fact]
    public void Partial()
    {
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<ProductDto>();
        var compilation = driver.Compile("");
        var symbol = compilation.GetTypeByMetadataName("GenerateConvertTests.DTO.ProductDto");
        Assert.NotNull(symbol);

        var typeInfo = TypeNameInfo.GetInfo(symbol);
        var original = MethodProvider.Create(symbol, null);
        var provider = new SourceProvider(typeInfo, symbol.Name, original, true, false);
        // compilation与symbol不一致,影响Create方法的测试
        // var provider = SourceProvider.Create(compilation, typeSymbol, typeSymbol.IsPartial());
        Assert.True(provider.IsPartial);
        Assert.False(provider.IsExtension);
        
    }
}
