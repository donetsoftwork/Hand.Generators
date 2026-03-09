using Hand;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class PredefinedTypeTests
{
    [Fact]
    public void IsBool()
    {
        var typeName = "bool";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsBool());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
        Assert.True(predefinedType.IsBool());
        Assert.True(SyntaxGenerator.BoolType.IsBool());
    }
    [Fact]
    public void IsByte()
    {
        var typeName = "byte";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsByte());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword));
        Assert.True(predefinedType.IsByte());
        Assert.True(SyntaxGenerator.ByteType.IsByte());
    }
    [Fact]
    public void IsSByte()
    {
        var typeName = "sbyte";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsSByte());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword));
        Assert.True(predefinedType.IsSByte());
        Assert.True(SyntaxGenerator.SByteType.IsSByte());
    }
    [Fact]
    public void IsChar()
    {
        var typeName = "char";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsChar());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword));
        Assert.True(predefinedType.IsChar());
        Assert.True(SyntaxGenerator.CharType.IsChar());
    }
    [Fact]
    public void IsInt()
    {
        var typeName = "int";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsInt());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
        Assert.True(predefinedType.IsInt());
        Assert.True(SyntaxGenerator.IntType.IsInt());
    }
    [Fact]
    public void IsUInt()
    {
        var typeName = "uint";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsUInt());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword));
        Assert.True(predefinedType.IsUInt());
        Assert.True(SyntaxGenerator.UIntType.IsUInt());
    }
    [Fact]
    public void IsLong()
    {
        var typeName = "long";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsLong());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword));
        Assert.True(predefinedType.IsLong());
        Assert.True(SyntaxGenerator.LongType.IsLong());
    }
    [Fact]
    public void IsULong()
    {
        var typeName = "ulong";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsULong());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword));
        Assert.True(predefinedType.IsULong());
        Assert.True(SyntaxGenerator.ULongType.IsULong());
    }
    [Fact]
    public void IsShort()
    {
        var typeName = "short";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsShort());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword));
        Assert.True(predefinedType.IsShort());
        Assert.True(SyntaxGenerator.ShortType.IsShort());
    }
    [Fact]
    public void IsFloat()
    {
        var typeName = "float";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsFloat());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword));
        Assert.True(predefinedType.IsFloat());
        Assert.True(SyntaxGenerator.FloatType.IsFloat());
    }
    [Fact]
    public void IsDouble()
    {
        var typeName = "double";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsDouble());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword));
        Assert.True(predefinedType.IsDouble());
        Assert.True(SyntaxGenerator.DoubleType.IsDouble());
    }
    [Fact]
    public void IsDecimal()
    {
        var typeName = "decimal";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsDecimal());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword));
        Assert.True(predefinedType.IsDecimal());
        Assert.True(SyntaxGenerator.DecimalType.IsDecimal());
    }
    [Fact]
    public void IsString()
    {
        var typeName = "string";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsString());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
        Assert.True(predefinedType.IsString());
        Assert.True(SyntaxGenerator.StringType.IsString());
    }
    [Fact]
    public void IsObject()
    {
        var typeName = "object";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsObject());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
        Assert.True(predefinedType.IsObject());
        Assert.True(SyntaxGenerator.ObjectType.IsObject());
    }
    [Fact]
    public void IsVoid()
    {
        var typeName = "void";
        var type = SyntaxFactory.ParseTypeName(typeName);
        Assert.True(type.IsVoid());
        var predefinedType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
        Assert.True(predefinedType.IsVoid());
        Assert.True(SyntaxGenerator.VoidType.IsVoid());
    }
}
