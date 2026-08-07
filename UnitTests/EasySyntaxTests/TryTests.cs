using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class TryTests
{
    [Fact]
    public void Try()
    {
        var fileName = SyntaxFactory.IdentifierName("fileName");
        var fs = SyntaxFactory.IdentifierName("fs");
        var size = SyntaxFactory.IdentifierName("size");
        var ex = SyntaxFactory.IdentifierName("ex");
        var fileOpenRead = SyntaxFactory.IdentifierName("File").Access("OpenRead");
        var consoleWriteLine = SyntaxFactory.IdentifierName("Console").Access("WriteLine");
        var method = SyntaxGenerator.LongType.Method("ReadSize", SyntaxGenerator.StringType.Parameter(fileName.Identifier))
            .ToBuilder()
            .Declare(SyntaxFactory.IdentifierName("FileStream").Nullable().Variable(fs.Identifier, SyntaxGenerator.NullLiteral))
            .Declare(SyntaxGenerator.LongType.Variable(size.Identifier))
            .Try()
                .AddExpression(fs.Assign(fileOpenRead.Invocation([fileName])))
                .AddExpression(size.Assign(fs.Access("Length")))
            .Catch(SyntaxFactory.IdentifierName("Exception").Catch(ex.Identifier))
                .AddExpression(consoleWriteLine.Invocation([ex.Access("Message")]))
                .AddExpression(size.Assign(SyntaxGenerator.Literal(-1)))
            .Finally()
                .AddExpression(fs.ConditionalInvocation(SyntaxFactory.IdentifierName("Close")))
            .End()
            .Return(size);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void Try2()
    {
        var fileName = SyntaxFactory.IdentifierName("fileName");
        var fs = SyntaxFactory.IdentifierName("fs");
        var size = SyntaxFactory.IdentifierName("size");
        var ex = SyntaxFactory.IdentifierName("ex");
        var fileOpenRead = SyntaxFactory.IdentifierName("File").Access("OpenRead");
        var consoleWriteLine = SyntaxFactory.IdentifierName("Console").Access("WriteLine");

        var @try = SyntaxGenerator
            .Try()
                .AddExpression(fs.Assign(fileOpenRead.Invocation([fileName])))
                .AddExpression(size.Assign(fs.Access("Length")))
            .Catch(SyntaxFactory.IdentifierName("Exception").Catch(ex.Identifier))
                .AddExpression(consoleWriteLine.Invocation([ex.Access("Message")]))
                .AddExpression(size.Assign(SyntaxGenerator.Literal(-1)))
            .Finally()
                .AddExpression(fs.ConditionalInvocation(SyntaxFactory.IdentifierName("Close")))
             .Build();

        var body = SyntaxGenerator.Scope()
            .Declare(SyntaxFactory.IdentifierName("FileStream").Nullable().Variable(fs.Identifier, SyntaxGenerator.NullLiteral))
            .Declare(SyntaxGenerator.LongType.Variable(size.Identifier))
            .Add(@try)
            .Add(size.Return())
            .Block();

        var method = SyntaxGenerator.LongType.Method("ReadSize", SyntaxGenerator.StringType.Parameter(fileName.Identifier))
            .WithBody(body);

        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }

    //long ReadSize(string fileName)
    //{
    //    FileStream? fs = null;
    //    long size;
    //    try
    //    {
    //        fs = File.OpenRead(fileName);
    //        size = fs.Length;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //        size = -1;
    //    }
    //    finally
    //    {
    //        fs?.Close();
    //    }
    //    return size;
    //}
}
