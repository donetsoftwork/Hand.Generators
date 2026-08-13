using Hand;
using Hand.Cachers;

namespace GenerateCoreTests;

public class AttributeSymbolCacherTests
{
    [Fact]
    public void VerifyTarget()
    {
        var sourceCode = @"
            using System;

            [AttributeUsage(AttributeTargets.Struct)]
            public class MyAttribute : Attribute;
            [My]
            public readonly record struct User(string Name);
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var sourceType = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(sourceType);
        var attribute = sourceType.GetAttributes().FirstOrDefault();
        Assert.NotNull(attribute);
        var destType = compilation.GetTypeByMetadataName("UserDTO");
        Assert.NotNull(destType);
        var attributeCacher = new AttributeSymbolCacher(compilation);
        Assert.False(attributeCacher.VerifyTarget(AttributeTargets.Class, attribute));
    }
    [Fact]
    public void Verify()
    {
        var sourceCode = @"
            using System;

            [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false))]
            public class MyAttribute : Attribute;
            [My]
            public readonly record struct User(string Name);
            [My]
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var sourceType = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(sourceType);
        var attribute = sourceType.GetAttributes().FirstOrDefault();
        Assert.NotNull(attribute);
        var destType = compilation.GetTypeByMetadataName("UserDTO");
        Assert.NotNull(destType);
        var attributeCacher = new AttributeSymbolCacher(compilation);
        Assert.False(attributeCacher.Verify(destType, attribute));
    }
    [Fact]
    public void GetAttributes()
    {
        var sourceCode = @"
            using System;

            [AttributeUsage(AttributeTargets.Struct)]
            public class MyAttribute : Attribute;
            [My]
            public readonly record struct User(string Name);
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var sourceType = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(sourceType);
        Assert.True(sourceType.GetAttributes().Length > 0);
        var destType = compilation.GetTypeByMetadataName("UserDTO");
        Assert.NotNull(destType);
        var attributeCacher = new AttributeSymbolCacher(compilation);
        var attributes = attributeCacher.GetAttributes(sourceType, AttributeTargets.Class);
        Assert.Empty(attributes);
    }
    [Fact]
    public void GetAttributes2()
    {
        var sourceCode = @"
            using System;

            [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
            public class MyAttribute : Attribute;
            [My]
            public readonly record struct User(string Name);
            [My]
            public partial class UserDTO;
            ";
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile(sourceCode);
        var sourceType = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(sourceType);
        Assert.True(sourceType.GetAttributes().Length > 0);
        var destType = compilation.GetTypeByMetadataName("UserDTO");
        Assert.NotNull(destType);
        var attributeCacher = new AttributeSymbolCacher(compilation);
        var attributes = attributeCacher.GetAttributes(sourceType, destType, AttributeTargets.Class);
        Assert.Empty(attributes);
    }
}
