using Hand;
using Hand.Enumerations;
using Hand.GenerateEnumeration;
using Hand.Primitives;

namespace GenerateEnumerationTests.FlagEnum2;

public class FileAccessTests
{
    [Fact]
    public void Generator()
    {
        var sourceCode = @"
using System.IO;
using Hand;
using Hand.Enumerations;
using Hand.Primitives;

namespace GenerateEnumerationTests;

[GenerateEnumeration<FileAccess>]
public partial class FileAccessEnumeration;
";
        var driver = SyntaxTreeDriver.CreateDefaultDriver()
            .Reference<Enumeration>()
            .Reference(typeof(GenerateEnumerationAttribute<>));
        var result = driver.Generate<EnumerationGenerator>(sourceCode)
            .GetRunResult();
        var syntaxTree = result.GeneratedTrees.FirstOrDefault();
        Assert.NotNull(syntaxTree);
        var code = syntaxTree.GetText().ToString();
        Assert.Contains("FileAccessEnumerationProvider", code);
    }
}

[GenerateEnumeration<FileAccess>]
public partial class FileAccessEnumeration;

//partial class FileAccessEnumeration : FlagEnumeration
//{
//    ///<summary>
//    ///构造函数
//    ///</summary>
//    private FileAccessEnumeration(ISet<string> flags, long original, string description) : base(flags, original, description)
//    {
//    }

//    ///<summary>
//    ///构造函数
//    ///</summary>
//    private FileAccessEnumeration(string name, long original, string description) : base(name, original, description)
//    {
//    }

//    public static readonly FileAccessEnumeration Read = new("Read", 1L, "");
//    public static readonly FileAccessEnumeration Write = new("Write", 2L, "");
//    public static readonly FileAccessEnumeration ReadWrite = new("ReadWrite", 3L, "");
//    ///<summary>
//    ///逻辑空
//    ///</summary>
//    public static readonly FileAccessEnumeration Empty = new("Empty", 0L, "逻辑空");
//    ///<summary>
//    ///提供器
//    ///</summary>
//    public static readonly IFlagEnumerationProvider<FileAccessEnumeration> Provider = new FileAccessEnumerationProvider();
//    ///<summary>
//    ///提供器
//    ///</summary>
//    private class FileAccessEnumerationProvider : FlagEnumerationProvider<FileAccessEnumeration>
//    {
//        ///<summary>
//        ///构造函数
//        ///</summary>
//        internal FileAccessEnumerationProvider() : base([FileAccessEnumeration.Empty, FileAccessEnumeration.Read, FileAccessEnumeration.Write, FileAccessEnumeration.ReadWrite], [FileAccessEnumeration.Read, FileAccessEnumeration.Write], new Dictionary<string, FileAccessEnumeration> { { "Empty", FileAccessEnumeration.Empty }, { "Read", FileAccessEnumeration.Read }, { "Write", FileAccessEnumeration.Write }, { "ReadWrite", FileAccessEnumeration.ReadWrite } }, new Dictionary<long, FileAccessEnumeration> { { 0L, FileAccessEnumeration.Empty }, { 1L, FileAccessEnumeration.Read }, { 2L, FileAccessEnumeration.Write }, { 3L, FileAccessEnumeration.ReadWrite } })
//        {
//        }

//        ///<summary>
//        ///逻辑空
//        ///</summary>
//        public override FileAccessEnumeration Empty => FileAccessEnumeration.Empty;

//        ///<inheritdoc/>
//        protected override FileAccessEnumeration CreateFlag(ISet<string> flags, long original, string description) => new(flags, original, description);
//    }
//}
