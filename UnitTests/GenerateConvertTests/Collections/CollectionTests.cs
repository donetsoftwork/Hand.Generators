using Hand;
using Hand.Collections;
using Hand.Converters;
using Hand.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests.Collections;

public class CollectionTests
{
    [Fact]
    public void ConvertAll()
    {
        var generator = SyntaxGenerator.Create(SyntaxFactory.ClassDeclaration("TestClass"));
        var itemConverter = new InstanceMethodConverter(SyntaxFactory.IdentifierName("ToDTO"));
        var converter = new ArrayConverter(itemConverter);
        var source = SyntaxFactory.IdentifierName("userArray");
        var dest = converter.Convert(generator, source);
        var code = dest.NormalizeWhitespace().ToFullString();
        Assert.Contains("Array.ConvertAll", code);
        Assert.Contains("ToDTO", code);
    }
    [Fact]
    public void ArrayToArray()
    {
        var source = @"
    using Hand.Mapping;

    record Category(string Name, int[] Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, string[] Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains("Array.ConvertAll", code);
    }
    [Fact]
    public void ArrayToArray2()
    {
        var source = @"
    using Hand.Mapping;

    record Category(string Name, Category[] Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, CategoryDTO[] Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains("Array.ConvertAll", code);
    }
    [Fact]
    public void ListToList()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, List<int> Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, List<string> Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains("Items.ConvertAll", code);
    }
    [Fact]
    public void ToList()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, List<int> Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, string[] Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains(".ToList", code);
    }
    [Fact]
    public void ToIList()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, IList<int> Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, string[] Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains(".ToList", code);
    }
    [Fact]
    public void ToICollection()
    {        
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, ICollection<int> Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, string[] Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains(".ToList", code);
    }
    [Fact]
    public void ToArray()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, int[] Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, List<string> Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains(".ToArray", code);
    }
    [Fact]
    public void CollectionToEnumerable()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, IEnumerable<int> Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, List<string> Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains(".ConvertAll", code);
    }
    [Fact]
    public void CollectionToFirst()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, int Item);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, List<string> Item);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains(".First", code);
    }
    [Fact]
    public void ElementToArray()
    {
        var source = @"
    using Hand.Mapping;

    record Category(string Name, int[] Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, string Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains("[", code);
    }
    [Fact]
    public void ElementToList()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, List<int> Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, string Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains("[", code);
    }
    [Fact]
    public void ElementToCollection()
    {
        var source = @"
    using System.Collections.Generic;
    using Hand.Mapping;

    record Category(string Name, IEnumerable<int> Items);
    [GenerateConvert<Category>]
    partial record CategoryDTO(string Name, string Items);
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference(typeof(GenerateConvertAttribute<>))
            .Reference(typeof(List<>));
        var result = driver.Generate<ConvertGenerator>(source)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("ToCategory", code);
        Assert.Contains("[", code);
    }
}
