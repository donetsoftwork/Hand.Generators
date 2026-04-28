# SyntaxTree执行器

## 一、 配置
### 1. Using
>* 添加using

~~~csharp
var service = SyntaxTreeDriver.CreateDriver()
    .Using("System");
~~~

### 2. Reference
>* 添加引用

~~~csharp
var service = SyntaxTreeDriver.CreateDriver()
    .Reference<DateTime>();
~~~

### 3. DefaultDriver
>* 默认实例,静态共享
>* 默认using System
>* 默认引用CurrentDomain的程序集

~~~csharp
var service = SyntaxTreeDriver.DefaultDriver;
Assert.Single(service.Usings);
Assert.NotEmpty(service.References);
~~~

### 4. ScriptDriver
>* 默认脚本实例,静态共享
>* 配置SourceCodeKind.Script,支持脚本代码
>* 默认using System
>* 默认引用CurrentDomain的程序集

~~~csharp
var service = SyntaxTreeDriver.ScriptDriver;
Assert.Single(service.Usings);
Assert.NotEmpty(service.References);
~~~

### 5. CreateDefaultDriver
>* 默认using System
>* 默认引用CurrentDomain的程序集

~~~csharp
var service = SyntaxTreeDriver.CreateDefaultDriver();
Assert.Single(service.Usings);
Assert.NotEmpty(service.References);
~~~

### 6. CreateScriptDriver
>* 默认using System
>* 配置SourceCodeKind.Script,支持脚本代码
>* 默认引用CurrentDomain的程序集

~~~csharp
var service = SyntaxTreeDriver.CreateScriptDriver();
Assert.Single(service.Usings);
Assert.NotEmpty(service.References);
~~~

## 二、Parse
>* 把源代码(字符串)转化为SyntaxTree

~~~csharp
var source = "public record UserCreateTime(DateTime Original);";
var service = SyntaxTreeDriver.CreateDriver()
    .Using("System");
var tree = service.Parse(source);
~~~

## 三、Compile
>* 编译源代码(字符串)

~~~csharp
var source = "public record UserCreateTime(DateTime Original);";
var service = SyntaxTreeDriver.CreateDriver()
    .Using("System");
var compilation = service.Compile(source);
var type = compilation.GetTypeByMetadataName("UserCreateTime");
~~~

## 四、ScriptCompile
>* 编译脚本

~~~csharp
var compilation = SyntaxTreeDriver.ScriptDriver.ScriptCompile("1+2");
var type = compilation.GetTypeByMetadataName("Script");
~~~

## 五、CreateScript
>* 创建脚本实例,通过ExecuteAsync执行获取结果
>* 类似Microsoft.CodeAnalysis.CSharp.Scripting,但更轻量级

~~~csharp
var script = SyntaxTreeDriver.ScriptDriver
    .CreateScript<int>("2+3");
var result = await script.ExecuteAsync();
Assert.Equal(5, result);
~~~

## 六、Generate
>* Generate返回GeneratorDriver,再通过GetRunResult可以获取生成结果
>* GeneratedTrees为本次生成的表达式树,如果为空表示没有生成成功
>* Diagnostics返回编译信息,一般为出错信息

~~~csharp
var source = "public partial class Greeting;";
var service = SyntaxTreeDriver.CreateDriver()
    .Using("System");
var result = service.Generate<HelloGenerator>(source)
    .GetRunResult();
var tree = result.GeneratedTrees.FirstOrDefault();
Assert.NotNull(tree);
var diagnostics = result.Diagnostics;
Assert.Empty(diagnostics);
~~~
