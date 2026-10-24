using Hand;
using Hand.GenerateEnumeration;
using Hand.Primitives;

namespace GenerateEnumerationTests.Enum2;

public class Enumeration1Tests
{
    [Fact]
    public void Generate()
    {
        var source = @"
using Hand.Primitives;
using Hand.Enumerations;
using GenerateEnumerationTests.Enum2;

namespace GenerateEnumerationTests;

[GenerateEnumeration<CardType>]
public partial class CardEnumeration;
{
}
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .WithDocumentationComments(Microsoft.CodeAnalysis.DocumentationMode.Diagnose)
            .Reference<CardType>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = service.Generate<EnumerationGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Silver", code);
    }
    [Fact]
    public void GenerateEnumeration()
    {
        var source = @"
using Hand.Primitives;
using Hand.Enumerations;
using GenerateEnumerationTests.Enum2;

namespace GenerateEnumerationTests;

[GenerateEnumeration<CardType>]
public partial class CardEnumeration : Enumeration;
{
}
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .WithDocumentationComments(Microsoft.CodeAnalysis.DocumentationMode.Diagnose)
            .Reference<CardType>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = service.Generate<EnumerationGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Silver", code);
    }
    [Fact]
    public void GenerateFlag()
    {
        var source = @"
using Hand.Primitives;
using Hand.Enumerations;
using GenerateEnumerationTests.Enum2;

namespace GenerateEnumerationTests;

[GenerateEnumeration<CardType>]
public partial class CardEnumeration : FlagEnumeration;
{
}
";
        var service = SyntaxTreeDriver.CreateDefaultDriver()
            .WithDocumentationComments(Microsoft.CodeAnalysis.DocumentationMode.Diagnose)
            .Reference<CardType>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = service.Generate<EnumerationGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("Silver", code);
    }
}
