using Hand;
using System.Drawing;

namespace SyntaxScriptingTests;

public class EnumTests
{
    [Fact]
    public void MyColorTest()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var code = @"enum MyColor : int
    {
        Red,
        Green,
        Blue
    };
var color = MyColor.Red;";
        var compilation = service.Compile(code);
        var colorType = compilation.GetTypeByMetadataName("MyColor");
        Assert.NotNull(colorType);
        var underlyingType = colorType.EnumUnderlyingType;
        Assert.NotNull(underlyingType);
        //var myColor = MyColor.Red;
    }
    [Fact]
    public void ColorTest()
    {
        // System.Drawing.Color is a struct, not an enum, so it should not have an underlying type.
        var service = SyntaxTreeDriver.CreateDriver()
            .Reference<Color>();
        var code = "var color = Color.Red;";
        var compilation = service.Compile(code);
        var colorType = compilation.GetTypeByMetadataName("System.Drawing.Color");
        Assert.NotNull(colorType);
        var underlyingType = colorType.EnumUnderlyingType;
        Assert.Null(underlyingType);
        //SyntaxKind.ExplicitKeyword
        //SyntaxFactory.Explicit
    }

    //enum MyColor : int
    //{
    //    Red,
    //    Green,
    //    Blue
    //}
}
