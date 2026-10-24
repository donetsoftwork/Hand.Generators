using Hand;
using Hand.Enumerations;
using Hand.Enumerations.Sources;
using Hand.GenerateEnumeration;
using Hand.Naming;
using Hand.Primitives;
using Microsoft.CodeAnalysis;

namespace GenerateEnumerationTests.Enumeration3;

/// <summary>
/// 枚举类生成枚举提供器
/// </summary>
public class StatusTests
{
    [Fact]
    public void Source()
    {
        var sourceCode = @"
using Hand.Enumerations;

namespace GenerateEnumerationTests;

public class Status : Enumeration
{
    private Status(string name, int value) : base(name, value, name)
    {
    }
    public static readonly Status Open = new(""Open"", 1);
    public static readonly Status Closed = new(""Closed"", 2);
}
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>();
        var compilation = driver.Compile(sourceCode);
        var enumerationSymbol = compilation.GetTypeByMetadataName("GenerateEnumerationTests.Status");
        Assert.NotNull(enumerationSymbol);
        var naming = TypedProvider.Inherit(enumerationSymbol);
        var providerSource = EnumerationProviderSource.Create(enumerationSymbol, naming);
        var generatedCode = providerSource.Generate()
            .BuildUnit()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("StatusProvider", generatedCode);
    }
    //public class Status : Enumeration
    //{
    //    private Status(string name, int value) : base(name, value, name)
    //    {
    //    }
    //    public static readonly Status Open = new("Open", 1);
    //    public static readonly Status Closed = new("Closed", 2);
    //}
    //public class StatusProvider : EnumerationProvider<Status>
    //{
    //    internal StatusProvider() : base(new Dictionary<string, Status> { { "Open", Status.Open }, { "Closed", Status.Closed } })
    //    {
    //    }

    //    public static readonly StatusProvider Instance = new();
    //}

    [Fact]
    public void Generator()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;

namespace GenerateEnumerationTests;

public class Status : Enumeration
{
    private Status(string name, int value) : base(name, value, name)
    {
    }
    public static readonly Status Open = new(""Open"", 1);
    public static readonly Status Closed = new(""Closed"", 2);
}
[GenerateEnumeration<Status>]
public partial class StatusProvider;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Instance", code);
    }
    [Fact]
    public void GeneratorWithBase()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;

namespace GenerateEnumerationTests;

public class Status : Enumeration
{
    private Status(string name, int value) : base(name, value, name)
    {
    }
    public static readonly Status Open = new(""Open"", 1);
    public static readonly Status Closed = new(""Closed"", 2);
}
[GenerateEnumeration<Status>]
public partial class StatusProvider : EnumerationProvider<Status>;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Instance", code);
    }
    [Fact]
    public void GeneratorWithInstance()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;

namespace GenerateEnumerationTests;

public class Status : Enumeration
{
    private Status(string name, int value) : base(name, value, name)
    {
    }
    public static readonly Status Open = new(""Open"", 1);
    public static readonly Status Closed = new(""Closed"", 2);
}
[GenerateEnumeration<Status>]
public partial class StatusProvider
{
    public static readonly StatusProvider Instance = new();
}
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.DoesNotContain("Instance", code);
    }
    [Fact]
    public void FlagGenerator()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;

namespace GenerateEnumerationTests;

public class Status : Enumeration
{
    private Status(string name, int value) : base(name, value, name)
    {
    }
    public static readonly Status Open = new(""Open"", 1);
    public static readonly Status Closed = new(""Closed"", 2);
}
[GenerateEnumeration<Status>]
public partial class StatusProvider : FlagEnumerationProvider<Status>;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        // 非位标记枚举类不能生成位标记枚举提供器
        Assert.Null(syntaxTree);
    }
}

public class Status : Enumeration
{
    private Status(string name, int value) : base(name, value, name)
    {
    }
    public static readonly Status Open = new("Open", 1);
    public static readonly Status Closed = new("Closed", 2);
}
[GenerateEnumeration<Status>]
public partial class StatusProvider;