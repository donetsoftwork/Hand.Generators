# C#.NET源生成器命名规则

## 一、 前言
>* 本文讲解在C#.NET源生成器中生成类的字段、属性、方法及参数和变量标识符的命名处理方式

## 二、 C# 命名规则
### 1. CamelCase(小驼峰)‌
>* 首字母小写,后续单词首字母大写
>* 参数和变量适用
>* 私有字段使用下划线 + 小驼峰

### 2. PascalCase (帕斯卡、大驼峰)‌
>* 首字母大写,后续单词首字母大写
>* 适用属性、方法及公开字段

### 3. 开源项目Hand.Naming用于命名
>* dotnet add package Hand.Naming --version 0.3.2.1-alpha

#### 3.1 CamelWordRule 组件
>* CamelWordRule.FistToLower处理小驼峰
>* 可以把大驼峰转化为小驼峰
>* 可以把下划线 + 小驼峰转化为小驼峰
>* 本身就是小驼峰的不受影响
>* 适用把属性名、字段名转化为参数名或变量名

~~~csharp
[Theory]
[InlineData("Test", "test")]
[InlineData("_id", "id")]
[InlineData("name", "name")]
public void FistToLower(string original, string expected)
{
    var result = CamelWordRule.FistToLower(original!);
    Assert.Equal(expected, result);
}
~~~

#### 3.2 UnderWordRule 组件
>* UnderWordRule.UnderLower处理下划线 + 小驼峰
>* 可以把大、小驼峰转化为下划线 + 小驼峰
>* 本身就是下划线 + 小驼峰的不受影响
>* 适用把参数名、属性名及公开字段名转化为私有字段名

~~~csharp
[Theory]
[InlineData("Test", "_test")]
[InlineData("test", "_test")]
[InlineData("_test", "_test")]
[InlineData("_Test", "_test")]
public void UnderLower(string original, string expected)
{
    var result = UnderWordRule.UnderLower(original!);
    Assert.Equal(expected, result);
}
~~~

#### 3.3 PascalWordRule 组件
>* PascalWordRule.FistToUpper处理大驼峰
>* 可以下划线 + 小驼峰及小驼峰转化转化为大驼峰
>* 本身就是大驼峰的不受影响
>* 适用把参数名和私有字段名转化为属性和公开字段名

~~~csharp
[Theory]
[InlineData("test", "Test")]
[InlineData("_id", "Id")]
[InlineData("Name", "Name")]
public void FistToUpper(string original, string expected)
{
    var result = PascalWordRule.FistToUpper(original!);
    Assert.Equal(expected, result);
}
~~~

## 三、 开源项目Hand.Generators.Naming处理成员名冲突
>* dotnet add package Hand.Generators.Naming --version 0.3.0-alpha

### 1. 按User生成UserDto的Case
>* 其中 UserDto 中已经存在Id属性了
>* 通过 Hand.Naming.KindContainer 组件处理命名冲突
>* 通过 Contains 方法判断是否含同名成员
>* 通过 AddKind 方法添加新属性

~~~csharp
var source = @"
    public partial class UserDto
    {
        public string? Id { get; set; }
    }
    public record User(int Id, string Name);
    ";
var driver = SyntaxTreeDriver.CreateDefaultDriver();
SyntaxTree syntaxTree = driver.Parse(source);
var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
Assert.NotNull(destType);
var compilation = driver.Compile(syntaxTree);
var sourceSymbol = compilation.GetTypeByMetadataName("User");
Assert.NotNull(sourceSymbol);
var destSymbol = compilation.GetTypeByMetadataName("UserDto");
Assert.NotNull(destSymbol);
var nameProvider = KindContainer.Create(destSymbol);
var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();

var generator = SyntaxGenerator.Clone(destType);
foreach (var sourceProperty in sourceProperties)
{
    var propertyName = sourceProperty.Name;
    if (nameProvider.Contains(propertyName))
        continue;
    var property = sourceProperty.Type.ToSyntax().GetSetProperty(propertyName).Public();
    generator.AddProperty(property);
    nameProvider.AddKind(propertyName, MemberKind.PropertyDeclaration);
}
var code = generator.Build().NormalizeWhitespace().ToFullString();
Assert.Contains("Name", code);
Assert.DoesNotContain("Id", code);
~~~

### 2. UserDto生成代码如下
~~~csharp
partial class UserDto
{
    public string Name { get; set; }
}
~~~

### 3. KindContainer 主要方法
~~~csharp
class KindContainer
{
    bool Contains(string name);
    bool TryGetMemberKind(string name, out MemberKind kind);
    void AddKind(string name, MemberKind kind);
}
~~~

## 四、 成员不能与类同名
>* 除构造函数外,成员不能与当前类同名

### 1. 使用 TypedProvider 组件处理类名冲突
>* TypedProvider 可以实现与 KindContainer 类似的功能
>* TypedProvider 会过滤当前类名
>* 扩展方法 TryDeclareField,封装了 Contains 和 AddKind
>* TryDeclareProperty 也是扩展方法,用于属性的尝试添加

~~~csharp
var source = @"
    public partial class B;
    public record A(string Name, string B);
    ";
var driver = SyntaxTreeDriver.CreateDefaultDriver();
SyntaxTree syntaxTree = driver.Parse(source);
var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
Assert.NotNull(destType);
var compilation = driver.Compile(syntaxTree);
var sourceSymbol = compilation.GetTypeByMetadataName("A");
Assert.NotNull(sourceSymbol);
var destSymbol = compilation.GetTypeByMetadataName("B");
Assert.NotNull(destSymbol);
var nameProvider = TypedProvider.Create(destSymbol);
var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();

var generator = SyntaxGenerator.Clone(destType);
foreach (var sourceProperty in sourceProperties)
{
    var propertyName = sourceProperty.Name;
    if (nameProvider.TryDeclareField(propertyName))
    {
        var field = sourceProperty.Type.ToSyntax()
            .Field(propertyName, SyntaxGenerator.DefaultLiteral.SuppressNull())
            .Public();
        generator.AddField(field);
    }
}
var code = generator.Build().NormalizeWhitespace().ToFullString();
Assert.Contains("Name", code);
~~~

### 2. 示例1生成如下代码
>* 由于类A的属性B与类B重名被 TypedProvider 过滤掉了
>* 如果示例2.1换成 KindContainer 生成的代码可能就编译不过了(CS0542)

~~~csharp
partial class B
{
    public string Name = default!;
}
~~~

## 五、方法支持重载
### 1. MethodContainer 组件支持处理重载
>* 通过 TryDeclareMethod 尝试添加方法签名
>* 虽然已经存在Add方法, 但新添加的Add方法参数类型不同,可以添加成功
>* MethodContainer 通过匹配参数个数、类型及顺序实现重载逻辑判断

~~~csharp
var source = @"
    public partial class Calculator
    {
        public static int Add(int a, int b) => a + b;
    }
    ";
var driver = SyntaxTreeDriver.CreateDefaultDriver();
SyntaxTree syntaxTree = driver.Parse(source);
var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
Assert.NotNull(destType);
var compilation = driver.Compile(syntaxTree);
var destSymbol = compilation.GetTypeByMetadataName("Calculator");
Assert.NotNull(destSymbol);
var nameContainer = MethodContainer.Create(destSymbol);
var generator = SyntaxGenerator.Clone(destType);
var methodName = "Add";
Assert.True(nameContainer.Contains(methodName));
var methodSymbol = compilation.GetDecimalSymbol();
INamedTypeSymbol[] parameterSymbols = [methodSymbol, methodSymbol];
if (nameContainer.TryDeclareMethod(methodName, parameterSymbols))
{
    var methodType = methodSymbol.ToSyntax();
    var a = SyntaxFactory.IdentifierName("a");
    var b = SyntaxFactory.IdentifierName("b");
    var method = methodType.Method(methodName, [methodType.Parameter(a.Identifier), methodType.Parameter(b.Identifier)])
        .ToBuilder()
        .Return(a.Add(b))
        .Public()
        .Static();
    generator.AddMethod(method);
}
var code = generator.Build().NormalizeWhitespace().ToFullString();
Assert.Contains(methodName, code);
~~~

### 2. 示例1生成如下代码
~~~csharp
partial class Calculator
{
    public static decimal Add(decimal a, decimal b)
    {
        return a + b;
    }
}
~~~

## 六、 综合应用
>* 如何既支持字段、属性又支持方法重载
>* 这就是 CompositeProvider 组件应用的场景

### 1. 使用 CompositeProvider 的 Case
~~~csharp
var source = @"
    public partial class B
    {
        public string? Id { get; set;}
        public void Deconstruct(out string? id)
        {
            id = Id;
        }
    }
    public record A(int? Id, string Name, string B);
    ";
var driver = SyntaxTreeDriver.CreateDefaultDriver();
SyntaxTree syntaxTree = driver.Parse(source);
var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
Assert.NotNull(destType);
var compilation = driver.Compile(syntaxTree);
var sourceSymbol = compilation.GetTypeByMetadataName("A");
Assert.NotNull(sourceSymbol);
var destSymbol = compilation.GetTypeByMetadataName("B");
Assert.NotNull(destSymbol);
var nameProvider = CompositeProvider.Create(destSymbol);
var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();
var sourceCount = sourceProperties.Length;
var generator = SyntaxGenerator.Clone(destType);

var parameterSymbol0 = compilation.GetNullable(compilation.GetStringSymbol());
var parameterSymbols = new List<ITypeSymbol>(sourceCount) { parameterSymbol0 };
var method = SyntaxGenerator.VoidType.Method("Deconstruct")
    .ToBuilder()
    .AddParameter(parameterSymbol0.ToSyntax().Parameter("id").Out())
    .AddExpression(SyntaxFactory.IdentifierName("id").Assign(SyntaxFactory.IdentifierName("Id")));
foreach (var sourceProperty in sourceProperties)
{
    var propertyName = sourceProperty.Name;
    if (nameProvider.TryDeclareProperty(propertyName))
    {
        var propertySymbol = sourceProperty.Type;
        parameterSymbols.Add(propertySymbol);
        var property = propertySymbol.ToSyntax()
            .GetSetProperty(propertyName)
            .Public();
        generator.AddProperty(property);

        var parameter = SyntaxFactory.IdentifierName(CamelWordRule.FistToLower(propertyName));
        method.AddParameter(propertySymbol.ToSyntax().Parameter(parameter.Identifier).Out())
            .AddExpression(parameter.Assign(SyntaxFactory.IdentifierName(propertyName)));
    }
}
if (nameProvider.TryDeclareMethod("Deconstruct", [.. parameterSymbols]))
    generator.AddMethod(method.End().Public());
var code = generator.Build().NormalizeWhitespace().ToFullString();
Assert.Contains("Name", code);
Assert.Contains("Deconstruct", code);
~~~

### 2. 示例1生成如下代码
~~~csharp
partial class B
{
    public string Name { get; set; }

    public void Deconstruct(out string? id, out string name)
    {
        id = Id;
        name = Name;
    }
}
~~~

## 七、处理基类成员冲突
>* 可以添加 new 来解决与基类成员重名的问题
>* InheritProvider 组件可以处理这种问题

### 1. 使用 InheritProvider 的 Case
>* 通过方法 TypedProvider.Inherit 构造 InheritProvider
>* 通过 TryDeclareField 的 isNew 输出参数判断是否与基类重名
>* 如果重名增加 new 修饰符

~~~csharp
var source = @"
    public record Column(string Name);
    public abstract class Table(string name)
    {
        public string Name { get; } = name;
        public abstract Column[] Columns { get; }
    }
    public record User(int Id, string Name);
    public partial class UserTable() : Table(""Users"");
";
var driver = SyntaxTreeDriver.CreateDefaultDriver();
SyntaxTree syntaxTree = driver.Parse(source);
var destType = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().LastOrDefault();
Assert.NotNull(destType);
var compilation = driver.Compile(syntaxTree);
var sourceSymbol = compilation.GetTypeByMetadataName("User");
Assert.NotNull(sourceSymbol);
var destSymbol = compilation.GetTypeByMetadataName("UserTable");
Assert.NotNull(destSymbol);
var columnType = SyntaxFactory.IdentifierName("Column");
InheritProvider nameProvider = TypedProvider.Inherit(destSymbol);
var sourceProperties = SymbolReflection.GetPublicPropertiesWithBase(sourceSymbol).ToArray();

var generator = SyntaxGenerator.Clone(destType);
var columnList = new List<ExpressionSyntax>();
foreach (var sourceProperty in sourceProperties)
{
    var columnName = sourceProperty.Name;
    if (nameProvider.TryDeclareField(columnName, out var isNew))
    {
        var column = SyntaxFactory.IdentifierName(columnName);
        var field = columnType
            .Field(column.Identifier, SyntaxGenerator.New([SyntaxGenerator.Literal(columnName)]))
            .Public();
        if (isNew)
            field = field.New();
        generator.AddField(field);
        columnList.Add(column);
    }
}
var columns = columnType.Array()
    .Property("Columns", SyntaxGenerator.Collection([.. columnList]))
    .Public()
    .Override();
generator.AddProperty(columns);

var code = generator.Build().NormalizeWhitespace().ToFullString();
Assert.Contains("new Column Name", code);
~~~

### 2. 示例1生成如下代码
~~~csharp
partial class UserTable
{
    public Column Id = new("Id");
    public new Column Name = new("Name");
    public override Column[] Columns => [Id, Name];
}
~~~

