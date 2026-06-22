using GenerateConvertTests.Supports;
using Hand;
using Hand.Builders;
using Hand.Converters;
using Hand.Enums;
using Microsoft.CodeAnalysis;
using System.Reflection.Metadata;

namespace GenerateConvertTests;

public class ConvertBuilderTests
{
    private readonly Compilation _compilation;
    private readonly ConvertBuilder _builder;
    public ConvertBuilderTests()
    {
        var source = "var i = 0;";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Using("System");
        _compilation = driver.Compile(source);
        _builder = new ConvertBuilder(_compilation);
    }
    /// <summary>
    /// 添加引用
    /// </summary>
    /// <typeparam name="type"></typeparam>
    /// <returns></returns>
    public static Compilation WithReference(Compilation compilation, Type type)
    {
        var references = type.Assembly.ToReferences()
            .ToArray();
        if (references.Length > 0)
            return compilation.AddReferences(references);
        return compilation;
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
        var nullable = _compilation.GetSpecialType(SpecialType.System_Nullable_T)
            .Construct(type);
        var converter2 = _builder.Get(type, type);
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
        var compilation = WithReference(_compilation, type);
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
        var compilation = WithReference(_compilation, type);
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
        var compilation = WithReference(_compilation, type);
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
        var type = typeof(MyColor);
        var compilation = WithReference(_compilation, type);
        var sourceType = compilation.GetStringSymbol();
        var destType = compilation.GetTypeByMetadataName(type.FullName!);
        Assert.NotNull(destType);
        var converter = _builder.Get(sourceType, destType);
        Assert.NotNull(converter);
        if (converter is not StaticMethodConverter)
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
    public void FlagToEnum()
    {
        var sourceType = typeof(MyColor);
        var destType = typeof(ConsoleColor);
        var compilation = WithReference(WithReference(_compilation, sourceType), destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not StaticMethodConverter)
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
        var compilation = WithReference(WithReference(_compilation, sourceType), destType);
        var sourceSymbol = compilation.GetTypeByMetadataName(sourceType.FullName!);
        Assert.NotNull(sourceSymbol);
        var destSymbol = compilation.GetTypeByMetadataName(destType.FullName!);
        Assert.NotNull(destSymbol);
        var converter = _builder.Get(sourceSymbol, destSymbol);
        Assert.NotNull(converter);
        if (converter is not StaticMethodConverter)
            Assert.Fail();
        var last = _builder.Sources.LastOrDefault();
        Assert.NotNull(last);
        var code = last.Generate()
            .Build()
            .NormalizeWhitespace()
            .ToFullString();
        Assert.Contains("ToMyColor", code);
    }
}
