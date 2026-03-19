# .NET源码生成器之SyntaxTree踩坑

## 一、不可变性的坑
### 1. 节点不可变
>* record调用AddParameterListParameters后record并不会修改
>* 以下Case中record的代码依然是record Person;

~~~csharp
var record = SyntaxGenerator.RecordDeclaration("Person")
    .WithSemicolonToken();
record.AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
// record Person;
~~~

### 2. 可以用变量接收更改后的结果
>* record和record2是不一样的
>* record代码为record Person;
>* record2代码为record Person(string Name);

~~~csharp
var record = SyntaxGenerator.RecordDeclaration("Person")
    .WithSemicolonToken();
var record2 = record.AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
~~~

### 3. 不可变的集合
>* SyntaxTree常用的SyntaxList和SeparatedSyntaxList都是不可变的
>* 以下集合初始化和Add都无效
>* 需要特别注意
>* 如果用变量赋值Add的返回值是可以的

~~~csharp
var list = new SeparatedSyntaxList<ParameterSyntax>
{
    SyntaxGenerator.IntType.Parameter("Id")
};
list.Add(SyntaxGenerator.StringType.Parameter("Name"));
Assert.Empty(list);
~~~

#### 3.1 EasySyntax用List保存临时结果
>* List保存临时结果,最后组装SyntaxTree

~~~csharp
public class SyntaxGenerator
{
    protected readonly List<UsingDirectiveSyntax> _usings = usings;
    protected readonly TypeDeclarationSyntax _type = type;
    protected readonly List<ParameterSyntax> _parameters = [];
    protected readonly List<MemberDeclarationSyntax> _members = members;
    public virtual CompilationUnitSyntax Build()
        => Build(_usings, _type, [.. _parameters], [.. _members]
    // ...
}
public abstract class StatementCollect
{
    protected readonly List<StatementSyntax> _statements = [];
    public static StatementSyntax Block(List<StatementSyntax> statements)
    {
        return statements.Count switch
        {
            0 => SyntaxFactory.EmptyStatement(),
            1 => statements[0],
            _ => SyntaxFactory.Block(statements),
        };
    }
    // ...
}
~~~

## 二、“类型”系统的坑
>* 这里说的“类型”是指类型名或者说类型名占位符
>* 用于表示变量、属性和方法返回值类型

### 1. 预定义类型
>* 使用SyntaxFactory.PredefinedType定义预定义类型需要传个SyntaxToken
>* 但是这个SyntaxToken不满足条件就会抛异常
>* 指定16种情况,类似枚举
>* 却没有一对应的类型、枚举或属性

~~~csharp
public static PredefinedTypeSyntax PredefinedType(SyntaxToken keyword)
{
    switch (keyword.Kind())
    {
        case SyntaxKind.BoolKeyword:
        case SyntaxKind.ByteKeyword:
        case SyntaxKind.SByteKeyword:
        case SyntaxKind.IntKeyword:
        case SyntaxKind.UIntKeyword:
        case SyntaxKind.ShortKeyword:
        case SyntaxKind.UShortKeyword:
        case SyntaxKind.LongKeyword:
        case SyntaxKind.ULongKeyword:
        case SyntaxKind.FloatKeyword:
        case SyntaxKind.DoubleKeyword:
        case SyntaxKind.DecimalKeyword:
        case SyntaxKind.StringKeyword:
        case SyntaxKind.CharKeyword:
        case SyntaxKind.ObjectKeyword:
        case SyntaxKind.VoidKeyword: break;
        default: throw new ArgumentException(nameof(keyword));
    }
    return (PredefinedTypeSyntax)Syntax.InternalSyntax.SyntaxFactory.PredefinedType((Syntax.InternalSyntax.SyntaxToken)keyword.Node!).CreateRed();
}
~~~

#### 1.1 EasySyntax中填了这个坑
>* SyntaxGenerator的其中16个静态属性表示这16种预定义类型

~~~csharp
 /// <summary>
 /// bool
 /// </summary>
 public static PredefinedTypeSyntax BoolType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
 /// <summary>
 /// byte
 /// </summary>
 public static PredefinedTypeSyntax ByteType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword));
 /// <summary>
 /// sbyte
 /// </summary>
 public static PredefinedTypeSyntax SByteType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword));
 /// <summary>
 /// int
 /// </summary>
 public static PredefinedTypeSyntax IntType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
 /// <summary>
 /// uint
 /// </summary>
 public static PredefinedTypeSyntax UIntType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword));
 /// <summary>
 /// short
 /// </summary>
 public static PredefinedTypeSyntax ShortType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword));
 /// <summary>
 /// ushort
 /// </summary>
 public static PredefinedTypeSyntax UShortType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword));
 /// <summary>
 /// long
 /// </summary>
 public static PredefinedTypeSyntax LongType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword));
 /// <summary>
 /// ulong
 /// </summary>
 public static PredefinedTypeSyntax ULongType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword));
 /// <summary>
 /// float
 /// </summary>
 public static PredefinedTypeSyntax FloatType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword));
 /// <summary>
 /// double
 /// </summary>
 public static PredefinedTypeSyntax DoubleType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword));
 /// <summary>
 /// decimal
 /// </summary>
 public static PredefinedTypeSyntax DecimalType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword));
 /// <summary>
 /// string
 /// </summary>
 public static PredefinedTypeSyntax StringType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));
 /// <summary>
 /// char
 /// </summary>
 public static PredefinedTypeSyntax CharType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword));
 /// <summary>
 /// object
 /// </summary>
 public static PredefinedTypeSyntax ObjectType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword));
 /// <summary>
 /// void
 /// </summary>
 public static PredefinedTypeSyntax VoidType => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
~~~

### 2. 普通类型
>* 一个简单的类型名,非预定义的16种情况
>* IdentifierNameSyntax是TypeSyntax子类,可以表示类型名
>* IdentifierNameSyntax是标识名,也可以表示变量名、字段名、属性名、方法名或命名空间
>* IdentifierNameSyntax是TypeSyntax子类这个事实笔者也是花了一些时间才消化的
>* 这是SyntaxTree的设计哲学,设计如此
>* 想通了也好理解,类名是个标识符、变量名和类型成员名何尝不是标识符,没必要区分开
>* SyntaxFactory.IdentifierName("User")可以表示User类
>* SyntaxFactory.IdentifierName("user")可以表示User类的实例

~~~csharp
var type = SyntaxFactory.IdentifierName("User");
var user = SyntaxFactory.IdentifierName("user");
// User user = new()
var variable = type.Variable(user.Identifier, SyntaxFactory.ImplicitObjectCreationExpression());
~~~

### 3. 限定名类型
>* 就是含命名空间的类名
>* QualifiedNameSyntax也是TypeSyntax子类,可以表示限定名类型
>* 同样QualifiedNameSyntax也可以表示成员名
>* Models.User可以表示特定的类
>* user1.Name自然也可以表示特定的属性

~~~csharp
var type = SyntaxFactory.IdentifierName("User");
var user = SyntaxFactory.IdentifierName("user1");
var property = SyntaxFactory.IdentifierName("Name");
var @namespace = SyntaxFactory.IdentifierName("Models");
// Models.User
var qualifiedType = type.Qualified(@namespace);
// user1.Name
var qualifiedProperty = property.Qualified(user1);
~~~

### 4. 泛型
>* 就是含类型参数的类名
>* GenericNameSyntax也是TypeSyntax子类,可以表示泛型类型
>* GenericNameSyntax也可以表示泛型方法

~~~csharp
// List<int>
var listType = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
// GetFieldValue<int>
var method = SyntaxGenerator.Generic("GetFieldValue", SyntaxGenerator.IntType);
~~~

### 5. 泛型限定名
>* 就是含命名空间的泛型
>* 泛型限定名用GenericNameSyntax还是QualifiedNameSyntax呢

#### 5.1 ParseTypeName得到的是QualifiedNameSyntax
>* SyntaxFactory.ParseTypeName("System.Collections.Generic.List\<int\>")的实际类型QualifiedNameSyntax
>* 但是ParseTypeName性能不好

#### 5.2 QualifiedNameSyntax表示泛型
>* SyntaxGenerator.Generic("List", SyntaxGenerator.IntType).Qualified("System.Collections.Generic")

#### 5.3 GenericNameSyntax也可以表示泛型限定名
>* SyntaxGenerator.Generic("System.Collections.Generic.List", SyntaxGenerator.IntType)

#### 5.4 GenericNameSyntax还是QualifiedNameSyntax选哪个?
>* 首先两者能生成一样的代码
>* ParseTypeName得到QualifiedNameSyntax,应该代表官方意见
>* 使用GenericNameSyntax性能好,也说得过去,带命名空间的泛型不还是泛型吗?就像白马也是马
>* 这是问题,是个仁者见仁智者见智的问题
>* 也可以说这不是问题,笔者觉得两个都可以用,同一个团队约定其中一种规则即可

## 三、ParseTypeName的坑
### 1. 预定义类型
#### 1.1 预定义类型应该使用SyntaxFactory.PredefinedType
>* ParseTypeName("int")的实际类型PredefinedTypeSyntax
>* 使用SyntaxGenerator.IntType也是不错的

~~~csharp
var type = SyntaxFactory.ParseTypeName("int");
if (type is PredefinedTypeSyntax predefinedType)
{
    var predefinedType0 = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
    Assert.Equal(predefinedType0.Keyword.Kind(), predefinedType.Keyword.Kind());
}
~~~

#### 1.2 预定义类型调用ParseTypeName性能不好
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("int")
>* PredefinedType代码为SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))
>* ParseTypeName耗时为30倍,内存为12倍
>* 如果嫌SyntaxFactory.PredefinedType表达式长,可以试试SyntaxGenerator.IntType
>* 需要安装开源包dotnet add package Hand.Generators.EasySyntax --version 0.1.0.1-alpha

| Method         | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| ParseTypeName  | 217.174 ns | 0.6011 ns | 0.6681 ns | 217.703 ns | 30.80 |    0.72 | 0.0575 |     992 B |       12.40 |
| PredefinedType |   7.056 ns | 0.1512 ns | 0.1681 ns |   6.929 ns |  1.00 |    0.03 | 0.0046 |      80 B |        1.00 |

### 2. 普通类型
#### 2.1 普通类型应该使用SyntaxFactory.IdentifierName
>* ParseTypeName("User")的实际类型IdentifierNameSyntax

~~~csharp
var type = SyntaxFactory.ParseTypeName("User");
if (type is IdentifierNameSyntax identifierName)
{
    var identifierName0 = SyntaxFactory.IdentifierName("User");
    Assert.Equal(identifierName0.ToFullString(), identifierName.ToFullString());
}
~~~

#### 2.2 普通类型调用ParseTypeName性能也不好
>* IdentifierName代码为SyntaxFactory.IdentifierName("User")
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("User")
>* ParseTypeName耗时为46倍,内存为7倍

| Method         | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|--------------- |----------:|---------:|---------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| IdentifierName |  10.84 ns | 0.238 ns | 0.275 ns |  1.00 |    0.03 | 0.0083 |      - |      - |     144 B |        1.00 |
| ParseTypeName  | 508.55 ns | 0.100 ns | 0.111 ns | 46.95 |    1.15 | 0.0694 | 0.0163 | 0.0082 |    1056 B |        7.33 |

### 3. 限定名类型
>* 就是含命名空间的类名

#### 3.1 限定名类型应该使用SyntaxFactory.IdentifierName
>* ParseTypeName("Models.User")的实际类型QualifiedNameSyntax与SyntaxFactory.IdentifierName("User").Qualified("Models")一致
>* 其实用SyntaxFactory.IdentifierName("Models.User")也是可以的

~~~csharp
var type = SyntaxFactory.ParseTypeName("Models.User");
if (type is QualifiedNameSyntax qualifiedName)
{
    var qualifiedName0 = SyntaxFactory.IdentifierName("User").Qualified("Models");
    Assert.Equal(qualifiedName0.ToFullString(), qualifiedName.ToFullString());
    var identifierName = SyntaxFactory.IdentifierName("Models.User");
    Assert.Equal(qualifiedName0.ToFullString(), identifierName.ToFullString());
}
~~~

#### 3.2 限定名类型调用ParseTypeName性能也不好
>* QualifiedName代码为SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("Models"), SyntaxFactory.IdentifierName("User"))
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("Models.User")
>* IdentifierName代码为SyntaxFactory.IdentifierName("Models.User")
>* ParseTypeName耗时22倍,内存2倍多
>* IdentifierName性能最好

| Method         | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|--------------- |----------:|---------:|---------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| QualifiedName  |  30.85 ns | 0.101 ns | 0.116 ns |  1.00 |    0.01 | 0.0231 |      - |      - |     400 B |        1.00 |
| ParseTypeName  | 707.37 ns | 0.029 ns | 0.033 ns | 22.93 |    0.08 | 0.0713 | 0.0194 | 0.0064 |    1120 B |        2.80 |
| IdentifierName |  10.69 ns | 0.090 ns | 0.104 ns |  0.35 |    0.00 | 0.0083 |      - |      - |     144 B |        0.36 |

### 4. 泛型
>* 就是含类型参数的类名

#### 4.1 泛型应该使用SyntaxFactory.GenericName
>* ParseTypeName("List\<int\>")的实际类型GenericNameSyntax与SyntaxGenerator.Generic("List", SyntaxGenerator.IntType)一致
>* SyntaxGenerator.Generic("List", SyntaxGenerator.IntType)是SyntaxFactory.GenericName("List").AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))的简化

~~~csharp
var type = SyntaxFactory.ParseTypeName("List<int>");
if (type is not GenericNameSyntax genericName)
{
    var genericName0 = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
    Assert.Equal(genericName0.ToFullString(), genericName.ToFullString());
}
~~~

#### 4.2 泛型调用ParseTypeName性能也不好
>* GenericName代码为SyntaxFactory.GenericName("List").AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("List\<int\>")
>* ParseTypeName耗时2倍

| Method        | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------- |---------:|--------:|--------:|------:|--------:|-------:|-------:|----------:|------------:|
| GenericName   | 128.9 ns | 0.92 ns | 1.02 ns |  1.00 |    0.01 | 0.0607 |      - |   1.02 KB |        1.00 |
| ParseTypeName | 284.3 ns | 0.69 ns | 0.74 ns |  2.21 |    0.02 | 0.0658 | 0.0001 |   1.11 KB |        1.08 |


### 5. 泛型限定名
>* 就是含命名空间的泛型

#### 5.1 泛型限定名应该使用SyntaxFactory.IdentifierName
>* SyntaxFactory.ParseTypeName("System.Collections.Generic.List\<int\>")的实际类型QualifiedNameSyntax与SyntaxGenerator.Generic("List", SyntaxGenerator.IntType).Qualified("System.Collections.Generic")一致
>* 其实直接用GenericName也是可以的

~~~csharp
var type = SyntaxFactory.ParseTypeName("System.Collections.Generic.List<int>");
if (type is QualifiedNameSyntax qualifiedName)
{
    var qualifiedName0 = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType).Qualified("System.Collections.Generic");
    Assert.Equal(qualifiedName0.ToFullString(), qualifiedName.ToFullString());
    var genericName = SyntaxGenerator.Generic("System.Collections.Generic.List", SyntaxGenerator.IntType);
    Assert.Equal(qualifiedName0.ToFullString(), genericName.ToFullString());
}
~~~

#### 5.2 泛型限定名调用ParseTypeName性能也不好
>* QualifiedName代码为SyntaxGenerator.Generic("List", SyntaxGenerator.IntType).Qualified("System.Collections.Generic")
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("System.Collections.Generic.List\<int\>")
>* GenericName代码为SyntaxGenerator.Generic("System.Collections.Generic.List", SyntaxGenerator.IntType)
>* ParseTypeName耗时5倍多
>* 直接SyntaxGenerator.Generic性能最好

| Method        | Mean      | Error    | StdDev   | Median    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------- |----------:|---------:|---------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| QualifiedName |  72.81 ns | 2.099 ns | 2.333 ns |  71.06 ns |  1.00 |    0.04 | 0.0384 |      - |     664 B |        1.00 |
| ParseTypeName | 425.04 ns | 1.605 ns | 1.717 ns | 426.37 ns |  5.84 |    0.18 | 0.0663 | 0.0001 |    1144 B |        1.72 |
| GenericName   |  42.27 ns | 0.637 ns | 0.708 ns |  42.81 ns |  0.58 |    0.02 | 0.0236 |      - |     408 B |        0.61 |

### 6. 什么时候用ParseTypeName呢
>* 笔者觉得只要预先明确类型名的都不应该使用ParseTypeName
>* 只有类型名传参或者从文本中解析出来才不得已用ParseTypeName

## 四、成员名如何表示
### 1. 用QualifiedNameSyntax表示
>* 前面提到用限定名表示属性

~~~csharp
var user = SyntaxFactory.IdentifierName("user1");
var property = SyntaxFactory.IdentifierName("Name");
// user1.Name
var qualifiedProperty = property.Qualified(user1);
~~~

### 2. 也可以用MemberAccessExpressionSyntax表示属性
~~~csharp
var user = SyntaxFactory.IdentifierName("user1");
// user1.Name
var userName = user.Access("Name");
~~~

### 3. QualifiedNameSyntax和MemberAccessExpressionSyntax哪种更好呢
>* 从语义上来说笔者觉得MemberAccessExpressionSyntax更准确
>* 但两者生成的代码是一样的
>* Access是从父对象获取子对象
>* 一个是成员限定所属上级对象
>* 有些场景两者都可用
>* 有些场景用其中一个更合适
>* 如下Case用Qualified就更巧妙

~~~csharp
var type = SyntaxFactory.IdentifierName("Student");
var score = SyntaxFactory.IdentifierName("Score");
var other = SyntaxFactory.IdentifierName("other");
// int Compare(Student other)
var method = SyntaxGenerator.IntType.Method("Compare", type.Parameter(other.Identifier))
    .ToBuilder()
    // return Score - other.Score
    .Return(score.Subtract(score.Qualified(other)));
~~~

## 五、空与null不同
### 1. 类型参数空与null不同
#### 1.1 ParameterList()定义空参数
~~~csharp
var recordDeclaration = SyntaxGenerator.RecordDeclaration("Person")
    .WithParameterList(SyntaxFactory.ParameterList())
    .WithSemicolonToken();
// record Person();
~~~

#### 1.2 null为无参数
>* 默认就是null

~~~csharp
var recordDeclaration = SyntaxGenerator.RecordDeclaration("Person")
    .WithParameterList(null)
    .WithSemicolonToken();
// record Person;
~~~

## 六、分号问题
>* 一般SyntaxTree会自动处理分号
>* 对于简化语法是需要手动加分号的
>* 使用WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))加分号
>* 可以简化为WithSemicolonToken()

### 1. 无成员的类型需要加分号
~~~csharp
var recordDeclaration = SyntaxGenerator.RecordStructDeclaration("Person")
    .Public()
    .AddParameterListParameters(
        SyntaxGenerator.StringType.Parameter("Name")
    )
    .WithSemicolonToken();
// public record struct Person(string Name);
~~~

### 2. 空方法体的方法
~~~csharp
var method = SyntaxGenerator.IntType.Method("CreateId")
    .Public()
    .Partial()
    .WithSemicolonToken();
// public partial int CreateId();
~~~

### 3. 空的属性操作器
~~~csharp
var property = SyntaxFactory.PropertyDeclaration(
    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
    SyntaxFactory.Identifier("Id"))
    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
    .AddAccessorListAccessors(
        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
        SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
    );
// public int Id { get; set; }
~~~

#### 3.1 使用EasySyntax定义空的属性操作器自动加分号
~~~csharp
var property = SyntaxGenerator.IntType.Property("Id",
    SyntaxKind.GetAccessorDeclaration,
    SyntaxKind.InitAccessorDeclaration)
    .Public();
// public int Id { get; set; }
~~~

## 七、SyntaxFactory和SyntaxKind的坑
>* SyntaxKind枚举有几百项
>* SyntaxFactory也有几百个静态方法,很多静态方法需要配合SyntaxKind使用
>* 有时从SyntaxFactory几百中选正确方法再从SyntaxKind几百中选正确的枚举项是个不小的挑战
>* 而且往往需要SyntaxFactory好几层嵌套
>* EasySyntax解决了部分问题,而且是常用的那些
>* 很多需要嵌套也能巧妙的改成链式的
>* 体验EasySyntax安装开源包 Hand.Generators.EasySyntax --version 0.1.0.1-alpha

## 八、总结
>* SyntaxTree有些坑还是需要注意的
>* 以上总结了不少了,建议大家收藏好,说不定哪天要考
>* 源码生成器是.NET的高级技能,无反射、性能好、AOT友好,对于.NET高级成员是必考题
>* EasySyntax已经填了一些坑,建议尝试一下
>* EasySyntax更多信息可以参考[上一篇文章](https://www.cnblogs.com/xiangji/p/19688804)


源码托管地址: https://github.com/donetsoftwork/Hand.Generators ，欢迎大家直接查看源码。
gitee同步更新:https://gitee.com/donetsoftwork/hand.-generators

如果大家喜欢请动动您发财的小手手帮忙点一下Star,谢谢！！！
