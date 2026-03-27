# SourceGenerator之partial范式及测试
>* SourceGenerator往往和partial配合使用能实现更好效果

## 一、什么是partial范式
>* partial关键字允许将一个类或方法分散到多个文件中
>* 所以partial是代码生成的一个很好的抓手
>* 再配合Attribute特性,可以更准确定位需要生成代码的类或方法
>* 可以理解为静态的“依赖注入”
>* 对代码按规则自动补足,减少重复代码编写及其可能导致的失误
>* 还可以代替反射提高性能,AOT友好
>* 笔者称之为SourceGenerator的partial范式
>* 开源项目GenerateCore用于践行partial范式

## 二、Framework中有大量partial范式的应用
>* 正则、序列化、日志等SourceGenerator都是和partial配合使用的

### 1. 正则表达式的Case
~~~csharp
partial class Program
{

    [GeneratedRegex("abc|def", RegexOptions.IgnoreCase)]
    private static partial Regex AbcOrDefGeneratedRegex();
}
~~~

### 2. 正则表达式的生成结果
~~~csharp
partial class Program
{
    /// <remarks>
    /// Pattern:<br/>
    /// <code>abc|def</code><br/>
    /// Options:<br/>
    /// <code>RegexOptions.IgnoreCase</code><br/>
    /// Explanation:<br/>
    /// <code>
    /// ○ Match with 2 alternative expressions, atomically.<br/>
    ///     ○ Match a sequence of expressions.<br/>
    ///         ○ Match a character in the set [Aa].<br/>
    ///         ○ Match a character in the set [Bb].<br/>
    ///         ○ Match a character in the set [Cc].<br/>
    ///     ○ Match a sequence of expressions.<br/>
    ///         ○ Match a character in the set [Dd].<br/>
    ///         ○ Match a character in the set [Ee].<br/>
    ///         ○ Match a character in the set [Ff].<br/>
    /// </code>
    /// </remarks>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Text.RegularExpressions.Generator", "8.0.14.7010")]
    private static partial global::System.Text.RegularExpressions.Regex AbcOrDefGeneratedRegex()
        => global::System.Text.RegularExpressions.Generated.AbcOrDefGeneratedRegex_0.Instance;
    // ...
}
~~~

## 三、partial范式的要素
### 1. Attribute
>* 通过Attribute特性来标记需要代码补足的位置
>* Attribute的命名最好与调用的SourceGenerator一致
>* 需要生成的类有相应的Attribute也可以增加可读性(有预期该类包含自动生成的代码)
>* 还可以通过Attribute给SourceGenerator传参数,SourceGenerator按参数调整生成代码
>* partial范式通过官方方法SyntaxValueProvider.ForAttributeWithMetadataName来标记定位

### 2. Filter
>* ISyntaxFilter是节点过滤接口
>* SyntaxFilter是默认实现,实现按节点类型和是否为partial来过滤

~~~csharp
interface ISyntaxFilter
{
    bool Match(SyntaxNode node, CancellationToken cancellation);
}
class SyntaxFilter(bool isPartial, params SyntaxKind[] kinds)
    : ISyntaxFilter;
~~~

### 3. GeneratorSource
>* GeneratorSource是转化源,是Transform的预处理结果
>* IGeneratorSource是转化源接口
>* GenerateFileName是生成文件名属性
>* Generate是生成代码方法
>* SyntaxGenerator是通过SyntaxTree生成代码的辅助类,是EasySyntax项目的功能
>* [查看介绍EasySyntax的文章](https://www.cnblogs.com/xiangji/p/19688804)

~~~csharp
interface IGeneratorSource
{
    string GenerateFileName { get; }
    SyntaxGenerator Generate();
}
~~~

### 4. Transform
>* Transform是对节点预处理
>* 把GeneratorAttributeSyntaxContext转化为需要的GeneratorSource类型
>* GeneratorAttributeSyntaxContext包含成员节点、节点符号、节点特性列表和SemanticModel
>* 如果不满足生成必要条件返回null会被自动过滤
>* ISyntaxTransform是转化接口
>* PassTransform是默认实现,直接返回官方对象
>* TSource一般实现接口IGeneratorSource

~~~csharp
interface IGeneratorTransform<TSource>
{
    TSource? Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellation);
}
class PassTransform : IGeneratorTransform<GeneratorAttributeSyntaxContext>
{
    public GeneratorAttributeSyntaxContext Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellation)
        => context;
}
~~~

### 5. Executor
>* IGeneratorExecutor是执行器接口
>* GeneratorExecutor是默认实现,一般可以直接使用

~~~csharp
interface IGeneratorExecutor<TSource>
{
    void Execute(SourceProductionContext context, TSource source);
}
class GeneratorExecutor<TSource> : IGeneratorExecutor<TSource>
    where TSource : IGeneratorSource
{
    public virtual void Execute(SourceProductionContext context, TSource source)
    {
        var cancellation = context.CancellationToken;
        if (cancellation.IsCancellationRequested)
            return;
        var builder = source.Generate();
        var code = builder.Build()
            .WithGenerated()
            .ToFullString();
        context.AddSource(source.GenerateFileName, code);
    }
}
~~~

### 6. 生成器基类ValuesGenerator
>* 通过ValuesGenerator简化代码生成器开发
>* 把业务逻辑都提取到TSource中
>* filter、transform和executor都会很简单

~~~csharp
class ValuesGenerator<TSource>(
    string attributeName, 
    ISyntaxFilter filter, 
    ISyntaxTransform<TSource> transform, 
    ISyntaxExecutor<TSource> executor);
~~~

## 四、通过ValuesGenerator实现代码生成器的Case
>* 定义类型HelloGenerator继承ValuesGenerator
>* 另外需要实现HelloGeneratorAttribute、HelloTransform和HelloSource

### 1. HelloGenerator代码非常简单
>* 含义是查找HelloGenerator标记
>* 查找含partial修饰的类
>* 使用HelloTransform转化HelloSource
>* 执行HelloSource生成代码

~~~csharp
class HelloGenerator()
    : ValuesGenerator<HelloSource>(
    "GenerateCoreTests.Hello.HelloGeneratorAttribute",
    new SyntaxFilter(true, SyntaxKind.ClassDeclaration),
    new HelloTransform(),
    new GeneratorExecutor<HelloSource>())
{
}
~~~

### 2. HelloGeneratorAttribute非常简单
~~~csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
class HelloGeneratorAttribute : Attribute
{
}
~~~

### 3. HelloTransform非常简单
~~~csharp
class HelloTransform : IGeneratorTransform<HelloSource>
{
    public HelloSource? Transform(GeneratorAttributeSyntaxContext context, CancellationToken cancellation)
    {
        if (context.TargetNode is ClassDeclarationSyntax type && context.TargetSymbol is INamedTypeSymbol symbol)
            return new(type, symbol);
        return null;
    }
}
~~~

### 4. HelloSource是比较纯净的业务逻辑
~~~csharp
class HelloSource(ClassDeclarationSyntax type, INamedTypeSymbol symbol)
    : IGeneratorSource
{
    private readonly ClassDeclarationSyntax _type = type;
    private readonly INamedTypeSymbol _symbol = symbol;
    string GenerateFileName
        => $"{_symbol.ToDisplayString()}.Hello.g.cs";
    public SyntaxGenerator Generate()
    {
        var builder = SyntaxGenerator.Clone(_type);
        var method = GenerateMethod();
        builder.AddMember(method);
        return builder;
    }
    public static MethodDeclarationSyntax GenerateMethod()
    {
        var name = SyntaxFactory.IdentifierName("name");
        var expression = SyntaxGenerator.Interpolation()
            .Add("Hello: '")
            .Add(name)
            .Add("'")
            .Build();

        return SyntaxGenerator.VoidType.Method("SayHello", SyntaxGenerator.StringType.Parameter(name.Identifier))
            .Public()
            .Static()
            .ToBuilder()
            .Add(SyntaxFactory.IdentifierName("Console").Access("WriteLine").Invocation([expression]))
            .End();
    }
}
~~~

### 5. 测试代码如下
~~~csharp
namespace GenerateCoreTests.Hello;

[HelloGenerator]
public partial class HelloTests;
~~~

### 6. 生成代码如下
~~~csharp
// <auto-generated/>
namespace GenerateCoreTests.Hello;
partial class HelloTests
{
    public static void SayHello(string name)
    {
        Console.WriteLine($"Hello: '{name}'");
    }
}
~~~

## 五、GenerateCore更多功能
### 1. 分析编译符号
>* 有时需要分析现有代码才能更好的生成代码
>* 用SymbolTypeDescriptor和SymbolTypeBuilder分析代码

#### 1.1 提取符号
>* 可以提取其中一种或多种符号

~~~csharp
SymbolTypeDescriptor GetDescriptor(Compilation compilation, INamedTypeSymbol symbol)
{
    // 提取字段、属性、构造函数、方法和运算符重载等信息
    var builder = new SymbolTypeBuilder()
        .WithField()
        .WithProperty()
        .WithConstructor()
        .WithOperator()
        .WithMethod();
    return builder.Build(compilation, symbol);
}
~~~

#### 1.2 获取符号
>* 通过SymbolTypeDescriptor获取符号
>* 这样可以判断某个成员是否存在,用于生成排重或是否可以调用
>* 只有先配置了提取的符号类型才能获取到
>* 以下获取符号比直接分析成员简单且可读性好

~~~csharp
/// <summary>
/// 获取字段
/// </summary>
IFieldSymbol? GetField(string name);
/// <summary>
/// 获取属性
/// </summary>
IPropertySymbol? GetProperty(string name);
/// <summary>
/// 获取构造函数
/// </summary>
IMethodSymbol? GetConstructor(params INamedTypeSymbol[] parameterTypes)
/// <summary>
/// 获取方法
/// </summary>
IMethodSymbol? GetMethod(string name, bool isPartial, params INamedTypeSymbol[] parameterTypes);
/// <summary>
/// 获取方法
/// </summary>
IMethodSymbol? GetMethod(string name, params INamedTypeSymbol[] parameterTypes);
/// <summary>
/// 按返回类型获取方法
/// </summary>
IEnumerable<IMethodSymbol> GetMethodsByReturnType(INamedTypeSymbol returnType);
/// <summary>
/// 获取运算符重载
/// </summary>
IMethodSymbol? GetOperator(string name, params INamedTypeSymbol[] parameterTypes);
/// <summary>
/// 获取相等重载符
/// </summary>
IMethodSymbol? GetEqualOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取不等重载符
/// </summary>
IMethodSymbol? GetUnEqualOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取相加重载符
/// </summary>
IMethodSymbol? GetAddOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取相减重载符
/// </summary>
IMethodSymbol? GetSubtractOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取相乘重载符
/// </summary>
IMethodSymbol? GetMultiplyOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取相除重载符
/// </summary>
IMethodSymbol? GetDivideOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取求余重载符
/// </summary>
IMethodSymbol? GetModOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取逻辑与重载符
/// </summary>
IMethodSymbol? GetAndOperator(INamedTypeSymbol otherType);
/// <summary>
/// 获取逻辑或重载符
/// </summary>
IMethodSymbol? GetOrOperator(INamedTypeSymbol otherType);
~~~

### 2. SymbolAttributeHelper解析Attribute
~~~csharp
/// <summary>
/// 获取标记
/// </summary>
IEnumerable<AttributeData> GetAttributesByType(IEnumerable<AttributeData> attributes, INamedTypeSymbol attributeType);
/// <summary>
/// 获取标记
/// </summary>
IEnumerable<AttributeData> GetAttributesByType(ISymbol symbol, INamedTypeSymbol attributeType);
/// <summary>
/// 获取标记参数值
/// </summary>
TValue? GetArgumentValue<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, string name);
/// <summary>
/// 获取标记参数值
/// </summary>
TValue? GetArgumentValue<TValue>(ISymbol symbol, INamedTypeSymbol? attributeType, int index);
/// <summary>
/// 获取标记参数值
/// </summary>
TValue? GetArgumentValue<TValue>(AttributeData attribute, string name);
/// <summary>
/// 获取标记参数值
/// </summary>
TValue? GetArgumentValue<TValue>(AttributeData attribute, int index);
~~~

## 六、SourceGenerator测试
>* SourceGenerator的测试是有点麻烦

### 1. Debugger.Launch用于测试
>* 在需要测试的代码前增加Debugger.Launch()
>* vs生成代码会触发断点
>* 缺点是不能单元测试且多线程执行影响调试体验还容易导致vs崩溃

### 2. Microsoft.CodeAnalysis.Analyzer.Testing用于测试
>* 优点是官方支持、支持单元测试
>* 感兴趣的可以体验一下,笔者有点用不习惯

### 3. 开源项目SyntaxScripting测试SourceGenerator
>* SyntaxScripting非常简单,只有1个类SyntaxTreeScript
>* 用于解析、编译代码和执行SourceGenerator
>* 执行SourceGenerator是通过官方的GeneratorDriver实现的
>* 是本次重点推荐的组件

#### 3.1 SyntaxScripting执行SourceGenerator的Case
>* Generate返回GeneratorDriver,再通过GetRunResult可以获取生成结果
>* GeneratedTrees为本次生成的表达式树,如果为空表示没有生成成功
>* Diagnostics返回编译信息,一般为出错信息

~~~csharp
var source = "partial class Greeting;";
var service = SyntaxTreeScript.Create()
    .Using("System");
var result = service.Generate<HelloGenerator>(source)
    .GetRunResult();
var tree = result.GeneratedTrees.FirstOrDefault();
Assert.NotNull(tree);
var diagnostics = result.Diagnostics;
Assert.Empty(diagnostics);
~~~

#### 3.2 SyntaxScripting添加引用
>* 通过Reference增加该类的程序集为引用

~~~csharp
var service = SyntaxTreeScript.Create()
    .Reference<DateTime>();
~~~

#### 3.3 Default
>* Default是默认实例,静态共享
>* 默认using System
>* 默认引用CurrentDomain的所有程序集
>* 易错点是当前引用通过AppDomain.CurrentDomain.GetAssemblies()不一定能获取到
>* 如果没有类被调用程序集可能就不会被加载
>* 也就是说即使用Default也可能需要添加引用

~~~csharp
var source = "partial class Greeting;";
var result = SyntaxScripting.Default.Generate<HelloGenerator>(source)
    .GetRunResult();
var tree = result.GeneratedTrees.FirstOrDefault();
Assert.NotNull(tree);
var diagnostics = result.Diagnostics;
Assert.Empty(diagnostics);
~~~

#### 3.4 CreateDefault
>* 同Default一样using System并引用CurrentDomain的所有程序集
>* 不同之处是非共享,每次调用CreateDefault都是新实例

~~~csharp
var source = "partial class Greeting;";
var result = SyntaxScripting.CreateDefault()
    .Generate<HelloGenerator>(source)
    .GetRunResult();
var tree = result.GeneratedTrees.FirstOrDefault();
Assert.NotNull(tree);
var diagnostics = result.Diagnostics;
Assert.Empty(diagnostics);
~~~

#### 3.5 new可以配置更多参数
~~~csharp
SyntaxTreeScript(
    CSharpParseOptions options, 
    string path, 
    List<UsingDirectiveSyntax> usings, 
    List<MetadataReference> references);
~~~

### 4. 直接测试转化源对象
>* 其他的逻辑都可以理解为配置
>* 转化源对象是真正的业务逻辑
>* 是更适合单元测试的对象

#### 4.1 测试代码如下
>* 代码虽然看着多,其实很简单
>* 而且绕过了SourceGenerator机制,直接执行业务逻辑
>* 这里还是用了SyntaxScripting组件,用来简化测试代码
>* 首先编译源代码code0
>* 其次从编译信息从提取type和symbol
>* 执行HelloSource

~~~csharp
var code0 = @"
namespace GenerateCoreTests.Hello;

[HelloGenerator]
partial class HelloTests;
";
var compilation = SyntaxTreeScript.Default
    .Compile(code0);
var syntaxTree = compilation.SyntaxTrees.FirstOrDefault();
Assert.NotNull(syntaxTree);
var type = syntaxTree.GetRoot()
    .DescendantNodes()
    .OfType<ClassDeclarationSyntax>()
    .FirstOrDefault();
Assert.NotNull(type);
var semanticModel = compilation.GetSemanticModel(syntaxTree);
var symbol = semanticModel.GetDeclaredSymbol(type);
Assert.NotNull(symbol);
HelloSource source = new(type, symbol);
var builder = source.Generate();
var code = builder.Build()
    .WithGenerated()
    .ToFullString();
Assert.Contains("SayHello", code);
~~~

## 七、总结
>* partial范式(GenerateCore+EasySyntax)简化SourceGenerator开发
>* partial范式支持把业务(GeneratorSource)独立拆分,增加代码可读性和可测试性
>* SyntaxScripting(+EasySyntax)支持对SourceGenerator执行和测试
>* SyntaxScripting也能支持对GeneratorSource直接执行测试


源码托管地址: https://github.com/donetsoftwork/Hand.Generators ，欢迎大家直接查看源码。
gitee同步更新:https://gitee.com/donetsoftwork/hand.-generators

如果大家喜欢请动动您发财的小手手帮忙点一下Star,谢谢！！！
