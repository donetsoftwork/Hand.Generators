using Hand;
using Hand.Patterns;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;

namespace EasySyntaxTests;

public class PatternTests
{
    [Fact]
    public void Declaration()
    {
        var pattern = SyntaxGenerator.IntType.VariablePattern("i");
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("int i", code);
    }
    [Fact]
    public void IsType()
    {
        var obj = SyntaxFactory.IdentifierName("obj");
        var expression = obj.IsType(SyntaxGenerator.IntType);
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("obj is int", code);
    }
    [Fact]
    public void IsType2()
    {
        var obj = SyntaxFactory.IdentifierName("obj");
        var expression = obj.IsType(SyntaxGenerator.IntType, "i");
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("obj is int i", code);
    }
    [Fact]
    public void Is()
    {
        var pattern = SyntaxGenerator.IntType.VariablePattern("i");
        var obj = SyntaxFactory.IdentifierName("obj");
        var expression = obj.Is(pattern);
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("obj is int i", code);
    }
    [Fact]
    public void Apple()
    {
        var fruit = SyntaxFactory.IdentifierName("fruit");
        var appleType = SyntaxFactory.IdentifierName("Apple");
        var statement = fruit.IsType(appleType)
            .If()
            .Add(SyntaxGenerator.Literal("I like Apple!").Return())
            .Build();
        var code = statement.NormalizeWhitespace().ToFullString();
        Assert.Contains("fruit is Apple", code);

        var statement0 = SyntaxFactory.IfStatement(
            SyntaxFactory.IsPatternExpression(fruit, SyntaxFactory.TypePattern(appleType)), 
            SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, 
                SyntaxFactory.Literal("I like Apple!"))));
        var code0 = statement0.NormalizeWhitespace().ToFullString();
        Assert.Contains("fruit is Apple", code0);
    }
    [Fact]
    public void MakeApplePie()
    {
        var fruit = SyntaxFactory.IdentifierName("fruit");
        var appleType = SyntaxFactory.IdentifierName("Apple");
        var apple = SyntaxFactory.IdentifierName("apple");
        var makeApplePieMethod = SyntaxFactory.IdentifierName("MakeApplePie");
        var statement = fruit.Is(appleType.VariablePattern(apple.Identifier))
            .If()
                .AddPatter(makeApplePieMethod.Invocation([apple]))
            .Build();
        var code = statement.NormalizeWhitespace().ToFullString();
        Assert.Contains("MakeApplePie", code);

        var statement0 = SyntaxFactory.IfStatement(
            SyntaxFactory.IsPatternExpression(fruit, 
                SyntaxFactory.DeclarationPattern(appleType, SyntaxFactory.SingleVariableDesignation(apple.Identifier))),
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(makeApplePieMethod, 
                    SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(apple))))));
        var code0 = statement0.NormalizeWhitespace().ToFullString();
        Assert.Contains("MakeApplePie", code0);
    }
    [Fact]
    public void IsNull()
    {
        var obj = SyntaxFactory.IdentifierName("obj");
        var expression = obj.Is(SyntaxGenerator.NullLiteral.ToPattern());
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("obj is null", code);

        var expression0 = SyntaxFactory.IsPatternExpression(obj, 
            SyntaxFactory.ConstantPattern(
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
        var code0 = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("obj is null", code0);
    }
    [Fact]
    public void NotNull()
    {
        var obj = SyntaxFactory.IdentifierName("obj");
        var expression = obj.Is(SyntaxGenerator.NotNullPattern);
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("obj is not null", code);
    }
    [Fact]
    public void GreaterThan()
    {
        var pattern = SyntaxGenerator.GreaterThanPattern(0);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("> 0", code);
    }
    [Fact]
    public void GreaterOrEqual()
    {
        var pattern = SyntaxGenerator.GreaterOrEqualPattern(3);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal(">= 3", code);
    }
    [Fact]
    public void LessThan()
    {
        var pattern = SyntaxGenerator.LessThanPattern(3);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("< 3", code);
    }
    [Fact]
    public void LessOrEqual()
    {
        var pattern = SyntaxGenerator.LessOrEqualPattern(3);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("<= 3", code);
    }
    [Fact]
    public void Equal()
    {
        var pattern = SyntaxGenerator.EqualPattern(3);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("== 3", code);
    }
    [Fact]
    public void NotEqual()
    {
        var pattern = SyntaxGenerator.NotEqualPattern(3);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("!= 3", code);
        var pattern0 = SyntaxFactory.RelationalPattern(
            SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken), 
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                SyntaxFactory.Literal(3)));
        var code0 = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("!= 3", code0);
    }
    [Fact]
    public void Var()
    {
        //var pattern = SyntaxFactory.VarPattern(SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("i")));
        var pattern = SyntaxGenerator.VarPattern("i");
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("var i", code);
        var pattern0 = SyntaxGenerator.VarPattern();
        var code0 = pattern0.NormalizeWhitespace().ToFullString();
        Assert.Equal("var _", code0);
    }
    [Fact]
    public void Var2()
    {
        var getScoresMethod = SyntaxFactory.IdentifierName("GetScores");
        var scores = SyntaxFactory.IdentifierName("scores");
        var id = SyntaxFactory.IdentifierName("id");
        var expression = getScoresMethod.Invocation([id])
            .Is(SyntaxGenerator.VarPattern(scores.Identifier))
            .LogicalAnd(scores.Access("Average").Invocation().GreaterOrEqual(SyntaxGenerator.Literal(60)));
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("GetScores(id)is var scores && scores.Average() >= 60", code);


        var expression0 = SyntaxFactory.BinaryExpression(SyntaxKind.LogicalAndExpression, 
            SyntaxFactory.IsPatternExpression(
                SyntaxFactory.InvocationExpression(getScoresMethod, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(id)))), 
                SyntaxFactory.VarPattern(SyntaxFactory.SingleVariableDesignation(scores.Identifier))),
            SyntaxFactory.BinaryExpression(SyntaxKind.GreaterThanOrEqualExpression,
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, 
                        scores, 
                        SyntaxFactory.IdentifierName("Average"))), 
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                    SyntaxFactory.Literal(60))));
        var code0 = expression0.NormalizeWhitespace().ToFullString();
        Assert.Equal("GetScores(id)is var scores && scores.Average() >= 60", code);
    }
    [Fact]
    public void Discard()
    {
        var date = SyntaxFactory.IdentifierName("date");
        var expression = date.Access("Day")
            .SwitchExpression()
            .Case(SyntaxGenerator.Literal(15), SyntaxGenerator.Literal("今夜是月圆之夜"))
            .Case(SyntaxFactory.DiscardPattern(), SyntaxGenerator.Literal("月圆要等到十五"))
            .Switch.Build();
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Contains("今夜是月圆之夜", code);
    }
    [Fact]
    public void Discard2()
    {
        var date = SyntaxFactory.IdentifierName("date");
        var expression = date.Access("Day")
            .SwitchExpression()
            .Case(SyntaxGenerator.Literal(15), SyntaxGenerator.Literal("今夜是月圆之夜"))
            .Default(SyntaxGenerator.Literal("月圆要等到十五"))
            .Build();
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Contains("今夜是月圆之夜", code);
    }
    [Fact]
    public void Or()
    {
        var left = SyntaxGenerator.Literal(1).ToPattern();
        var right = SyntaxGenerator.Literal(2).ToPattern();
        var pattern = left.Or(right);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("1 or 2", code);
    }
    [Fact]
    public void OrPattern()
    {
        var left = SyntaxGenerator.Literal(1);
        var right = SyntaxGenerator.Literal(2);
        //var pattern = SyntaxFactory.BinaryPattern(SyntaxKind.OrPattern, SyntaxFactory.ConstantPattern(left), SyntaxFactory.Token(SyntaxKind.OrKeyword), SyntaxFactory.ConstantPattern(right));
        var pattern = left.OrPattern(right);
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("1 or 2", code);
    }
    [Fact]
    public void And()
    {
        var pattern = SyntaxGenerator.GreaterThanPattern(0)
            .And(SyntaxGenerator.LessThanPattern(10));
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("> 0 and < 10", code);

        var pattern0 = SyntaxFactory.BinaryPattern(SyntaxKind.AndPattern,
            SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.GreaterThanToken),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(0))),
            SyntaxFactory.Token(SyntaxKind.AndKeyword),
            SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.LessThanToken),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                    SyntaxFactory.Literal(10))));
        var code0 = pattern0.NormalizeWhitespace().ToFullString();
        Assert.Equal("> 0 and < 10", code0);
    }
    [Fact]
    public void Not()
    {
        var pattern = SyntaxGenerator.NullLiteral
            .ToPattern()
            .Not();
        var obj = SyntaxFactory.IdentifierName("obj");
        var expression = obj.Is(pattern);
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("obj is not null", code);
    }
    [Fact]
    public void Property()
    {
        var nationalDays = new PropertyPatternBuilder(null)
            .Add("Month", SyntaxGenerator.Literal(10))
            .Add("Day", SyntaxGenerator.LessOrEqualPattern(7))
            .Build();
        var code = nationalDays.NormalizeWhitespace().ToFullString();
        Assert.Equal("{ Month: 10, Day: <= 7 }", code);
        var date = SyntaxFactory.IdentifierName("date");
        var body = date.Is(nationalDays);
        var method = SyntaxGenerator.BoolType.Method("IsNationalDay", SyntaxGenerator.DateTimeType.Parameter(date.Identifier))
            .WithExpressionBody(body);
        code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("IsNationalDay", code);
        SyntaxFactory.Subpattern(SyntaxFactory.NameColon("Month"), SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(10))));
        SyntaxFactory.Subpattern(SyntaxFactory.NameColon("Day"), SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.LessThanEqualsToken), SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(7))));
        var nationalDays0 = SyntaxFactory.RecursivePattern(
            null, 
            null, 
            SyntaxFactory.PropertyPatternClause(SyntaxFactory.SeparatedList([
                SyntaxFactory.Subpattern(
                    SyntaxFactory.NameColon("Month"), 
                    SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                        SyntaxFactory.Literal(10)))),
                SyntaxFactory.Subpattern(
                    SyntaxFactory.NameColon("Day"), 
                    SyntaxFactory.RelationalPattern(
                        SyntaxFactory.Token(SyntaxKind.LessThanEqualsToken), 
                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                            SyntaxFactory.Literal(7))))])),
            null);
        var code0 = nationalDays0.NormalizeWhitespace().ToFullString();
        Assert.Equal("{ Month: 10, Day: <= 7 }", code0);
    }
    [Fact]
    public void FindFoot()
    {
        var thing = SyntaxFactory.IdentifierName("thing");
        var food = SyntaxFactory.IdentifierName("food");
        var foodType = SyntaxFactory.IdentifierName("Food");
        var now = SyntaxGenerator.DateTimeType.Access("Now");
        var pattern = new PropertyPatternBuilder(foodType, food.Identifier)
             .Add("ExpirationDate", SyntaxGenerator.GreaterThanPattern(now))
             .Build();
        var method = foodType.Nullable().Method("FindFoot", SyntaxGenerator.ObjectType.Parameter(thing.Identifier))
            .ToBuilder()
            .If(thing.Is(pattern))
            .Return(food)
            .Return(SyntaxGenerator.NullLiteral);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("FindFoot", code);
    }
    [Fact]
    public void IsConferenceDay()
    {
        // https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#property-pattern
        var pattern = new PropertyPatternBuilder(null)
            .Add("Year", SyntaxGenerator.Literal(2020))
            .Add("Month", SyntaxGenerator.Literal(5))
            .Add("Day", SyntaxGenerator.OrPattern(SyntaxGenerator.Literal(19), SyntaxGenerator.Literal(20), SyntaxGenerator.Literal(21)))
            .Build();
        var date = SyntaxFactory.IdentifierName("date");
        var body = date.Is(pattern);
        var method = SyntaxGenerator.BoolType.Method("IsConferenceDay", SyntaxGenerator.DateTimeType.Parameter(date.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("IsConferenceDay", code);
    }
    //bool IsConferenceDay(DateTime date) => date is { Year: 2020, Month: 5, Day: 19 or 20 or 21 };
    [Fact]
    public void List()
    {
        var name = SyntaxFactory.IdentifierName("name");
        var pattern = new ListPatternBuilder()
            .Add(SyntaxGenerator.Literal('曾'))
            .Add(SyntaxGenerator.Literal('国'))
            .Add(SyntaxFactory.DiscardPattern())
            .Build();
        var expression = name.Is(pattern);
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("name is ['曾', '国', _ ]", code);

        var pattern0 = SyntaxFactory.ListPattern(SyntaxFactory.SeparatedList<PatternSyntax>([
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, 
                SyntaxFactory.Literal('曾'))),
            SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, 
                SyntaxFactory.Literal('国'))),
            SyntaxFactory.DiscardPattern()]));
        var expression0 = name.Is(pattern0);
        var code0 = expression0.NormalizeWhitespace().ToFullString();
        Assert.Equal("name is ['曾', '国', _ ]", code0);
    }
    [Fact]
    public void Slice()
    {
        //var pattern = SyntaxFactory.SlicePattern(SyntaxFactory.ConstantPattern(SyntaxFactory.IdentifierName("list")));
        var pattern = SyntaxFactory.IdentifierName("list").Slice();
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("..list", code);
        var pattern0 = SyntaxFactory.SlicePattern();
        var code0 = pattern0.NormalizeWhitespace().ToFullString();
        Assert.Equal("..", code0);
    }
    [Fact]
    public void Slice2()
    {
        // 属性模式
        var middle = new PropertyPatternBuilder(null, "middle")
            // or逻辑模式
            .Add("Length", SyntaxGenerator.Literal(2).OrPattern(SyntaxGenerator.Literal(4)))
            .Build();
        var pattern = new ListPatternBuilder()
            // 关系模式
            .Add(SyntaxGenerator.GreaterThanPattern(0))
            // 弃元模式
            .Add(SyntaxFactory.DiscardPattern())
            // 切片模式
            .Add(middle.Slice())
            // 常量模式
            .Add(SyntaxGenerator.Literal(9))
            //  var 模式
            .Add(SyntaxGenerator.VarPattern("last"))
            .Build();
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("[> 0, _, ..{ Length: 2 or 4 } middle, 9, var last]", code);
    }
    [Fact]
    public void IsPalindrome()
    {
        var input = SyntaxFactory.IdentifierName("input");
        var inputType = SyntaxGenerator.Generic("ReadOnlySpan", SyntaxGenerator.CharType);
        var isPalindromeMethod = SyntaxFactory.IdentifierName("IsPalindrome");
        var first = SyntaxFactory.IdentifierName("first");
        var middle = SyntaxFactory.IdentifierName("middle");
        var last = SyntaxFactory.IdentifierName("last");
        var middleSlice = SyntaxGenerator.VarPattern(middle.Identifier)
            .Slice();
        var pattern = new ListPatternBuilder()
            .Add(SyntaxGenerator.VarPattern(first.Identifier))
            .Add(middleSlice)
            .Add(SyntaxGenerator.VarPattern(last.Identifier))
            .Build();
        var expression1 = input.Is(pattern.Not());
        var expression2 = first.Equal(last).LogicalAnd(isPalindromeMethod.Invocation([middle]));
        var body = expression1.LogicalOr(expression2.Parenthesized());
        var method = SyntaxGenerator.BoolType.Method(isPalindromeMethod.Identifier, inputType.Parameter(input.Identifier))
            .Static()
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("IsPalindrome", code);

        var middleSlice0 = SyntaxFactory.SlicePattern(SyntaxGenerator.VarPattern(middle.Identifier));
        var code0 = middleSlice0.NormalizeWhitespace().ToFullString();
        Assert.Equal("..var middle", code0);
    }
    //static bool IsPalindrome(ReadOnlySpan<char> input) => input is not [var first, ..var middle, var last] || (first == last && IsPalindrome(middle));
    //{
    //    if (input is [var first, .. var middle, var last])
    //        return first == last && IsPalindrome(middle);
    //    return true;
    //}
    [Fact]
    public void Recursive()
    {
        var point = SyntaxFactory.IdentifierName("point");
        var builder = new RecursivePatternBuilder(null);
        builder.Positional.Add(SyntaxGenerator.GreaterOrEqualPattern(0))
            .Add(SyntaxGenerator.GreaterOrEqualPattern(0));
        builder.Property.Add("Weight", SyntaxGenerator.GreaterThanPattern(0));
        var isInDomain = point.Is(builder.Build());
        var code = isInDomain.NormalizeWhitespace().ToFullString();
        Assert.Contains("point is (>= 0, >= 0) { Weight: > 0 }", code);
    }
    [Fact]
    public void Recursive0()
    {
        var pattern = SyntaxFactory.RecursivePattern()
            .WithPropertyPatternClause(
                SyntaxFactory.PropertyPatternClause(
                    SyntaxFactory.SeparatedList(
                    [
                                    SyntaxFactory.Subpattern(
                                        SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("Success")),
                                        SyntaxFactory.ConstantPattern(
                                            SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))
                    ])))
            .WithDesignation(
                SyntaxFactory.SingleVariableDesignation(
                    SyntaxFactory.Identifier("a")));
        //SyntaxFactory.RecursivePattern()
        var code = pattern.NormalizeWhitespace().ToFullString();
        Assert.Equal("{ Success: true } a", code);
    }
    [Fact]
    public void Parenthesized()
    {
        // https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#parenthesized-pattern
        var input = SyntaxFactory.IdentifierName("input");
        var pattern = SyntaxFactory.TypePattern(SyntaxGenerator.FloatType)
            .Or(SyntaxFactory.TypePattern(SyntaxGenerator.DoubleType))
            .Parenthesized()
            .Not();
        var expression = input.Is(pattern);
        var code = expression.NormalizeWhitespace().ToFullString();
        Assert.Equal("input is not (float or double)", code);

        var pattern0 = SyntaxFactory.UnaryPattern(
            SyntaxFactory.ParenthesizedPattern(
                SyntaxFactory.BinaryPattern(
                    SyntaxKind.OrPattern, 
                    SyntaxFactory.TypePattern(
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword))), 
                    SyntaxFactory.Token(SyntaxKind.OrKeyword), 
                    SyntaxFactory.TypePattern(
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword))))));
        var expression0 = SyntaxFactory.IsPatternExpression(input, pattern0);
        var code0 = expression0.NormalizeWhitespace().ToFullString();
        Assert.Equal("input is not (float or double)", code0);
    }
    [Fact]
    public void IsLetter()
    {
        // https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#property-pattern
        // >=a and <=z
        var patter1 = SyntaxGenerator.GreaterOrEqualPattern(SyntaxGenerator.Literal('a'))
            .And(SyntaxGenerator.LessOrEqualPattern(SyntaxGenerator.Literal('z')));
        // >=A AND <=Z
        var patter2 = SyntaxGenerator.GreaterOrEqualPattern(SyntaxGenerator.Literal('A'))
            .And(SyntaxGenerator.LessOrEqualPattern(SyntaxGenerator.Literal('Z')));
        var pattern = patter1.Parenthesized().Or(patter2.Parenthesized());
        var c = SyntaxFactory.IdentifierName("c");
        var body = c.Is(pattern);
        var method = SyntaxGenerator.BoolType.Method("IsLetter", SyntaxGenerator.CharType.Parameter(c.Identifier))
            .Static()
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("IsLetter", code);
    }
    //static bool IsLetter(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
    [Fact]
    public void TakeFive()
    {
        var stringType = SyntaxGenerator.StringType;
        var s = SyntaxFactory.IdentifierName("s");
        //  string { Length: >= 5 } s
        var pattern1 = new PropertyPatternBuilder(stringType, s.Identifier)
            .Add("Length", SyntaxGenerator.GreaterOrEqualPattern(5))
            .Build();
        var pattern2 = stringType.VariablePattern(s.Identifier);

        var collectionType = SyntaxGenerator.Generic(nameof(ICollection), SyntaxGenerator.CharType);
        var symbols = SyntaxFactory.IdentifierName("symbols");
        // ICollection<char> { Count: >= 5 } symbols
        var pattern3 = new PropertyPatternBuilder(collectionType, symbols.Identifier)
            .Add("Count", SyntaxGenerator.GreaterOrEqualPattern(5))
            .Build();
        // ICollection<char> symbols
        var pattern4 = collectionType.VariablePattern(symbols.Identifier);
        var pattern5 = SyntaxGenerator.NullPattern;

        var input = SyntaxFactory.IdentifierName("input");
        var argumentNullExceptionType = SyntaxFactory.IdentifierName(nameof(ArgumentNullException));
        var argumentExceptionType = SyntaxFactory.IdentifierName(nameof(ArgumentException));
        var body = input.SwitchExpression()
            .Case(pattern1, s.Access("Substring").Invocation([SyntaxGenerator.Literal(0), SyntaxGenerator.Literal(5)]))
            .Case(pattern2, s)
            .Case(pattern3, stringType.New([symbols.Access("Take").Invocation([SyntaxGenerator.Literal(5)]).Access("ToArray").Invocation()]))
            .Case(pattern4, stringType.New([symbols.Access("ToArray").Invocation()]))
            .Case(pattern5, argumentNullExceptionType.Throw([SyntaxFactory.IdentifierName("nameof").Invocation([input])]))
            .Default(argumentExceptionType.Throw([SyntaxGenerator.Literal("Not supported input type.")]))
            .Build();
        var method = stringType.Method("TakeFive", SyntaxGenerator.ObjectType.Parameter(input.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("TakeFive", code);
    }
    //string TakeFive(object input) => input switch
    //{
    //    string { Length: >= 5 } s => s.Substring(0, 5),
    //    string s => s,
    //    ICollection<char> { Count: >= 5 } symbols => new string(symbols.Take(5).ToArray()),
    //    ICollection<char> symbols => new string(symbols.ToArray()),
    //    null => throw new ArgumentNullException(nameof(input)),
    //    _ => throw new ArgumentNullException("Not supported input type.") };
    [Fact]
    public void IsAnyEndOnXAxis()
    {
        var pattern1 = new PropertyPatternBuilder(null)
            .Add("Start.Y", SyntaxGenerator.Literal(0))
            .Build();
        var pattern2 = new PropertyPatternBuilder(null)
            .Add("Start.X", SyntaxGenerator.Literal(0))
            .Build();

        var segmentType = SyntaxFactory.IdentifierName("Segment");
        var segment = SyntaxFactory.IdentifierName("segment");
        var body = segment.Is(pattern1.Or(pattern2));
        var method = SyntaxGenerator.BoolType.Method("IsAnyEndOnXAxis", segmentType.Parameter(segment.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("IsAnyEndOnXAxis", code);
    }
    //bool IsAnyEndOnXAxis(Segment segment) => segment is { Start.Y: 0 } or { Start.X: 0 };
    [Fact]
    public void Positional()
    {
        var point = SyntaxFactory.IdentifierName("point");
        var pattern = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.Literal(0))
            .Add(SyntaxGenerator.Literal(0))
            .Build();
        var isOrigin = point.Is(pattern);
        var code = isOrigin.NormalizeWhitespace().ToFullString();
        Assert.Contains("point is (0, 0)", code);
    }
    [Fact]
    public void NamedPositional()
    {
        var point = SyntaxFactory.IdentifierName("point");
        var pattern = new NamedPositionalPatternBuilder(null)
            .Add("X", SyntaxGenerator.Literal(0))
            .Add("Y", SyntaxGenerator.Literal(0))
            .Build();
        var isOrigin = point.Is(pattern);
        var code = isOrigin.NormalizeWhitespace().ToFullString();
        Assert.Contains("point is (X: 0, Y: 0)", code);
    }

    [Fact]
    public void Classify()
    {
        var pattern1 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.Literal(0))
            .Add(SyntaxGenerator.Literal(0))
            .Build();
        var pattern2 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.Literal(1))
            .Add(SyntaxGenerator.Literal(0))
            .Build();
        var pattern3 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.Literal(0))
            .Add(SyntaxGenerator.Literal(1))
            .Build();

        var pointType = SyntaxFactory.IdentifierName("Point");
        var point = SyntaxFactory.IdentifierName("point");
        var body = point.SwitchExpression()
            .Case(pattern1, SyntaxGenerator.Literal("Origin"))
            .Case(pattern2, SyntaxGenerator.Literal("positive X basis end"))
            .Case(pattern3, SyntaxGenerator.Literal("positive Y basis end"))
            .Default(SyntaxGenerator.Literal("Just a point"))
            .Build();
        var method = SyntaxGenerator.StringType.Method("Classify", pointType.Parameter(point.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("Classify", code);
    }
    //string Classify(Point point) => point switch
    //{
    //    (0, 0) => "Origin",
    //    (1, 0) => "positive X basis end",
    //    (0, 1) => "positive Y basis end",
    //    _ => "Just a point"
    //};
    [Fact]
    public void GetGroupTicketPriceDiscount()
    {
        var groupSize = SyntaxFactory.IdentifierName("groupSize");
        var visitDate = SyntaxFactory.IdentifierName("visitDate");
        var dayOfWeekType = SyntaxFactory.IdentifierName("DayOfWeek");
        var pattern1 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.LessOrEqualPattern(0))
            .Add(SyntaxFactory.DiscardPattern())
            .Build();
        var pattern2 = new PositionalPatternBuilder(null)
            .Add(SyntaxFactory.DiscardPattern())
            .Add(dayOfWeekType.Access("Saturday").ToPattern().Or(dayOfWeekType.Access("Sunday").ToPattern()))
            .Build();
        var pattern3 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.GreaterOrEqualPattern(5).And(SyntaxGenerator.LessThanPattern(10)))
            .Add(dayOfWeekType.Access("Monday"))
            .Build();
        var pattern4 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.GreaterOrEqualPattern(10))
            .Add(dayOfWeekType.Access("Monday"))
            .Build();
        var pattern5 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.GreaterOrEqualPattern(5).And(SyntaxGenerator.LessThanPattern(10)))
            .Add(SyntaxFactory.DiscardPattern())
            .Build();
        var pattern6 = new PositionalPatternBuilder(null)
            .Add(SyntaxGenerator.GreaterOrEqualPattern(10))
            .Add(SyntaxFactory.DiscardPattern())
            .Build();

        var argumentExceptionType = SyntaxFactory.IdentifierName("ArgumentException");
        var body = SyntaxGenerator.Tuple(groupSize, visitDate.Access("DayOfWeek"))
            .SwitchExpression()
            .Case(pattern1, argumentExceptionType.Throw([SyntaxGenerator.Literal("Group size must be positive.")]))
            .Case(pattern2, SyntaxGenerator.Literal(0.0m))
            .Case(pattern3, SyntaxGenerator.Literal(20.0m))
            .Case(pattern4, SyntaxGenerator.Literal(30.0m))
            .Case(pattern5, SyntaxGenerator.Literal(12.0m))
            .Case(pattern6, SyntaxGenerator.Literal(15.0m))
            .Default(SyntaxGenerator.Literal(0.0m))
            .Build();
        var method = SyntaxGenerator.DecimalType.Method("GetGroupTicketPriceDiscount",
                SyntaxGenerator.IntType.Parameter(groupSize.Identifier),
                SyntaxGenerator.DateTimeType.Parameter(visitDate.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("GetGroupTicketPriceDiscount", code);
    }
    //decimal GetGroupTicketPriceDiscount(int groupSize, DateTime visitDate) => (groupSize, visitDate.DayOfWeek) switch
    //{
    //    ( <= 0, _) => throw new ArgumentException("Group size must be positive."),
    //    (_, DayOfWeek.Saturday or DayOfWeek.Sunday) => 0.0M,
    //    ( >= 5 and < 10, DayOfWeek.Monday) => 20.0M,
    //    ( >= 10, DayOfWeek.Monday) => 30.0M,
    //    ( >= 5 and < 10, _) => 12.0M,
    //    ( >= 10, _) => 15.0M,
    //    _ => 0.0M
    //};
    [Fact]
    public void PrintIfAllCoordinatesArePositive()
    {
        var point2DType = SyntaxFactory.IdentifierName("Point2D");
        var point3DType = SyntaxFactory.IdentifierName("Point3D");
        var point = SyntaxFactory.IdentifierName("point");
        var p = SyntaxFactory.IdentifierName("p");
        var pattern1 = new PositionalPatternBuilder(point2DType, p.Identifier)
            .Add(SyntaxGenerator.GreaterThanPattern(0))
            .Add(SyntaxGenerator.GreaterThanPattern(0))
            .Build();
        var pattern2 = new PositionalPatternBuilder(point3DType, p.Identifier)
            .Add(SyntaxGenerator.GreaterThanPattern(0))
            .Add(SyntaxGenerator.GreaterThanPattern(0))
            .Add(SyntaxGenerator.GreaterThanPattern(0))
            .Build();
        var body = point.SwitchExpression()
            .Case(pattern1, p.Access("ToString").Invocation())
            .Case(pattern2, p.Access("ToString").Invocation())
            .Default(SyntaxGenerator.StringType.Access("Empty"))
            .Build();
        var method = SyntaxGenerator.StringType.Method("PrintIfAllCoordinatesArePositive",
                SyntaxGenerator.ObjectType.Parameter(point.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("PrintIfAllCoordinatesArePositive", code);
    }
    //string PrintIfAllCoordinatesArePositive(object point) => point switch
    //{
    //    Point2D(> 0, > 0) p => p.ToString(),
    //    Point3D(> 0, > 0, > 0) p => p.ToString(),
    //    _ => string.Empty
    //};
    [Fact]
    public void IsAcceptable()
    {
        var id = SyntaxFactory.IdentifierName("id");
        var absLimit = SyntaxFactory.IdentifierName("absLimit");
        var simulateDataFetchMethod = SyntaxFactory.IdentifierName("SimulateDataFetch");
        var results = SyntaxFactory.IdentifierName("results");

        var body = simulateDataFetchMethod.Invocation([id])
            .Is(SyntaxGenerator.VarPattern(results.Identifier))
            .LogicalAnd(results.Access("Min").Invocation().GreaterOrEqual(absLimit))
            .LogicalAnd(results.Access("Max").Invocation().LessOrEqual(absLimit));
        var method = SyntaxGenerator.BoolType.Method("IsAcceptable",
            SyntaxGenerator.IntType.Parameter(id.Identifier),
            SyntaxGenerator.IntType.Parameter(absLimit.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("IsAcceptable", code);
    }
    // bool IsAcceptable(int id, int absLimit) => SimulateDataFetch(id)is var results && results.Min() >= absLimit && results.Max() <= absLimit;
    [Fact]
    public void Transform()
    {
        var point = SyntaxFactory.IdentifierName("point");
        var pointType = SyntaxFactory.IdentifierName("Point");
        var x = SyntaxFactory.IdentifierName("x");
        var y = SyntaxFactory.IdentifierName("y");
        var pattern = SyntaxGenerator.VarParenthesizedPattern(x.Identifier, y.Identifier);
        var body = point.SwitchExpression()
            .Case(pattern, pointType.New([x.PreMinus(), y]))
                .When(x.LessThan(y))
            .Case(pattern, pointType.New([x, y.PreMinus()]))
                .When(x.GreaterThan(y))
            .Case(pattern, pointType.New([x, y]))
            .Switch
            .Build();
        var method = pointType.Method("Transform", pointType.Parameter(point.Identifier))
            .WithExpressionBody(body);
        var code = method.NormalizeWhitespace().ToFullString();
        Assert.Contains("Transform", code);
    }
    //Point Transform(Point point) => point switch
    //{
    //    var (x, y) when x < y => new Point(-x, y),
    //    var (x, y) when x > y => new Point(x, -y),
    //    var (x, y) => new Point(x, y)};
}
