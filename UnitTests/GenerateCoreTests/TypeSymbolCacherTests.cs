using Hand;
using Hand.Cachers;
using Hand.Models;
using Hand.Reflection;
using Hand.Types;

namespace GenerateCoreTests;

public class TypeSymbolCacherTests
{
    [Fact]
    public void Primitive_Int()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("");
        var infos = new TypeSymbolCacher(compilation);
        var intSymbol = compilation.GetIntSymbol();
        var intInfo = infos.Get(intSymbol);
        Assert.Equal(TypeSymbolKind.Primitive, intInfo.Kind);
        Assert.False(intInfo.IsNullable);
        var intNullable = compilation.GetNullable(intSymbol);
        var intNullableInfo = infos.Get(intNullable);
        Assert.Equal(TypeSymbolKind.Primitive, intNullableInfo.Kind);
        Assert.True(intNullableInfo.IsNullable);
    }
    [Fact]
    public void Primitive_String()
    {
        var compilation = SyntaxTreeDriver.DefaultDriver.Compile("");
        var infos = new TypeSymbolCacher(compilation);
        var stringSymbol = compilation.GetStringSymbol();
        var stringInfo = infos.Get(stringSymbol);
        Assert.Equal(TypeSymbolKind.Primitive, stringInfo.Kind);
        Assert.False(stringInfo.IsNullable);
        var stringNullable = compilation.GetNullable(stringSymbol);
        var stringNullableInfo = infos.Get(stringNullable);
        Assert.Equal(TypeSymbolKind.Primitive, stringNullableInfo.Kind);
        Assert.True(stringNullableInfo.IsNullable);
    }
    [Fact]
    public void Enum()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<AttributeTargets>();
        var compilation = driver.Compile("");
        var enumSymbol = compilation.GetTypeByMetadataName(typeof(AttributeTargets).FullName!);
        Assert.NotNull(enumSymbol);
        var infos = new TypeSymbolCacher(compilation);
        var enumInfo = infos.Get(enumSymbol);
        Assert.Equal(TypeSymbolKind.Enum, enumInfo.Kind);
        Assert.False(enumInfo.IsNullable);
        var enumNullable = compilation.GetNullable(enumSymbol);
        var enumNullableInfo = infos.Get(enumNullable);
        Assert.Equal(TypeSymbolKind.Enum, enumNullableInfo.Kind);
        Assert.True(enumNullableInfo.IsNullable);
    }
    [Fact]
    public void Array()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<AttributeTargets>();
        var compilation = driver.Compile("");
        var stringSymbol = compilation.GetStringSymbol();
        var arraySymbol = compilation.CreateArrayTypeSymbol(stringSymbol, 1);
        Assert.NotNull(arraySymbol);
        var infos = new TypeSymbolCacher(compilation);
        var arrayInfo = infos.Get(arraySymbol);
        Assert.Equal(TypeSymbolKind.Array, arrayInfo.Kind);
        Assert.False(arrayInfo.IsNullable);
        var arrayNullable = compilation.GetNullable(arraySymbol);
        var arrayNullableInfo = infos.Get(arrayNullable);
        Assert.Equal(TypeSymbolKind.Array, arrayNullableInfo.Kind);
        Assert.True(arrayNullableInfo.IsNullable);
    }
    [Fact]
    public void Collection_List()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<AttributeTargets>();
        var compilation = driver.Compile("");
        var stringSymbol = compilation.GetStringSymbol();
        var collectionSymbol = compilation.GetList(stringSymbol);
        Assert.NotNull(collectionSymbol);
        var infos = new TypeSymbolCacher(compilation);
        var collectionInfo = infos.Get(collectionSymbol) as CollectionTypeInfo;
        Assert.NotNull(collectionInfo);
        Assert.False(collectionInfo.IsNullable);
        var collectionNullable = compilation.GetNullable(collectionSymbol);
        var collectionNullableInfo = infos.Get(collectionNullable);
        Assert.Equal(TypeSymbolKind.Collection, collectionNullableInfo.Kind);
        Assert.True(collectionNullableInfo.IsNullable);
        Assert.False(collectionInfo.IsInterface);
    }
    [Fact]
    public void Collection_IList()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<AttributeTargets>();
        var compilation = driver.Compile("");
        var stringSymbol = compilation.GetStringSymbol();
        var collectionSymbol = compilation.GetIList(stringSymbol);
        Assert.NotNull(collectionSymbol);
        var infos = new TypeSymbolCacher(compilation);
        var collectionInfo = infos.Get(collectionSymbol) as CollectionTypeInfo;
        Assert.NotNull(collectionInfo);
        Assert.False(collectionInfo.IsNullable);
        var collectionNullable = compilation.GetNullable(collectionSymbol);
        var collectionNullableInfo = infos.Get(collectionNullable) as CollectionTypeInfo;
        Assert.NotNull(collectionNullableInfo);
        Assert.True(collectionNullableInfo.IsNullable);
        Assert.True(collectionInfo.IsInterface);
    }
    [Fact]
    public void Collection_IEnumerable()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<AttributeTargets>();
        var compilation = driver.Compile("");
        var stringSymbol = compilation.GetStringSymbol();
        var collectionSymbol = compilation.GetIEnumerable(stringSymbol);
        Assert.NotNull(collectionSymbol);
        var infos = new TypeSymbolCacher(compilation);
        var collectionInfo = infos.Get(collectionSymbol) as CollectionTypeInfo;
        Assert.NotNull(collectionInfo);
        Assert.False(collectionInfo.IsNullable);
        var collectionNullable = compilation.GetNullable(collectionSymbol);
        var collectionNullableInfo = infos.Get(collectionNullable) as CollectionTypeInfo;
        Assert.NotNull(collectionNullableInfo);
        Assert.True(collectionNullableInfo.IsNullable);
        Assert.True(collectionInfo.IsInterface);
    }
    [Fact]
    public void Complex()
    {
        var driver = SyntaxTreeDriver.CreateDriver();
        var sourceCode = "public record struct User(long Id, string Name);";
        var compilation = driver.Compile(sourceCode);
        var complexSymbol = compilation.GetTypeByMetadataName("User");
        Assert.NotNull(complexSymbol);
        var infos = new TypeSymbolCacher(compilation);
        var complexInfo = infos.Get(complexSymbol) as ComplexTypeInfo;
        Assert.NotNull(complexInfo);
        //Assert.Equal(TypeSymbolKind.Complex, complexInfo.Kind);
        Assert.False(complexInfo.IsNullable);
        Assert.False(complexInfo.IsInterface);
        var complexNullable = compilation.GetNullable(complexSymbol);
        var complexNullableInfo = infos.Get(complexNullable) as ComplexTypeInfo;
        Assert.NotNull(complexNullableInfo);
        //Assert.Equal(TypeSymbolKind.Complex, complexNullableInfo.Kind);
        Assert.True(complexNullableInfo.IsNullable);
        Assert.False(complexNullableInfo.IsInterface);
    }
    [Fact]
    public void Entity()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<IEntityId>();
        var sourceCode = @"using Hand.Models;
            public record struct UserId(long Original) : IEntityId;";
        var compilation = driver.Compile(sourceCode);
        var entitySymbol = compilation.GetTypeByMetadataName("UserId");
        Assert.NotNull(entitySymbol);
        var infos = new TypeSymbolCacher(compilation);
        var entityInfo = infos.Get(entitySymbol) as EntityTypeInfo;
        Assert.NotNull(entityInfo);
        //Assert.Equal(TypeSymbolKind.Entity, entityInfo.Kind);
        Assert.False(entityInfo.IsNullable);
        Assert.False(entityInfo.IsInterface);
        var entityNullable = compilation.GetNullable(entitySymbol);
        var entityNullableInfo = infos.Get(entityNullable) as EntityTypeInfo;
        Assert.NotNull(entityNullableInfo);
        //Assert.Equal(TypeSymbolKind.Entity, entityNullableInfo.Kind);
        Assert.True(entityNullableInfo.IsNullable);
        Assert.False(entityNullableInfo.IsInterface);
    }
    [Fact]
    public void EntityIsInterface()
    {
        var driver = SyntaxTreeDriver.CreateDriver()
            .Reference<IEntityId>();
        var compilation = driver.Compile("");
        var entitySymbol = compilation.GetTypeByMetadataName("Hand.Models.IEntityId");
        Assert.NotNull(entitySymbol);
        var infos = new TypeSymbolCacher(compilation);
        var entityInfo = infos.Get(entitySymbol) as EntityTypeInfo;
        Assert.NotNull(entityInfo);
        //Assert.Equal(TypeSymbolKind.Entity, entityInfo.Kind);
        Assert.False(entityInfo.IsNullable);
        Assert.True(entityInfo.IsInterface);
        var entityNullable = compilation.GetNullable(entitySymbol);
        var entityNullableInfo = infos.Get(entityNullable) as EntityTypeInfo;
        Assert.NotNull(entityNullableInfo);
        //Assert.Equal(TypeSymbolKind.Entity, entityNullableInfo.Kind);
        Assert.True(entityNullableInfo.IsNullable);
        Assert.True(entityNullableInfo.IsInterface);
    }
}
