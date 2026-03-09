using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using static Hand.SyntaxGenerator;

namespace EasySyntaxTests;

public class DoTests
{
    [Fact]
    public void DoSTh()
    {
        var console = SyntaxFactory.IdentifierName("Console");
        var writeLine = console.Access("WriteLine");
        var thing = SyntaxFactory.IdentifierName("thing");
        // void DoSTh()
        var method = SyntaxGenerator.VoidType.Method("DoSTh")
            // {
            .ToBuilder()
            // string? thing
            .Declare(SyntaxGenerator.StringType.Nullable().Variable(thing.Identifier))
            // Console.WriteLine("Enter some things:")
            .Add(writeLine.Invocation([SyntaxGenerator.Literal("Enter some things:")]))
            // do ... while(thing!="exit"){
            .Do(thing.NotEqual(SyntaxGenerator.Literal("exit")))
                // thing=Console.ReadLine()
                .Add(thing.Assign(console.Access("ReadLine").Invocation()))
                // Console.Write("Do ")
                .Add(console.Access("Write").Invocation([SyntaxGenerator.Literal("Do ")]))
                // Console.WriteLine(thing)
                .Add(writeLine.Invocation([thing]))
                // }
                .End()
            // }
            .End();
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void DoSTh0()
    {
        var console = SyntaxFactory.IdentifierName("Console");
        var writeLine = console.Access("WriteLine");
        var thing = SyntaxFactory.IdentifierName("thing");
        var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "DoSTh")
            .AddBodyStatements(
                SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))), SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(thing.Identifier)))),
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(writeLine)
                       .AddArgumentListArguments(SyntaxFactory.Argument(
                           SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Enter some things:"))
                       ))
                ),
                SyntaxFactory.DoStatement(
                    SyntaxFactory.Block(
                        SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                            thing,
                            SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, console, SyntaxFactory.IdentifierName("ReadLine"))))),
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, console, SyntaxFactory.IdentifierName("Write")))
                                .AddArgumentListArguments(SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Do "))
                                ))
                        ),
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.InvocationExpression(writeLine)
                                .AddArgumentListArguments(SyntaxFactory.Argument(thing))
                        )
                    ),
                    thing.NotEqual(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("exit"))))
            );
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void DoSTh2()
    {
        var console = SyntaxFactory.IdentifierName("Console");
        var write = console.Access("Write");
        var writeLine = console.Access("WriteLine");
        var readLine = console.Access("ReadLine");
        var thing = SyntaxFactory.IdentifierName("thing");
        // void DoSTh()
        var method = VoidType.Method("DoSTh")
            // {
            .ToBuilder()
            // string? thing
            .Declare(StringType.Nullable().Variable(thing.Identifier))
            // Console.WriteLine("Enter some things:")
            .Add(writeLine.Invocation([Literal("Enter some things:")]))
            // do ... while(thing!="exit"){
            .Do(thing.NotEqual(Literal("exit")))
                // thing=Console.ReadLine()
                .Add(thing.Assign(readLine.Invocation()))
                // Console.Write("Do ")
                .Add(write.Invocation([Literal("Do ")]))
                // Console.WriteLine(thing)
                .Add(writeLine.Invocation([thing]))
                // }
                .End()
            // }
            .End();
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    //void DoSTh()
    //{
    //    string? thing;
    //    Console.WriteLine("Enter some things:");
    //    do
    //    {
    //        thing = Console.ReadLine();
    //        Console.Write("Do ");
    //        Console.WriteLine(thing);
    //    }
    //    while (thing != "exit");
    //}
}
