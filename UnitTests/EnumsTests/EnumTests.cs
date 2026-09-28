using Hand;
using Hand.Enums.Builders;
using Hand.Types;
using Microsoft.CodeAnalysis;

namespace EnumsTests;

public class EnumTests
{
    [Fact]
    public void GetTypeMembers()
    {
        var source = @"
using System.Runtime.Serialization;

namespace Tests;
[Flags]
public enum DaysOfWeek
{
    /// <summary>
    /// 星期日
    /// </summary>
    Sunday = 1,
    /// <summary>
    /// 星期一
    /// </summary>
    Monday = 1 << 1,
    /// <summary>
    /// 星期二
    /// </summary>
    Tuesday = 1 << 2,
    /// <summary>
    /// 星期三
    /// </summary>
    Wednesday = 1 << 3,
    /// <summary>
    /// 星期四
    /// </summary>
    Thursday = 1 << 4,
    /// <summary>
    /// 星期五
    /// </summary>
    Friday = 1 << 5,
    /// <summary>
    /// 星期六
    /// </summary>
    Saturday = 1 << 6,
}
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<FlagsAttribute>();
        var compilation = driver.Compile(source);
        var classSymbol = compilation.GetTypeByMetadataName("Tests.DaysOfWeek");
        Assert.NotNull(classSymbol);
        var friday = classSymbol.GetMembers("Friday").FirstOrDefault() as IFieldSymbol;
        Assert.NotNull(friday);
        var builder = new EnumBundleBuilder(compilation);
        var typeInfo = new EnumTypeInfo(classSymbol, classSymbol, true, false);
        var bundle = builder.Get(typeInfo);
        Assert.True(bundle.HasFlag);
        var fields = bundle.Fields.ToArray();
        Assert.Equal(7, fields.Length);
    }
}
