using Hand;
using Hand.Enumerations;
using Hand.Enumerations.Sources;
using Hand.GenerateEnumeration;
using Hand.Naming;
using Hand.Primitives;
using Microsoft.CodeAnalysis;

namespace GenerateEnumerationTests.FlagEnumeration3;

public class AccessEnumerationTests
{
    [Fact]
    public void Source()
    {
        var sourceCode = @"
using Hand.Enumerations;

namespace GenerateEnumerationTests;

public class AccessEnumeration(ISet<string> flags, long original, string description = "")
    : FlagEnumeration(flags, original, description)
{
    public AccessEnumeration(long original, string name)
        : this(new HashSet<string>() { name }, original, name)
    {
    }
    /// <summary>
    /// 空
    /// </summary>
    public static readonly AccessEnumeration None = new(0, nameof(None));
    /// <summary>
    /// 读
    /// </summary>
    public static readonly AccessEnumeration Read = new(1, nameof(Read));
    /// <summary>
    /// 写
    /// </summary>
    public static readonly AccessEnumeration Write = new(2, nameof(Write));
}
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>();
        var compilation = driver.Compile(sourceCode);
        var enumerationSymbol = compilation.GetTypeByMetadataName("GenerateEnumerationTests.AccessEnumeration");
        Assert.NotNull(enumerationSymbol);
        var naming = TypedProvider.Inherit(enumerationSymbol);
        var providerSource = EnumerationProviderSource.CreateFlag(enumerationSymbol, naming);
        Assert.NotNull(providerSource);
        var generatedCode = providerSource.Generate()
            .BuildUnit()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("AccessEnumerationProvider", generatedCode);
    }
    //public class AccessEnumeration(ISet<string> flags, long original, string description = "")
    //    : FlagEnumeration(flags, original, description)
    //{
    //    public AccessEnumeration(long original, string name)
    //        : this(new HashSet<string>() { name }, original, name)
    //    {
    //    }
    //    /// <summary>
    //    /// 空
    //    /// </summary>
    //    public static readonly AccessEnumeration None = new(0, nameof(None));
    //    /// <summary>
    //    /// 读
    //    /// </summary>
    //    public static readonly AccessEnumeration Read = new(1, nameof(Read));
    //    /// <summary>
    //    /// 写
    //    /// </summary>
    //    public static readonly AccessEnumeration Write = new(2, nameof(Write));
    //}
    //public class AccessEnumerationProvider : FlagEnumerationProvider<AccessEnumeration>
    //{
    //    internal AccessEnumerationProvider() : base(new Dictionary<string, AccessEnumeration> { { "None", AccessEnumeration.None }, { "Read", AccessEnumeration.Read }, { "Write", AccessEnumeration.Write } })
    //    {
    //    }

    //    public override AccessEnumeration Empty => AccessEnumeration.None;

    //    protected override AccessEnumeration CreateFlag(ISet<string> flags, long original, string description) => new(flags, original, description);
    //    public static readonly AccessEnumerationProvider Instance = new();
    //}
    [Fact]
    public void Generator()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;

namespace GenerateEnumerationTests;

public class AccessEnumeration(ISet<string> flags, long original, string description = "")
    : FlagEnumeration(flags, original, description)
{
    public AccessEnumeration(long original, string name)
        : this(new HashSet<string>() { name }, original, name)
    {
    }
    /// <summary>
    /// 空
    /// </summary>
    public static readonly AccessEnumeration None = new(0, nameof(None));
    /// <summary>
    /// 读
    /// </summary>
    public static readonly AccessEnumeration Read = new(1, nameof(Read));
    /// <summary>
    /// 写
    /// </summary>
    public static readonly AccessEnumeration Write = new(2, nameof(Write));
}
[GenerateEnumeration<AccessEnumeration>]
public partial class AccessProvider;
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

public class AccessEnumeration(ISet<string> flags, long original, string description = "")
    : FlagEnumeration(flags, original, description)
{
    public AccessEnumeration(long original, string name)
        : this(new HashSet<string>() { name }, original, name)
    {
    }
    /// <summary>
    /// 空
    /// </summary>
    public static readonly AccessEnumeration None = new(0, nameof(None));
    /// <summary>
    /// 读
    /// </summary>
    public static readonly AccessEnumeration Read = new(1, nameof(Read));
    /// <summary>
    /// 写
    /// </summary>
    public static readonly AccessEnumeration Write = new(2, nameof(Write));
}
[GenerateEnumeration<AccessEnumeration>]
public partial class AccessProvider : FlagEnumerationProvider<AccessEnumeration>;
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

public class AccessEnumeration(ISet<string> flags, long original, string description = "")
    : FlagEnumeration(flags, original, description)
{
    public AccessEnumeration(long original, string name)
        : this(new HashSet<string>() { name }, original, name)
    {
    }
    /// <summary>
    /// 空
    /// </summary>
    public static readonly AccessEnumeration None = new(0, nameof(None));
    /// <summary>
    /// 读
    /// </summary>
    public static readonly AccessEnumeration Read = new(1, nameof(Read));
    /// <summary>
    /// 写
    /// </summary>
    public static readonly AccessEnumeration Write = new(2, nameof(Write));
}
[GenerateEnumeration<AccessEnumeration>]
public partial class AccessProvider
{
    public static readonly AccessProvider Instance = new();
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
    public void NoFlag()
    {
        var sourceCode = @"
using Hand.Enumerations;
using Hand.Primitives;

namespace GenerateEnumerationTests;

public class AccessEnumeration(ISet<string> flags, long original, string description = "")
    : FlagEnumeration(flags, original, description)
{
    public AccessEnumeration(long original, string name)
        : this(new HashSet<string>() { name }, original, name)
    {
    }
    /// <summary>
    /// 空
    /// </summary>
    public static readonly AccessEnumeration None = new(0, nameof(None));
    /// <summary>
    /// 读
    /// </summary>
    public static readonly AccessEnumeration Read = new(1, nameof(Read));
    /// <summary>
    /// 写
    /// </summary>
    public static readonly AccessEnumeration Write = new(2, nameof(Write));
}
[GenerateEnumeration<AccessEnumeration>]
public partial class AccessProvider : EnumerationProvider<AccessEnumeration>;
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
}

public class AccessEnumeration(ISet<string> flags, long original, string description = "")
    : FlagEnumeration(flags, original, description)
{
    public AccessEnumeration(long original, string name)
        : this(new HashSet<string>() { name }, original, name)
    {
    }
    /// <summary>
    /// 空
    /// </summary>
    public static readonly AccessEnumeration None = new(0, nameof(None));
    /// <summary>
    /// 读
    /// </summary>
    public static readonly AccessEnumeration Read = new(1, nameof(Read));
    /// <summary>
    /// 写
    /// </summary>
    public static readonly AccessEnumeration Write = new(2, nameof(Write));
}
[GenerateEnumeration<AccessEnumeration>]
public partial class AccessProvider;
