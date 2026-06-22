# Roslyn语法的模式匹配之EasySyntax增加模式匹配支持
>* 使用Roslyn语法生成包含模式匹配的代码

## 一、先看一个模式匹配的Case
>* 该Case是一个使用模式匹配检测回文算法,只有一行代码
>* 这是不是你见过最简洁的检测回文代码
>* 但是这里面用到了不少模式匹配
>* 有逻辑模式、列表模式、var模式和切片模式
>* 模式匹配不仅仅是语法糖,在.net中有很高的地位
>* 所以SourceGenerator非常有必要支持生成模式匹配的代码
>* 为此开源项目EasySyntax全面增加模式匹配的支持

~~~csharp
public static bool IsPalindrome(ReadOnlySpan<char> input)
    => input is not [var first, ..var middle, var last] || (first == last && IsPalindrome(middle));
~~~

## 二、 模式匹配的主要类型
### 1. 类型模式
>* 检查表达式的运行时类型

### 2. 声明模式
>* 检查表达式的运行时类型，如果匹配成功，请将表达式结果分配给声明的变量

### 3. 常量模式
>* 测试表达式结果是否等于指定的常量

### 4. 关系模式
>* 将表达式结果与指定的常量进行比较

### 5. var模式
>* 匹配任何表达式并将其结果分配给声明的变量

### 6. 丢弃模式
>* 匹配任何表达式

### 7. 逻辑模式
>* 测试表达式是否与模式的逻辑组合匹配
>* 通过not、or、and及括号进行组合

### 8. 带括号模式
>* 可在任何模式两边加上括号
>* 用来强调或更改 逻辑模式中的优先级

### 9. 列表模式
>* 测试元素序列是否与相应的嵌套模式匹配
>* 用于数组和集合类型

### 10. 切片模式
>* 切片模式只能显示在列表模式中,不能单独使用
>* 切片模匹配一个子列表
>* 切片模式中可以嵌套子模式

### 11. 属性模式
>* 测试表达式的属性或字段是否与嵌套模式匹配

### 12. 位置模式
>* 解构表达式结果并测试结果是否与嵌套模式匹配
>* 用于元组、record类型或其他定义了Deconstruct的类型

其中逻辑模式、列表模式、属性模式和位置模式是可嵌套其他模式的复杂模式

## 三、 在 C# 中以下场景支持模式匹配：
>* is 表达式
>* switch 语句
>* switch 表达式

## 四、简单的模式匹配
### 1. 类型模式
#### 1.1 类型模式的Case
~~~csharp
if(fruit is Apple)
	return "I like Apple!";
~~~

#### 1.2 EasySyntax语法实现
>* 使用IsType扩展方法

~~~csharp
var fruit = SyntaxFactory.IdentifierName("fruit");
var appleType = SyntaxFactory.IdentifierName("Apple");
var statement = fruit.IsType(appleType)
    .If()
        .Add(SyntaxGenerator.Literal("I like Apple!").Return())
    .Build();
~~~

#### 1.3 Roslyn原始语法
~~~csharp
var fruit = SyntaxFactory.IdentifierName("fruit");
var appleType = SyntaxFactory.IdentifierName("Apple");
var statement = SyntaxFactory.IfStatement(
    SyntaxFactory.IsPatternExpression(fruit, 
        SyntaxFactory.TypePattern(appleType)), 
    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, 
        SyntaxFactory.Literal("I like Apple!"))));
~~~

#### 1.4 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#declaration-and-type-patterns

### 2. 声明模式
#### 2.1 声明模式的Case
~~~csharp
if (fruit is Apple apple)
    MakeApplePie(apple);
~~~

#### 2.2 EasySyntax 语法实现
>* 使用VariablePattern扩展方法

~~~csharp
var fruit = SyntaxFactory.IdentifierName("fruit");
var appleType = SyntaxFactory.IdentifierName("Apple");
var apple = SyntaxFactory.IdentifierName("apple");
var makeApplePieMethod = SyntaxFactory.IdentifierName("MakeApplePie");
var statement = fruit.Is(appleType.VariablePattern(apple.Identifier))
    .If()
        .AddPatter(makeApplePieMethod.Invocation([apple]))
    .Build();
~~~

#### 2.3 Roslyn原始语法
~~~csharp
var fruit = SyntaxFactory.IdentifierName("fruit");
var appleType = SyntaxFactory.IdentifierName("Apple");
var apple = SyntaxFactory.IdentifierName("apple");
var makeApplePieMethod = SyntaxFactory.IdentifierName("MakeApplePie");
var statement = SyntaxFactory.IfStatement(
    SyntaxFactory.IsPatternExpression(fruit, 
        SyntaxFactory.DeclarationPattern(appleType, SyntaxFactory.SingleVariableDesignation(apple.Identifier))),
    SyntaxFactory.ExpressionStatement(
        SyntaxFactory.InvocationExpression(makeApplePieMethod, 
            SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(apple))))));
~~~

#### 2.4 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#declaration-and-type-patterns

### 3. 常量模式
#### 3.1 声明模式的Case
~~~csharp
obj is null
~~~

#### 3.2 EasySyntax 语法实现
>* 使用ToPattern扩展方法把表达式转化为常量模式

~~~csharp
var obj = SyntaxFactory.IdentifierName("obj");
var expression = obj.Is(SyntaxGenerator.NullLiteral.ToPattern();
~~~

#### 3.3 Roslyn原始语法
~~~csharp
var obj = SyntaxFactory.IdentifierName("obj");
var expression = SyntaxFactory.IsPatternExpression(obj, 
    SyntaxFactory.ConstantPattern(
        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)));
~~~

#### 3.4 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#constant-pattern

### 4. 关系模式
#### 4.1 声明模式的Case
~~~csharp
!= 3
~~~

#### 4.2 EasySyntax 语法实现
>* GreaterThanPattern扩展方法
>* GreaterOrEqualPattern扩展方法
>* LessThanPattern扩展方法
>* LessOrEqualPattern扩展方法
>* EqualPattern扩展方法
>* NotEqualPattern扩展方法

~~~csharp
 var pattern = SyntaxGenerator.NotEqualPattern(3);
~~~

#### 4.3 Roslyn原始语法
~~~csharp
var pattern = SyntaxFactory.RelationalPattern(
    SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken), 
    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
        SyntaxFactory.Literal(3)));
~~~

#### 4.4 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#relational-patterns

### 5. var模式
#### 5.1 var模式的Case
~~~csharp
GetScores(id) is var scores && scores.Average() >= 60
~~~

#### 5.2 EasySyntax 语法实现
>* GreaterThanPattern扩展方法
>* GreaterOrEqualPattern扩展方法
>* LessThanPattern扩展方法
>* LessOrEqualPattern扩展方法
>* EqualPattern扩展方法
>* NotEqualPattern扩展方法

~~~csharp
var getScoresMethod = SyntaxFactory.IdentifierName("GetScores");
var scores = SyntaxFactory.IdentifierName("scores");
var id = SyntaxFactory.IdentifierName("id");
var expression = getScoresMethod.Invocation([id])
    .Is(SyntaxGenerator.VarPattern(scores.Identifier))
    .LogicalAnd(scores.Access("Average").Invocation().GreaterOrEqual(SyntaxGenerator.Literal(60)));
~~~

### 5.3 Roslyn原始语法
~~~csharp
var getScoresMethod = SyntaxFactory.IdentifierName("GetScores");
var scores = SyntaxFactory.IdentifierName("scores");
var id = SyntaxFactory.IdentifierName("id");
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
~~~

#### 5.4 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#var-pattern

### 6. 丢弃模式
>* 丢弃模式常用于switch表达式
>* 也可用于列表和位置模式

#### 6.1 声明模式的Case
~~~csharp
date.Day switch
{
    15 => "今夜是月圆之夜",
    _ => "月圆要等到十五"
}
~~~

#### 6.2 Roslyn原始语法
>* 使用SyntaxFactory.DiscardPattern()声明丢弃模式
>* 以下case结合了EasySyntax的SwitchExpression语法

~~~csharp
var date = SyntaxFactory.IdentifierName("date");
var expression = date.Access("Day")
    .SwitchExpression()
        .Case(SyntaxGenerator.Literal(15), SyntaxGenerator.Literal("今夜是月圆之夜"))
        .Case(SyntaxFactory.DiscardPattern(), SyntaxGenerator.Literal("月圆要等到十五"))
    .Switch.Build();
~~~

#### 6.3 EasySyntax语法可以进一步简化
>* SwitchExpression中的Default封装了SyntaxFactory.DiscardPattern()
>* Default是最后一个分支,可以安全的直接Build

~~~csharp
var date = SyntaxFactory.IdentifierName("date");
var expression = date.Access("Day")
    .SwitchExpression()
        .Case(SyntaxGenerator.Literal(15), SyntaxGenerator.Literal("今夜是月圆之夜"))
        .Default(SyntaxGenerator.Literal("月圆要等到十五"))
    .Build();
~~~

#### 6.4 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#discard-pattern

## 五、逻辑模式
>* 逻辑模式嵌套了其他模式
>* 逻辑模式通过and、or和not及括号组装其他模式

### 1. 逻辑模式的Case
>* 该Case通过and组装了两个关系模式

~~~csharp
0 and < 10
~~~

### 2. EasySyntax 语法实现
>* And扩展方法
>* Or扩展方法
>* Not扩展方法

~~~csharp
var pattern = SyntaxGenerator.GreaterThanPattern(0)
    .And(SyntaxGenerator.LessThanPattern(10));
~~~

### 3. Roslyn原始语法
~~~csharp
var pattern = SyntaxFactory.BinaryPattern(SyntaxKind.AndPattern,
    SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.GreaterThanToken),
        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(0))),
    SyntaxFactory.Token(SyntaxKind.AndKeyword),
    SyntaxFactory.RelationalPattern(SyntaxFactory.Token(SyntaxKind.LessThanToken),
        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
            SyntaxFactory.Literal(10))));
~~~

### 4. 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#logical-patterns

## 六、带括号模式
>* 可在任何模式两边加上括号
>* 用来强调或更改逻辑模式中的优先级

### 1 逻辑模式的Case
>* 该Case通过and组装了两个关系模式

~~~csharp
input is not (float or double)
~~~

### 2 EasySyntax 语法实现
>* 使用Parenthesized扩展方法转化为带括号模式

~~~csharp
var input = SyntaxFactory.IdentifierName("input");
var pattern = SyntaxFactory.TypePattern(SyntaxGenerator.FloatType)
    .Or(SyntaxFactory.TypePattern(SyntaxGenerator.DoubleType))
    .Parenthesized()
    .Not();
var expression = input.Is(pattern);
~~~

### 3 Roslyn原始语法
~~~csharp
var input = SyntaxFactory.IdentifierName("input");
var pattern = SyntaxFactory.UnaryPattern(
    SyntaxFactory.ParenthesizedPattern(
        SyntaxFactory.BinaryPattern(
            SyntaxKind.OrPattern, 
            SyntaxFactory.TypePattern(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword))), 
            SyntaxFactory.Token(SyntaxKind.OrKeyword), 
            SyntaxFactory.TypePattern(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword))))));
var expression = SyntaxFactory.IsPatternExpression(input, pattern);
~~~

### 4. 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#parenthesized-pattern

## 七、列表模式
>* 测试元素序列是否与相应的嵌套模式匹配
>* 用于字符串、数组和集合类型

### 1.列表模式的Case
>* 该Case由3个模式组成
>* 前两个是常量模式
>* 第三个是丢弃模式

~~~csharp
name is ['曾', '国', _ ]
~~~

### 2.EasySyntax语法实现
>* 使用ListPatternBuilder组装多个模式

~~~csharp
var name = SyntaxFactory.IdentifierName("name");
var pattern = new ListPatternBuilder()
    .Add(SyntaxGenerator.Literal('曾'))
    .Add(SyntaxGenerator.Literal('国'))
    .Add(SyntaxFactory.DiscardPattern())
    .Build();
var expression = name.Is(pattern);
~~~

### 3.Roslyn原始语法
~~~csharp
var name = SyntaxFactory.IdentifierName("name");
var pattern = SyntaxFactory.ListPattern(SyntaxFactory.SeparatedList<PatternSyntax>([
    SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, 
        SyntaxFactory.Literal('曾'))),
    SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, 
        SyntaxFactory.Literal('国'))),
    SyntaxFactory.DiscardPattern()]));
var expression = name.Is(pattern);
~~~

### 4. 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#list-patterns

## 八、切片模式
>* 切片模式只能显示在列表模式中,不能单独使用
>* 切片模匹配一个子列表
>* 切片模式中可以嵌套子模式

### 1.切片模式的Case
>* 其中.. var middle就是切片模式
>* 该Case定义了一个使用切片模式和列表模式实现的回文算法
>* 通过模式提取首字母、尾字母和中间切片
>* 如果首尾字母相同再用中间切片递归调用该算法
>* 非常简洁高效

~~~csharp
static bool IsPalindrome(ReadOnlySpan<char> input)
    => input is not [var first, .. var middle, var last] || (first == last && IsPalindrome(middle));
~~~

### 2.EasySyntax语法实现
>* 使用Slice扩展方法定义切片模式
>* 用于把原模式转化为切片模式

~~~csharp
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
~~~

### 3.Roslyn原始语法实现切片模式
>* 以下代码等效EasySyntax语法的SyntaxGenerator.VarPattern(middle.Identifier).Slice()

~~~csharp
SyntaxFactory.SlicePattern(SyntaxGenerator.VarPattern(middle.Identifier));
~~~

### 4. 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#list-patterns

## 九、属性模式
>* 由属性名(或字段名)及其模式组成
>* 可以同时定义多个属性(或字段)

### 1.逻辑模式的Case
>* 该Case包含了两个属性
>* Month使用常量模式
>* Day使用关系模式

~~~csharp
{ Month: 10, Day: <= 7 }
~~~

### 2.EasySyntax语法实现
>* 使用PropertyPatternBuilder组装多个属性(或字段)

~~~csharp
var nationalDays = new PropertyPatternBuilder(null)
    .Add("Month", SyntaxGenerator.Literal(10))
    .Add("Day", SyntaxGenerator.LessOrEqualPattern(7))
    .Build();
~~~

### 3.Roslyn原始语法
~~~csharp
var nationalDays = SyntaxFactory.RecursivePattern(
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
~~~

### 4. 属性模式可选参数
>* 可以定义一个可选的类型参数,相当于类型模式
>* 还可以定义一个可选的变量名,相当于声明模式

#### 4.1 EasySyntax的Case
>* 该case的类型参数为Food
>* 该case的变量名为food

~~~csharp
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
~~~

#### 4.2 该Case生成以下代码
>* 该Case描述一个查找食物的场景
>* 先确认该东西是否为食物
>* 再确认是否在保质期内

~~~csharp
Food? FindFoot(object thing)
{
    if (thing is Food { ExpirationDate: > DateTime.Now } food)
        return food;
    return null;
}
~~~

### 5. 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#property-pattern

## 十、位置模式
>* 解构表达式结果并测试结果是否与嵌套模式匹配
>* 用于元组、record类型或其他定义了Deconstruct的类型
>* 位置模式功能很强大也比较复杂

### 1.位置模式的简单Case
~~~csharp
point is (0, 0)
~~~

### 2.EasySyntax语法实现
>* 使用PropertyPatternBuilder组装多个属性(或字段)

~~~csharp
var point = SyntaxFactory.IdentifierName("point");
var pattern = new PositionalPatternBuilder(null)
    .Add(SyntaxGenerator.Literal(0))
    .Add(SyntaxGenerator.Literal(0))
    .Build();
var isOrigin = point.Is(pattern);
~~~

### 3.Roslyn原始语法
~~~csharp
var nationalDays = SyntaxFactory.RecursivePattern(
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
~~~

### 4. 位置模式可选参数
>* 可以定义一个可选的type(类型参数),相当于类型模式
>* 还可以定义一个可选的name(变量名),相当于声明模式

#### 4.1 EasySyntax的Case
>* 该case的类型参数为Point2D和Point3D
>* 该case的变量名为p

~~~csharp
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
~~~

#### 4.2 该Case生成以下代码
>* 如果是二维的点且每个坐标值都大于0
>* 或者是三维的点且每个坐标值都大于0
>* 否则返回空

~~~csharp
string PrintIfAllCoordinatesArePositive(object point) => point switch
{
    Point2D(> 0, > 0) p => p.ToString(),
    Point3D(> 0, > 0, > 0) p => p.ToString(),
    _ => string.Empty
};
~~~

### 5. 位置模式支持属性名
>* 增加属性名能提高代码的可读性

#### 5.1 EasySyntax的Case
>* 使用NamedPositionalPatternBuilder来支持属性名
>* NamedPositionalPatternBuilder也支持type和name可选参数
>* 使用NamedPositionalPatternBuilder也可以轻松重写4.的Case,限与篇幅就不举例了

~~~csharp
var point = SyntaxFactory.IdentifierName("point");
var pattern = new NamedPositionalPatternBuilder(null)
    .Add("X", SyntaxGenerator.Literal(0))
    .Add("Y", SyntaxGenerator.Literal(0))
    .Build();
var isOrigin = point.Is(pattern);
~~~

#### 5.2 该Case生成以下代码
>* 增加属性名X和Y使得代码可读性明显的提高

~~~csharp
point is (X: 0, Y: 0)
~~~

### 6. 位置模式还可以附加属性模式
>* 不是嵌套,是属性模式作为位置模式的可选参数

#### 6.1 例如以下代码
>* WeightedPoint是结构体,另外还有一个属性Weight

~~~csharp
public record WeightedPoint(int X, int Y)
{
    public int Weight { get; set; }
}
~~~

#### 6.2 EasySyntax的Case
>* 使用RecursivePatternBuilder来定义含属性模式的位置模式
>* RecursivePatternBuilder也支持type和name可选参数
>* NamedRecursivePatternBuilder能实现位置参数属性名表示及属性模式,限与篇幅本文不展开

~~~csharp
var point = SyntaxFactory.IdentifierName("point");
var builder = new RecursivePatternBuilder(null);
builder.Positional.Add(SyntaxGenerator.GreaterOrEqualPattern(0))
    .Add(SyntaxGenerator.GreaterOrEqualPattern(0));
builder.Property.Add("Weight", SyntaxGenerator.GreaterThanPattern(0));
var isInDomain = point.Is(builder.Build());
~~~

#### 6.3 该Case生成以下代码
~~~csharp
point is (>= 0, >= 0) { Weight: > 0 }
~~~

### 7. 参看官方阅读
>* https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/patterns#positional-pattern


源码托管地址: https://github.com/donetsoftwork/Hand.Generators ，欢迎大家直接查看源码。
gitee同步更新:https://gitee.com/donetsoftwork/hand.-generators

如果大家喜欢请动动您发财的小手手帮忙点一下Star,谢谢！！！
