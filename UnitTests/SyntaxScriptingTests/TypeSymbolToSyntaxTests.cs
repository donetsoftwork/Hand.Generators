using Hand;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SyntaxScriptingTests.Supports;

namespace SyntaxScriptingTests;

public class TypeSymbolToSyntaxTests
{
    [Fact]
    public void PredefinedType()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        if(intType.ToSyntax() is not PredefinedTypeSyntax syntax)
        {
            Assert.Fail();
            return;
        }
        //ITypeParameterSymbol
        var code = syntax.NormalizeWhitespace().ToFullString();
        Assert.Equal("int", code);
    }
    [Fact]
    public void Enum()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var source = @"namespace Tests;
    public enum MyColor : int
    {
        Red,
        Green,
        Blue
    };
    var color = MyColor.Red;";
        var compilation = service.Compile(source);
        var enumType = compilation.GetTypeByMetadataName("Tests.MyColor");
        Assert.NotNull(enumType);
        var underlyingType = enumType.EnumUnderlyingType;
        Assert.NotNull(underlyingType);
        var type = enumType.ToSyntax();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.Equal("Tests.MyColor", code);
    }
    [Fact]
    public void Array()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var arrayType = compilation.CreateArrayTypeSymbol(intType, 1);
        if (arrayType.ToSyntax() is not ArrayTypeSyntax syntax)
        {
            Assert.Fail();
            return;
        }
        var code = syntax.NormalizeWhitespace().ToFullString();
        Assert.Equal("int[]", code);
    }
    [Fact]
    public void Generic()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var genericType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T)
            .Construct(intType);
        var syntax = genericType.ToSyntax();
        if (syntax is not GenericNameSyntax && (syntax is not QualifiedNameSyntax qualifiedSyntax || qualifiedSyntax.Right is not GenericNameSyntax))
        {
            Assert.Fail();
            return;
        }
        var code = syntax.NormalizeWhitespace().ToFullString();
        Assert.Equal("System.Collections.Generic.IList<int>", code);
    }
    [Fact]
    public void PredefinedTypeNullable()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var nullable = compilation.GetSpecialType(SpecialType.System_Nullable_T)
            .Construct(intType);
        if (nullable.ToSyntax() is not NullableTypeSyntax syntax)
        {
            Assert.Fail();
            return;
        }
        var code = syntax.NormalizeWhitespace().ToFullString();
        Assert.Equal("int?", code);
    }
    [Fact]
    public void PredefinedTypeNullable2()
    {
        var service = SyntaxTreeDriver.CreateScriptDriver();
        var compilation = service.ScriptCompile("int? obj = 1; return obj+1;", returnType: typeof(int?));
        var predefinedType = compilation.GetSpecialType(SpecialType.System_Int32);
        var nullableType = compilation.GetSpecialType(SpecialType.System_Nullable_T);
        var predefinedTypeWithNull = nullableType.Construct(predefinedType)
            .WithNullableAnnotation(NullableAnnotation.Annotated) as INamedTypeSymbol;
        Assert.NotNull(predefinedTypeWithNull);
        var type = predefinedTypeWithNull.ToSyntax();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.Equal("int?", code);
        var entryPoint = compilation.GetEntryPoint(default);
        Assert.NotNull(entryPoint);
        if(entryPoint.ReturnType is not INamedTypeSymbol taskType)
        {
            Assert.Fail();
            return;
        }
        var returnType = taskType.TypeArguments[0] as INamedTypeSymbol;
        Assert.NotNull(returnType);
        Assert.Equal(NullableAnnotation.Annotated, returnType.NullableAnnotation);
        Assert.True(returnType.IsGenericType(nullableType));
    }
    [Fact]
    public void CustomTypeNullable()
    {
        var service = SyntaxTreeDriver.CreateScriptDriver()
            .Reference<UserId>();
        var source = @"UserId? userId = new UserId(1); return userId;";
        var compilation = service.ScriptCompile(source, returnType: typeof(UserId?));
        var customType = compilation.GetTypeByMetadataName("SyntaxScriptingTests.Supports.UserId");
        Assert.NotNull(customType);
        var nullableType = compilation.GetSpecialType(SpecialType.System_Nullable_T);
        var customTypeWithNull = nullableType.Construct(customType)
            .WithNullableAnnotation(NullableAnnotation.Annotated) as INamedTypeSymbol;
        Assert.NotNull(customTypeWithNull);
        var type = customTypeWithNull.ToSyntax();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.Equal("SyntaxScriptingTests.Supports.UserId?", code);
        var entryPoint = compilation.GetEntryPoint(default);
        Assert.NotNull(entryPoint);
        if (entryPoint.ReturnType is not INamedTypeSymbol taskType)
        {
            Assert.Fail();
            return;
        }
        var returnType = taskType.TypeArguments[0] as INamedTypeSymbol;
        Assert.NotNull(returnType);
        Assert.Equal(NullableAnnotation.Annotated, returnType.NullableAnnotation);
        Assert.True(returnType.IsGenericType(nullableType));
    }
    [Fact]
    public void PredefinedTypeAnnotated()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var predefinedType = compilation.GetSpecialType(SpecialType.System_String);
        var annotatedType = predefinedType.WithNullableAnnotation(NullableAnnotation.Annotated) as INamedTypeSymbol;
        Assert.NotNull(annotatedType);
        var type = annotatedType.ToSyntax();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.Equal("string?", code);
    }
    [Fact]
    public void CustomTypeAnnotated()
    {
        var service = SyntaxTreeDriver.CreateDriver()
             .Reference<UserName>();
        var compilation = service.Compile("");
        var customType = compilation.GetTypeByMetadataName("SyntaxScriptingTests.Supports.UserName");
        Assert.NotNull(customType);
        var annotatedType = customType.WithNullableAnnotation(NullableAnnotation.Annotated) as INamedTypeSymbol;
        Assert.NotNull(annotatedType);
        var type = annotatedType.ToSyntax();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.Equal("SyntaxScriptingTests.Supports.UserName?", code);
        var property = SymbolReflection.GetProperties(customType)
            .FirstOrDefault(p => p.Name == "Value");
        Assert.NotNull(property);
        // string?
        var propertyType = property.Type as INamedTypeSymbol;
        Assert.NotNull(propertyType);
        Assert.Equal(NullableAnnotation.Annotated, propertyType.NullableAnnotation);
        var nullableType = compilation.GetSpecialType(SpecialType.System_Nullable_T);
        Assert.False(propertyType.IsGenericType(nullableType));
    }
    [Fact]
    public void ToGlobalName()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var dateTimeType = compilation.GetSpecialType(SpecialType.System_DateTime);
        var syntax = dateTimeType.ToGlobalName();
        var code = syntax.NormalizeWhitespace().ToFullString();
        Assert.Equal("global::System.DateTime", code);
    }
    [Fact]
    public void ToMiniName()
    {
        var service = SyntaxTreeDriver.CreateDriver();
        var compilation = service.Compile("");
        var dateTimeType = compilation.GetSpecialType(SpecialType.System_DateTime);
        var syntax = dateTimeType.ToMiniName();
        var code = syntax.NormalizeWhitespace().ToFullString();
        Assert.Equal("DateTime", code);
    }
}
