using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Hand.SyntaxGenerator;

namespace EasySyntaxTests;

public class WhileTests
{
    [Fact]
    public void GetIds()
    {
        var readerType = SyntaxFactory.IdentifierName("DbDataReader");        
        var reader = SyntaxFactory.IdentifierName("reader");
        var listType = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
        var list = SyntaxFactory.IdentifierName("list");
        // reader.GetFieldValue<int>
        var getFieldValue = SyntaxGenerator.Generic("GetFieldValue", SyntaxGenerator.IntType).Qualified(reader);
        // List<int> GetIds(DbDataReader reader)
        var method = listType.Method("GetIds", readerType.Parameter(reader.Identifier))
            .ToBuilder()
            // List<int> list = []
            .Declare(listType.Variable(list.Identifier, SyntaxFactory.CollectionExpression()))
            // while(reader.Read()){
            .While(reader.Access("Read").Invocation())
                // list.Add(reader.GetFieldValue(0))
                .Add(list.Access("Add").Invocation([getFieldValue.Invocation([SyntaxGenerator.Literal(0)])]))
            // }
            .End()
            // return list
            .Return(list);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void GetIds0()
    {
        var readerType = SyntaxFactory.IdentifierName("DbDataReader");
        var reader = SyntaxFactory.IdentifierName("reader");
        var listType = SyntaxFactory.GenericName("List")
            .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
        var list = SyntaxFactory.IdentifierName("list");
        // reader.GetFieldValue<int>
        var getFieldValue = SyntaxFactory.QualifiedName(
            reader, 
            SyntaxFactory.GenericName("GetFieldValue")
                .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))));
        var method = SyntaxFactory.MethodDeclaration(listType, "GetIds")
            .AddParameterListParameters(
                SyntaxFactory.Parameter(reader.Identifier)
                    .WithType(readerType)
            )
            .AddBodyStatements(
                SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    listType, SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(list.Identifier)
                            .WithInitializer(SyntaxFactory.CollectionExpression())))),
                SyntaxFactory.WhileStatement(
                    SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, reader, SyntaxFactory.IdentifierName("Read"))),
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, list, SyntaxFactory.IdentifierName("Add")))
                            .AddArgumentListArguments(SyntaxFactory.Argument(
                                SyntaxFactory.InvocationExpression(getFieldValue)
                                .AddArgumentListArguments(SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0))
                                ))
                            ))
                    )
                ),
                SyntaxFactory.ReturnStatement(list)
            );

        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }
    [Fact]
    public void GetIds2()
    {
        var readerType = SyntaxFactory.IdentifierName("DbDataReader");
        var reader = SyntaxFactory.IdentifierName("reader");
        var listType = Generic("List", IntType);
        var list = SyntaxFactory.IdentifierName("list");
        // reader.Read
        var read = reader.Access("Read");
        // reader.GetFieldValue<int>
        var getFieldValue = Generic("GetFieldValue", IntType).Qualified("reader");
        // List<int> GetIds(DbDataReader reader)
        // list.Add
        var add = list.Access("Add");
        // List<int> GetIds(DbDataReader reader)
        var method = listType.Method("GetIds", readerType.Parameter(reader.Identifier))
            .ToBuilder()
            // List<int> list = []
            .Declare(listType.Variable(list.Identifier, SyntaxFactory.CollectionExpression()))
            // while(reader.Read()){
            .While(read.Invocation())
                // list.Add(reader.GetFieldValue(0))
                .Add(add.Invocation([getFieldValue.Invocation([Literal(0)])]))
            // }
            .End()
            // return list
            .Return(list);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
        //SyntaxFactory.DoStatement()
        //SyntaxFactory.LabeledStatement()
        //SyntaxFactory.ThrowExpression()
        //Exception
        SyntaxFactory.BreakStatement();
        SyntaxFactory.ContinueStatement();
        //SyntaxFactory.GotoStatement()
    }
}
