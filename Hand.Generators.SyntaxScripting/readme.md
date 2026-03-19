# SyntaxTree执行器

## 一、 配置
### 1. Using
>* 添加using

~~~csharp
var service = SyntaxTreeScript.Create()
    .Using("System");
~~~

### 2. Reference
>* 添加引用

~~~csharp
var service = SyntaxTreeScript.Create()
    .Reference<DateTime>();
~~~

### 3. Default
>* 默认实例,静态共享
>* 默认using System
>* 默认引用CurrentDomain的程序集

~~~csharp
var service = SyntaxTreeScript.Default;
Assert.Single(service.Usings);
Assert.NotEmpty(service.References);
~~~

### 4. CreateDefault
>* 默认using System
>* 默认引用CurrentDomain的程序集

~~~csharp
var service = SyntaxTreeScript.CreateDefault();
Assert.Single(service.Usings);
Assert.NotEmpty(service.References);
~~~

## 二、Parse
>* 把源代码(字符串)转化为SyntaxTree

~~~csharp
var source = "public record UserCreateTime(DateTime Original);";
var service = SyntaxTreeScript.Create()
    .Using("System");
var tree = service.Parse(source);
~~~

## 三、Compile
>* 编译源代码(字符串)

~~~csharp
var source = "public record UserCreateTime(DateTime Original);";
var service = SyntaxTreeScript.Create()
    .Using("System");
var compilation = service.Compile(source);
var type = compilation.GetTypeByMetadataName("UserCreateTime");
~~~

## 四、Generate
>* Generate返回GeneratorDriver,再通过GetRunResult可以获取生成结果
>* GeneratedTrees为本次生成的表达式树,如果为空表示没有生成成功
>* Diagnostics返回编译信息,一般为出错信息

~~~csharp
var source = "public partial class Greeting;";
var service = SyntaxTreeScript.Create()
    .Using("System");
var result = service.Generate<HelloGenerator>(source)
    .GetRunResult();
var tree = result.GeneratedTrees.FirstOrDefault();
Assert.NotNull(tree);
var diagnostics = result.Diagnostics;
Assert.Empty(diagnostics);
~~~
