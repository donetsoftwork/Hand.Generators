using GenerateConvertTests.Supports;
using Hand;
using Hand.Enums;
using Hand.Reflection;

namespace GenerateConvertTests.Enums;

public class FlagEnumBundleTests
{
    [Fact]
    public void GetFieldsByName()
    {
        var source = "var columnType = ColumnType.Unique;";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<ColumnType>(true)
            //.WithDocumentationComments()
            .Using("System");
        var compilation = driver.Compile(source);
        var type = compilation.GetTypeByMetadataName("GenerateConvertTests.Supports.ColumnType");
        Assert.NotNull(type);
        var field = SymbolReflection.GetEnumField(type, (short)1);
        Assert.NotNull(field);
        //<member name = "F:GenerateConvertTests.Supports.ColumnType.Identity" >
        //    <summary >
        //    自增列
        //    </summary >
        //</member >
        var xml = field.GetDocumentationCommentXml();
        Assert.NotNull(xml);
        var id = field.GetDocumentationCommentId();
        Assert.NotNull(id);
        var builder = new EnumBundleBuilder(compilation);
        var bundle = builder.Get(type);
        Assert.NotNull(bundle);
        var key = bundle.GetFieldByName("Key");
        Assert.NotNull(key);

        if (bundle is FlagEnumBundle enumBundle)
        {
            var flagFlag = enumBundle.GetFieldsByFlag(2);
            Assert.NotNull(flagFlag);
            var fields = enumBundle.GetFieldsByName("Key", "N").ToArray();
            Assert.Single(fields);
            Assert.Equal(6, enumBundle.Fields.Count);
        }
        else
        {
            Assert.False(false);
        }
    }

    
}


