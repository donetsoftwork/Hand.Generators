using GenerateConvertTests.DTO;
using GenerateConvertTests.Supports;
using Hand;
using Hand.Builders;
using Hand.Converters;
using Hand.Converters.Constructors;
using Hand.Converters.Methods;
using Hand.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateConvertTests;

public class ConvertBuilderTests
{
    private readonly Compilation _compilation;
    private readonly ConvertBuilder _builder;
    public ConvertBuilderTests()
    {
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Using("System");
        _compilation = driver.Compile("");
        _builder = new ConvertBuilder(_compilation);
    }
    [Theory]
    [InlineData(SpecialType.System_Boolean)]
    [InlineData(SpecialType.System_Char)]
    [InlineData(SpecialType.System_SByte)]
    [InlineData(SpecialType.System_Int16)]
    [InlineData(SpecialType.System_Int32)]
    [InlineData(SpecialType.System_Int64)]
    [InlineData(SpecialType.System_Byte)]
    [InlineData(SpecialType.System_UInt16)]
    [InlineData(SpecialType.System_UInt32)]
    [InlineData(SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_Single)]
    [InlineData(SpecialType.System_Double)]
    [InlineData(SpecialType.System_Decimal)]
    [InlineData(SpecialType.System_DateTime)]
    [InlineData(SpecialType.System_String)]
    [InlineData(SpecialType.System_Object)]
    public void Pass(SpecialType specialType)
    {
        var type = _compilation.GetSpecialType(specialType);
        var converter = _builder.Get(type, type);
        Assert.NotNull(converter);
        if (converter is not PassConverter)
            Assert.Fail();
        var nullable = _compilation.GetNullable(type);
        var converter2 = _builder.Get(nullable, nullable);
        Assert.NotNull(converter2);
        if (converter2 is not PassConverter)
            Assert.Fail();
    }
    [Theory]
    [InlineData(SpecialType.System_Int32, SpecialType.System_Int64)]
    [InlineData(SpecialType.System_Int16, SpecialType.System_Int64)]
    [InlineData(SpecialType.System_SByte, SpecialType.System_Int64)]
    [InlineData(SpecialType.System_UInt32, SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_UInt16, SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_Byte, SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_Int16, SpecialType.System_Int32)]
    [InlineData(SpecialType.System_SByte, SpecialType.System_Int32)]
    [InlineData(SpecialType.System_UInt16, SpecialType.System_UInt32)]
    [InlineData(SpecialType.System_Byte, SpecialType.System_UInt32)]
    [InlineData(SpecialType.System_SByte, SpecialType.System_Int16)]
    [InlineData(SpecialType.System_Byte, SpecialType.System_UInt16)]
    [InlineData(SpecialType.System_Single, SpecialType.System_Double)]
    [InlineData(SpecialType.System_Byte, SpecialType.System_Int64)]
    [InlineData(SpecialType.System_UInt16, SpecialType.System_Int64)]
    [InlineData(SpecialType.System_UInt32, SpecialType.System_Int64)]
    [InlineData(SpecialType.System_Int32, SpecialType.System_Single)]
    [InlineData(SpecialType.System_Int32, SpecialType.System_Decimal)]
    [InlineData(SpecialType.System_Int32, SpecialType.System_Double)]
    public void Implicit(SpecialType source, SpecialType dest)
    {
        // 短类型变长类型,隐式转化
        var sourceType = _compilation.GetSpecialType(source);
        var destType = _compilation.GetSpecialType(dest);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not PassConverter)
            Assert.Fail();
    }
    [Theory]
    [InlineData(SpecialType.System_Int64, SpecialType.System_Int32)]
    [InlineData(SpecialType.System_Int64, SpecialType.System_Int16)]
    [InlineData(SpecialType.System_Int64, SpecialType.System_SByte)]
    [InlineData(SpecialType.System_UInt64, SpecialType.System_UInt32)]
    [InlineData(SpecialType.System_UInt64, SpecialType.System_UInt16)]
    [InlineData(SpecialType.System_UInt64, SpecialType.System_Byte)]
    [InlineData(SpecialType.System_Int32, SpecialType.System_Int16)]
    [InlineData(SpecialType.System_Int32, SpecialType.System_SByte)]
    [InlineData(SpecialType.System_UInt32, SpecialType.System_UInt16)]
    [InlineData(SpecialType.System_UInt32, SpecialType.System_Byte)]
    [InlineData(SpecialType.System_Int16, SpecialType.System_SByte)]
    [InlineData(SpecialType.System_UInt16, SpecialType.System_Byte)]
    [InlineData(SpecialType.System_Double, SpecialType.System_Single)]
    [InlineData(SpecialType.System_Single, SpecialType.System_Int32)]
    [InlineData(SpecialType.System_Decimal, SpecialType.System_Int32)]
    [InlineData(SpecialType.System_Double, SpecialType.System_Int32)]
    [InlineData(SpecialType.System_SByte, SpecialType.System_Byte)]
    [InlineData(SpecialType.System_Int16, SpecialType.System_UInt16)]
    [InlineData(SpecialType.System_Int32, SpecialType.System_UInt32)]
    [InlineData(SpecialType.System_Int64, SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_Byte, SpecialType.System_SByte)]
    [InlineData(SpecialType.System_UInt16, SpecialType.System_Int16)]
    [InlineData(SpecialType.System_UInt32, SpecialType.System_Int32)]
    [InlineData(SpecialType.System_UInt64, SpecialType.System_Int64)]
    [InlineData(SpecialType.System_Int32, SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_Int16, SpecialType.System_UInt64)]
    [InlineData(SpecialType.System_SByte, SpecialType.System_UInt64)]
    public void Explicit(SpecialType source, SpecialType dest)
    {
        // 显式转化
        // 长类型变短类型
        // 有符号变无符号
        // 无符号变有符号
        var sourceType = _compilation.GetSpecialType(source);
        var destType = _compilation.GetSpecialType(dest);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not CastConverter)
            Assert.Fail();
    }
    [Theory]
    [InlineData(typeof(MyColor), SpecialType.System_SByte)]
    [InlineData(typeof(MyColor), SpecialType.System_Int16)]
    [InlineData(typeof(MyColor), SpecialType.System_Int32)]
    [InlineData(typeof(MyColor), SpecialType.System_Int64)]
    [InlineData(typeof(MyColor), SpecialType.System_Byte)]
    [InlineData(typeof(MyColor), SpecialType.System_UInt16)]
    [InlineData(typeof(MyColor), SpecialType.System_UInt32)]
    [InlineData(typeof(MyColor), SpecialType.System_UInt64)]
    [InlineData(typeof(MyColor), SpecialType.System_Single)]
    [InlineData(typeof(MyColor), SpecialType.System_Decimal)]
    [InlineData(typeof(MyColor), SpecialType.System_Double)]
    public void EnumToNumeric(Type type, SpecialType numeric)
    {
        // 枚举显式转化数值类型
        var typeName = type.FullName;
        var compilation = _compilation.WithReference(type);
        var sourceType = compilation.GetTypeByMetadataName(typeName!);
        Assert.NotNull(sourceType);
        var destType = compilation.GetSpecialType(numeric);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not CastConverter)
            Assert.Fail();
    }
    [Theory]
    [InlineData(SpecialType.System_SByte, typeof(MyColor))]
    [InlineData(SpecialType.System_Int16, typeof(MyColor))]
    [InlineData(SpecialType.System_Int32, typeof(MyColor))]
    [InlineData(SpecialType.System_Int64, typeof(MyColor))]
    [InlineData(SpecialType.System_Byte, typeof(MyColor))]
    [InlineData(SpecialType.System_UInt16, typeof(MyColor))]
    [InlineData(SpecialType.System_UInt32, typeof(MyColor))]
    [InlineData(SpecialType.System_UInt64, typeof(MyColor))]
    [InlineData(SpecialType.System_Single, typeof(MyColor))]
    [InlineData(SpecialType.System_Decimal, typeof(MyColor))]
    [InlineData(SpecialType.System_Double, typeof(MyColor))]
    public void NumericToEnum(SpecialType numeric, Type type)
    {
        // 枚举显式转化数值类型
        var typeName = type.FullName;
        var compilation = _compilation.WithReference(type);
        var sourceType = compilation.GetSpecialType(numeric);
        var destType = compilation.GetTypeByMetadataName(typeName!);
        Assert.NotNull(destType);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not CastConverter)
            Assert.Fail();
    }
    [Fact]
    public void EnumFromString()
    {
        var type = typeof(ConsoleColor);
        var compilation = _compilation.WithReference(type);
        var sourceType = compilation.GetStringSymbol();
        var destType = compilation.GetTypeByMetadataName(type.FullName!);
        Assert.NotNull(destType);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not EnumParseConverter)
            Assert.Fail();
    }
    [Fact]
    public void FlagEnumFromString()
    {
        var type = typeof(ColumnType);
        var compilation = _compilation.WithReference(type);
        var sourceType = compilation.GetStringSymbol();
        var destType = compilation.GetTypeByMetadataName(type.FullName!);
        Assert.NotNull(destType);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not EnumParseConverter)
            Assert.Fail();
    }
    [Fact]
    public void EnumFromMemberString()
    {
        var type = typeof(MyColorDTO);
        var compilation = _compilation.WithReference(type);
        var sourceType = compilation.GetStringSymbol();
        var destType = compilation.GetTypeByMetadataName(type.FullName!);
        Assert.NotNull(destType);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToMyColorDTO", code);
    }
    [Fact]
    public void FlagEnumFromMemberString()
    {
        var type = typeof(MyColor);
        var compilation = _compilation.WithReference(type);
        var sourceType = compilation.GetStringSymbol();
        var destType = compilation.GetTypeByMetadataName(type.FullName!);
        Assert.NotNull(destType);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToMyColor", code);
    }
    [Fact]
    public void FlagToFlag()
    {
        var sourceType = typeof(ColumnType);
        var destType = typeof(ColumnTypeDTO);
        var compilation = _compilation.WithReference(typeof(FlagsAttribute))
            .WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToDTO", code);
    }
    [Fact]
    public void FlagToEnum()
    {
        var sourceType = typeof(MyColor);
        var destType = typeof(ConsoleColor);
        //var compilation = WithReference(WithReference(WithReference(_compilation, typeof(FlagsAttribute)), sourceType), destType);
        var compilation = _compilation.WithReference(typeof(FlagsAttribute))
            .WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToConsoleColor", code);
    }
    [Fact]
    public void EnumToEnum()
    {
        var sourceType = typeof(ConsoleColor);
        var destType = typeof(MyColor);
        var compilation = _compilation.WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToMyColor", code);
    }
    [Fact]
    public void PrimitiveToEntity()
    {
        var sourceType = typeof(long);
        var destType = typeof(UserId);
        var compilation = _compilation.WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetLongSymbol();
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not ConstructorConverter)
            Assert.Fail();
    }
    [Fact]
    public void PrimitiveToEntity2()
    {
        var sourceType = typeof(string);
        var destType = typeof(UserId);
        var compilation = _compilation.WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetStringSymbol();
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not CompositeConverter)
            Assert.Fail();
    }
    [Fact]
    public void EntityToEntity()
    {
        var sourceType = typeof(CustomerId);
        var destType = typeof(UserId);
        var compilation = _compilation.WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not CompositeConverter)
            Assert.Fail();
    }
    [Fact]
    public void ToDTO()
    {
        var sourceType = typeof(User);
        var destType = typeof(UserDTO);
        var compilation = _compilation.WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        //var source = SyntaxFactory.IdentifierName("user");
        //var dest = converter.Convert(source);
        //var code = dest.NormalizeWhitespace()
        //    .ToFullString();
        //Assert.Contains("UserDTO", code);
        var last = _builder.Sources.FirstOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToDTO", code);
    }
    [Fact]
    public void ToUser()
    {
        var sourceType = typeof(UserDTO);
        var destType = typeof(User);
        var compilation = _compilation.WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToUser", code);
    }
    [Fact]
    public void ToEntity()
    {
        var sourceType = typeof(User);
        var destType = typeof(UserEntity);
        var compilation = _compilation.WithReference(sourceType)
            .WithReference(destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not MethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToEntity", code);
    }
}
