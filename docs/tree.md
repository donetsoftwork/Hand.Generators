# .net源码生成器使用SyntaxTree生成代码及简化语法

## 一、SyntaxTree是什么
>* SyntaxTree是语法树,是源代码的树形结构表示
>* 由Roslyn编译器生成
>* 在SourceGenerator中会自动生成
>* 整个源代码结构是1个SyntaxTree
>* SyntaxTree有一个根节点(SyntaxNode)
>* 每个SyntaxNode也包含一个SyntaxTree
>* 这样看整个源代码结构就是片“森林”

~~~csharp
public abstract partial class SyntaxNode
{
    // ...
    public SyntaxTree SyntaxTree => this.SyntaxTreeCore;
}

public abstract class SyntaxTree
{
    // ...
    public SyntaxNode GetRoot(CancellationToken cancellationToken = default)
    {
        return GetRootCore(cancellationToken);
    }
}
~~~

## 二、SyntaxTree的作用
### 1. 分析源代码
>* 用于分析本次增量代码
>* 是否满足生成器规则
>* 找出指定的类、方法及特性(Attribute)

### 2. 直接用来生成代码
>* 源代码字符串可以转化为SyntaxTree
>* SyntaxTree也可以转化为源代码
>* 也可以部分用字符串转化为SyntaxTree组合使用

### 3. SyntaxTree生成代码有优势
>* 直接生成语法树保证语法的正确性和准确性
>* 原生csharp代码,更方便开发和调试
>* SyntaxTree不会生成过多的字符串碎片

### 4. SyntaxTree生成代码的方法
>* 一般使用SyntaxFactory类来生成代码
>* 笔者对部分功能进行简化,使得生成代码更简单
>* dotnet add package Hand.Generators.EasySyntax --version 0.1.0.1-alpha
>* 希望通过本篇文章引导大家把拼接字符串生成代码的方式替换为使用SyntaxTree

## 三、SyntaxTree基本元素
### 1. 声明命名空间
#### 1.1 原始方式
~~~csharp
var ns = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("Models"));
var fns = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.IdentifierName("Services"));
~~~

#### 1.2 简化方式
~~~csharp
var ns = SyntaxGenerator.NamespaceDeclaration("Models");
var fns = SyntaxGenerator.FileScopedNamespaceDeclaration("Services");
~~~

#### 1.3 生成的代码
~~~csharp
namespace Models
{
}

namespace Services;
~~~

### 2. 预定义类型
#### 2.1 原始方式
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ByteKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.SByteKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UIntKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ShortKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.UShortKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.LongKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ULongKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.FloatKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DoubleKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.DecimalKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.CharKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
>* SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword))

#### 2.2 简化方式
>* SyntaxGenerator.BoolType
>* SyntaxGenerator.ByteType
>* SyntaxGenerator.SByteType
>* SyntaxGenerator.IntType
>* SyntaxGenerator.UIntType
>* SyntaxGenerator.ShortType
>* SyntaxGenerator.UShortType
>* SyntaxGenerator.LongType
>* SyntaxGenerator.ULongType
>* SyntaxGenerator.FloatType
>* SyntaxGenerator.DoubleType
>* SyntaxGenerator.DecimalType
>* SyntaxGenerator.StringType
>* SyntaxGenerator.CharType
>* SyntaxGenerator.ObjectType
>* SyntaxGenerator.VoidType

#### 2.3 生成的代码
>* bool
>* byte
>* sbyte
>* int
>* uint
>* short
>* ushort
>* long
>* ulong
>* float
>* double
>* decimal
>* string
>* char
>* object
>* void

### 3. 常量表达式
#### 3.1 原始方式
>* SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))
>* SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1U))
>* SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1L))
>* SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1UL))
>* SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1F))
>* SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1D))
>* SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1M))
>* SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("abc"))
>* SyntaxFactory.LiteralExpression(SyntaxKind.CharacterLiteralExpression, SyntaxFactory.Literal('a'))
>* SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
>* SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)
>* SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
>* SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)
>* SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("Object"))
>* SyntaxFactory.ImplicitObjectCreationExpression()
>* SyntaxFactory.CollectionExpression()

#### 3.2 简化方式
>* SyntaxGenerator.Literal(1)
>* SyntaxGenerator.Literal(1U)
>* SyntaxGenerator.Literal(1L)
>* SyntaxGenerator.Literal(1UL)
>* SyntaxGenerator.Literal(1F)
>* SyntaxGenerator.Literal(1D)
>* SyntaxGenerator.Literal(1M)
>* SyntaxGenerator.Literal("abc")
>* SyntaxGenerator.Literal('a')
>* SyntaxGenerator.TrueLiteral
>* SyntaxGenerator.FalseLiteral
>* SyntaxGenerator.NullLiteral
>* SyntaxGenerator.DefaultLiteral
>* SyntaxFactory.IdentifierName("Object").New()

#### 3.3 生成的代码
>* 1
>* 1U
>* 1L
>* 1UL
>* 1F
>* 1D
>* ushort
>* 1M
>* "abc"
>* 'a'
>* true
>* null
>* default
>* new Object()
>* new()
>* []

### 4. 运算
#### 4.1 原始方式
>* SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.MultiplyExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.DivideExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.ModuloExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.LeftShiftExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.RightShiftExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseAndExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseOrExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.ExclusiveOrExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.LogicalAndExpression, left, right)
>* SyntaxFactory.BinaryExpression(SyntaxKind.LogicalOrExpression, left, right)
>* SyntaxFactory.PrefixUnaryExpression(SyntaxKind.PreIncrementExpression, variable)
>* SyntaxFactory.PrefixUnaryExpression(SyntaxKind.PreDecrementExpression, variable)
>* SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, variable)
>* SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostDecrementExpression, variable)
>* SyntaxFactory.PrefixUnaryExpression(SyntaxKind.BitwiseNotExpression, variable)
>* SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, variable)
>* SyntaxFactory.QualifiedName(prefix, name)
>* SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, owner, member)
>* SyntaxFactory.ConditionalAccessExpression(owner, SyntaxFactory.MemberBindingExpression(SyntaxFactory.IdentifierName(member)))

#### 4.2 简化方式(扩展方法)
>* left.Add(right)
>* left.Subtract(right)
>* left.Multiply(right)
>* left.Divide(right)
>* left.Modulo(right)
>* left.LeftShift(right)
>* left.RightShift(right)
>* left.And(right)
>* left.Or(right)
>* left.XOr(right)
>* left.LogicalAnd(right)
>* left.LogicalOr(right)
>* variable.PreIncrement()
>* variable.PreDecrement()
>* variable.PostIncrement()
>* variable.PostDecrement()
>* variable.Not()
>* variable.LogicalNot()
>* name.Qualified(prefix)
>* owner.Access(member)
>* owner.ConditionalAccess(member)

#### 4.3 生成的代码
>* left + right
>* left - right
>* left * right
>* left / right
>* left % right
>* left << right
>* left >> right
>* left & right
>* left | right
>* left ^ right
>* left && right
>* left || right
>* ++variable
>* --variable
>* variable++
>* variable--
>* ~variable
>* !variable
>* prefix.name
>* owner.member
>* owner?.member

### 5. 定义变量
#### 5.1 原始方式
~~~csharp
var x = SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
    .AddVariables(SyntaxFactory.VariableDeclarator("x"));
var y = SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
    .AddVariables(SyntaxFactory.VariableDeclarator("y")
        .WithInitializer(SyntaxFactory.EqualsValueClause(
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))));
var z = SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(SyntaxFactory.Token(SyntaxKind.VarKeyword)))
    .AddVariables(SyntaxFactory.VariableDeclarator("z")
        .WithInitializer(SyntaxFactory.EqualsValueClause(
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))));
~~~

#### 5.2 简化方式
~~~csharp
var x = SyntaxGenerator.IntType.Variable("x");
var y = SyntaxGenerator.IntType.Variable("y", SyntaxGenerator.Literal(1));
var z = SyntaxGenerator.VarType.Variable("z", SyntaxGenerator.Literal(1));
~~~

#### 5.3 生成的代码
~~~csharp
int x;
int y = 1;
var z = 1;
~~~

### 6. 定义参数
#### 6.1 原始方式
~~~csharp
var a = SyntaxFactory.Parameter(SyntaxFactory.Identifier("a"))
    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
var b = SyntaxFactory.Parameter(SyntaxFactory.Identifier("b"))
    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
    .WithDefault(SyntaxFactory.EqualsValueClause(
        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))));
~~~

#### 6.2 简化方式
~~~csharp
var a = SyntaxGenerator.IntType.Parameter("a");
var b = SyntaxGenerator.IntType.Parameter("b", SyntaxGenerator.Literal(1));
~~~

#### 6.3 生成的代码
~~~csharp
int a
int b = 1
~~~

### 7.定义函数
#### 7.1 原始方式
~~~csharp
var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "Increment")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(SyntaxFactory.Identifier("num"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))),
        SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
            .WithDefault(SyntaxFactory.EqualsValueClause(
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))))
    )
    .WithBody(SyntaxFactory.Block(
        SyntaxFactory.ReturnStatement(
            SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.IdentifierName("num"),
                SyntaxFactory.IdentifierName("value"))
        )
    ));
~~~

#### 7.2 简化方式
~~~csharp
var num = SyntaxFactory.IdentifierName("num");
var value = SyntaxFactory.IdentifierName("value");
var method = SyntaxGenerator.IntType.Method("Increment", 
        SyntaxGenerator.IntType.Parameter(num.Identifier), 
        SyntaxGenerator.IntType.Parameter(value.Identifier, SyntaxGenerator.Literal(1)))
    .ToBuilder()
    .Return(num.Add(value));
~~~

#### 7.3 生成的代码
~~~csharp
int Increment(int num, int value = 1)
{
    return num + value;
}
~~~

### 8. 定义字段
#### 8.1 原始方式
~~~csharp
var _x = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
    .AddDeclarationVariables(
        SyntaxFactory.VariableDeclarator("_x")
    );
var _y = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
    .AddDeclarationVariables(
        SyntaxFactory.VariableDeclarator("_y")
        .WithInitializer(SyntaxFactory.EqualsValueClause(
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))
        )
    );
~~~

#### 8.2 简化方式
~~~csharp
var _x = SyntaxGenerator.IntType.Field("_x");
var _y = SyntaxGenerator.IntType.Field("_y", SyntaxGenerator.Literal(1));
~~~

#### 8.3 生成的代码
~~~csharp
int _x;
int _y = 1;
~~~

### 9. 定义属性
#### 9.1 原始方式
~~~csharp
var Id = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Id"))
    .AddAccessorListAccessors(
        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
    );
var Name = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), SyntaxFactory.Identifier("Name"))
    .AddAccessorListAccessors(
        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .AddBodyStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("_name")))
    );
var Age = SyntaxFactory.PropertyDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.Identifier("Age"))
    .AddAccessorListAccessors(
        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .AddBodyStatements(SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("_age"))),
        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .AddBodyStatements(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_age"), SyntaxFactory.IdentifierName("value"))))
    );
~~~

#### 9.2 简化方式
~~~csharp
var Id = SyntaxGenerator.IntType.Property(SyntaxFactory.Identifier("Id"), SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
var Name = SyntaxGenerator.StringType.GetOnlyProperty(SyntaxFactory.Identifier("Name"), SyntaxFactory.IdentifierName("_name"));
var getAge = SyntaxGenerator.PropertyGetDeclaration()
    .ToBuilder()
    .Return(SyntaxFactory.IdentifierName("_age"));
var setAge = SyntaxGenerator.PropertySetDeclaration()
    .ToBuilder()
    .Add(SyntaxFactory.IdentifierName("_age").AssignValue())
    .End();
var Age = SyntaxGenerator.IntType.Property(SyntaxFactory.Identifier("Age"), getAge, setAge);
~~~

#### 9.3 生成的代码
~~~csharp
int Id { get; set; }
string Name
{
    get
    {
        return _name;
    }
}
int Age
{
    get
    {
        return _age;
    }

    set
    {
        _age = value;
    }
}
~~~

### 10. 声明类和结构体
#### 10.1 原始方式
~~~csharp
var userClass = SyntaxFactory.ClassDeclaration("UserClass");
var userStruct = SyntaxFactory.StructDeclaration("UserStruct");
~~~

#### 10.2. 无简化方式

#### 10.3. 生成的代码
~~~csharp
class UserClass
{
}
struct UserStruct
{
}
~~~

### 11.定义构造函数
#### 11.1 原始方式
~~~csharp
var constructor = SyntaxFactory.ConstructorDeclaration("UserId")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(SyntaxFactory.Identifier("original"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
    .AddBodyStatements(SyntaxFactory.ExpressionStatement(
        SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_original"), SyntaxFactory.IdentifierName("original"))));
~~~

#### 11.2 简化方式
~~~csharp
var type = SyntaxFactory.ClassDeclaration("UserId");
var original = SyntaxFactory.IdentifierName("original");
var constructor = type.Constructor(SyntaxGenerator.IntType.Parameter(original.Identifier))
    .ToBuilder()
    .Add(SyntaxFactory.IdentifierName("_original").Assign(original))
    .End();
~~~

#### 11.3. 生成的代码
~~~csharp
UserId(int original)
{
    _original = original;
}
~~~

### 12. 声明记录类
#### 12.1 原始方式
~~~csharp
var record = SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), "Person")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(SyntaxFactory.Identifier("Name"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
    )
    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
~~~

#### 12.2 简化方式

~~~csharp
var record = SyntaxGenerator.RecordDeclaration("Person")
    .AddParameterListParameters(
        SyntaxGenerator.StringType.Parameter("Name")
    )
    .WithSemicolonToken();
~~~

#### 12.3 生成的代码
~~~csharp
record Person(string Name);
~~~

### 13. 声明记录结构体
#### 13.1 原始方式
~~~csharp
var recordDeclaration = SyntaxFactory.RecordDeclaration(SyntaxKind.RecordStructDeclaration, SyntaxFactory.Token(SyntaxKind.RecordKeyword), SyntaxFactory.Identifier("UserId"))
    .WithClassOrStructKeyword(SyntaxFactory.Token(SyntaxKind.StructKeyword))
    .AddParameterListParameters(
        SyntaxFactory.Parameter(SyntaxFactory.Identifier("Id"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
    )            
    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
~~~

#### 13.2 简化方式
~~~csharp
var type = SyntaxGenerator.RecordStructDeclaration("UserId")
    .AddParameterListParameters(
        SyntaxGenerator.IntType.Parameter("Id")
    )
    .WithSemicolonToken();
~~~

#### 13.3 生成的代码
~~~csharp
record struct UserId(int Id);
~~~

### 14. 访问修饰符
#### 14.1 访问修饰符种类
>* private
>* protected
>* internal
>* public

#### 14.2 原始方式
~~~csharp
var field = SyntaxFactory.FieldDeclaration(
    SyntaxFactory.VariableDeclaration(
        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
        SyntaxFactory.SingletonSeparatedList(
            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("_id"))
        )
    ))
    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
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
~~~

#### 14.3 简化方式
~~~csharp
var field = SyntaxGenerator.IntType.Field("_id")
    .Private();
var property = SyntaxGenerator.IntType.Property("Id", 
        SyntaxKind.GetAccessorDeclaration, 
        SyntaxKind.InitAccessorDeclaration)
    .Public();
~~~

#### 14.4 生成的代码
~~~csharp
private int _id;
public int Id { get; init; }
~~~

### 15. 字段修饰符
#### 13.1 字段修饰符种类
>* readonly
>* const
>* volatile‌

#### 15.2 原始方式
~~~csharp
var field = SyntaxFactory.FieldDeclaration(
    SyntaxFactory.VariableDeclaration(
        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
        SyntaxFactory.SingletonSeparatedList(
            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("_id"))
        )
    ))
    .AddModifiers(
        SyntaxFactory.Token(SyntaxKind.PrivateKeyword), 
        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
~~~

#### 15.3 简化方式
~~~csharp
var field = SyntaxGenerator.IntType.Field("_id")
     .Private()
     .ReadOnly();
~~~

#### 15.4 生成的代码
~~~csharp
private readonly int _id;
~~~

### 16. 参数修饰符
#### 16.1 参数修饰符种类
>* params
>* in
>* ref
>* out

#### 16.2 原始方式
~~~csharp
var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
    .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
    .AddModifiers(SyntaxFactory.Token(SyntaxKind.RefKeyword));
~~~

#### 16.3 简化方式
~~~csharp
var parameter = SyntaxGenerator.StringType.Parameter("name")
    .Ref();
~~~

#### 16.4 生成的代码
~~~csharp
ref string name
~~~

### 17. 方法修饰符
#### 17.1 方法修饰符种类
>* virtual‌
>* override
>* extern‌
>* async

#### 17.2 原始方式
~~~csharp
var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), "CreateId")
    .AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword))
    .WithBody(
        SyntaxFactory.Block(SyntaxFactory.ReturnStatement(
            SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, SyntaxFactory.IdentifierName("_seed"))))
    );
~~~

#### 17.3 简化方式
~~~csharp
var method = SyntaxGenerator.IntType.Method("CreateId")
    .Virtual()
    .ToBuilder()
    .Return(SyntaxFactory.IdentifierName("_seed").PostIncrement());
~~~

#### 17.4 生成的代码
~~~csharp
virtual int CreateId()
{
    return _seed++;
}
~~~

### 18. 其他修饰符
#### 18.1 其他修饰符种类
>* abstract
>* sealed
>* new
>* static‌
>* partial


#### 18.2 原始方式
~~~csharp
var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), "CreateId")
    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword))
    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
~~~

#### 18.3 简化方式
~~~csharp
var method = SyntaxGenerator.IntType.Method("CreateId")
    .Partial()
    .WithSemicolonToken();
~~~

#### 18.4 生成的代码
~~~csharp
partial int CreateId();
~~~

## 四、方法体定义

### 1. 方法体构造器
>* 调用ToBuilder简化方法体定义

#### 1.1 支持构造函数、方法和属性
~~~csharp
var type = SyntaxFactory.ClassDeclaration("UserId");
var original = SyntaxFactory.IdentifierName("original");
var constructor = type.Constructor(SyntaxGenerator.IntType.Parameter(original.Identifier))
    .ToBuilder()
    .Add(SyntaxFactory.IdentifierName("_original").Assign(original))
    .End();

var num = SyntaxFactory.IdentifierName("num");
var value = SyntaxFactory.IdentifierName("value");
var method = SyntaxGenerator.IntType.Method("Increment", 
        SyntaxGenerator.IntType.Parameter(num.Identifier), 
        SyntaxGenerator.IntType.Parameter(value.Identifier, SyntaxGenerator.Literal(1)))
    .ToBuilder()
    .Return(num.Add(value));

var getAge = SyntaxGenerator.PropertyGetDeclaration()
    .ToBuilder()
    .Return(SyntaxFactory.IdentifierName("_age"));
var setAge = SyntaxGenerator.PropertySetDeclaration()
    .ToBuilder()
    .Add(SyntaxFactory.IdentifierName("_age").AssignValue())
    .End();
var property = SyntaxGenerator.IntType.Property(SyntaxFactory.Identifier("Age"), getAge, setAge);
~~~

#### 1.2 生成的代码
~~~csharp
UserId(int original)
{
    _original = original;
}

int Increment(int num, int value = 1)
{
    return num + value;
}


int Age
{
    get
    {
        return _age;
    }

    set
    {
        _age = value;
    }
}
~~~

### 2. if/else分支逻辑
#### 2.1 原始方式
~~~csharp
var value = SyntaxFactory.IdentifierName("value");
var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)), "BoolToString")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(value.Identifier)
            .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)))))
    .AddBodyStatements(
        SyntaxFactory.IfStatement(
            SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, value, SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
            SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("false"))),
            SyntaxFactory.ElseClause(
                SyntaxFactory.IfStatement(
                value,
                SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("true"))),
                SyntaxFactory.ElseClause(SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("false")))))))
    );
~~~

#### 2.2 简化方式
~~~csharp
var value = SyntaxFactory.IdentifierName("value");
var method = SyntaxGenerator.StringType.Method("BoolToString", SyntaxGenerator.BoolType.Nullable().Parameter(value.Identifier))
    .ToBuilder()
    .If(value.IsNull())
        .Add(SyntaxGenerator.Literal("false").Return())
    .ElseIf(value)
        .Return(SyntaxGenerator.Literal("true"))
    .Return(SyntaxGenerator.Literal("false"));
~~~

#### 2.3 生成的代码
~~~csharp
string BoolToString(bool? value)
{
    if (value == null)
        return "false";
    else if (value)
        return "true";
    return "false";
}
~~~

### 3. switch/case分支逻辑
#### 3.1 原始方式
~~~csharp
var value = SyntaxFactory.IdentifierName("value");
var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)), "IntToBool")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(value.Identifier)
            .WithType(SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))))
    .AddBodyStatements(
        SyntaxFactory.SwitchStatement(value)
            .AddSections(
                SyntaxFactory.SwitchSection(
                    SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.CaseSwitchLabel(
                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0)))),
                    SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)))),
                SyntaxFactory.SwitchSection(
                    SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.CaseSwitchLabel(
                        SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)))),
                    SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))),
                SyntaxFactory.SwitchSection(
                    SyntaxFactory.SingletonList<SwitchLabelSyntax>(SyntaxFactory.DefaultSwitchLabel()),
                    SyntaxFactory.SingletonList<StatementSyntax>(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))))
    );
~~~

#### 3.2 简化方式
~~~csharp
var value = SyntaxFactory.IdentifierName("value");
var method = SyntaxGenerator.BoolType.Method("IntToBool", SyntaxGenerator.IntType.Parameter(value.Identifier))
    .ToBuilder()
    .Switch(value)
        .Case(SyntaxGenerator.Literal(0))
            .Add(SyntaxGenerator.FalseLiteral.Return())
        .Case(SyntaxGenerator.Literal(1))
            .Add(SyntaxGenerator.TrueLiteral.Return())
        .Default()
            .Return(SyntaxGenerator.TrueLiteral)
    .End();
~~~

#### 3.3 生成的代码
~~~csharp
bool IntToBool(int value)
{
    switch (value)
    {
        case 0:
            return false;
        case 1:
            return true;
        default:
            return true;
    }
}
~~~

### 4. foreach循环
#### 4.1 原始方式
~~~csharp
var list = SyntaxFactory.IdentifierName("list");
var item = SyntaxFactory.IdentifierName("item");
var count = SyntaxFactory.IdentifierName("count");
var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "Count")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(list.Identifier)
            .WithType(SyntaxFactory.ArrayType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))))
    .AddBodyStatements(
        SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
            SyntaxFactory.IdentifierName("var"), SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.VariableDeclarator(count.Identifier)
                    .WithInitializer(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                        SyntaxFactory.Literal(0)))))),
        SyntaxFactory.ForEachStatement(
            SyntaxFactory.IdentifierName("var"), 
            item.Identifier, 
            list, 
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(SyntaxKind.AddAssignmentExpression, count, item))),
        SyntaxFactory.ReturnStatement(count)
    );
~~~

#### 4.2 简化方式
~~~csharp
var list = SyntaxFactory.IdentifierName("list");
var item = SyntaxFactory.IdentifierName("item");
var count = SyntaxFactory.IdentifierName("count");
var method = SyntaxGenerator.IntType.Method("Count", SyntaxGenerator.IntType.Array().Parameter(list.Identifier))
    .ToBuilder()
    .Declare(SyntaxGenerator.VarType.Variable(count.Identifier, SyntaxGenerator.Literal(0)))
    .ForEach(SyntaxGenerator.VarType, item.Identifier, list)
        .Add(count.AddAssign(item))
    .End()
    .Return(count);
~~~

#### 4.3 生成的代码
~~~csharp
int Count(int[] list)
{
    var count = 0;
    foreach (var item in list)
        count += item;
    return count;
}
~~~

### 5. for循环
#### 5.1 原始方式
~~~csharp
var i = SyntaxFactory.IdentifierName("i");
var num = SyntaxFactory.IdentifierName("num");
var count = SyntaxFactory.IdentifierName("count");
var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), "Total")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(num.Identifier)
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
    .AddBodyStatements(
        SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
            SyntaxFactory.IdentifierName("var"), SyntaxFactory.SingletonSeparatedList(
                SyntaxFactory.VariableDeclarator(count.Identifier)
                    .WithInitializer(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                        SyntaxFactory.Literal(0)))))),
        SyntaxFactory.ForStatement(
            SyntaxFactory.VariableDeclaration(
                SyntaxFactory.IdentifierName("var")).AddVariables(
                    SyntaxFactory.VariableDeclarator(i.Identifier)
                        .WithInitializer(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(0)))),
            default,
            SyntaxFactory.BinaryExpression(SyntaxKind.LessThanExpression, i, num),
            SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, i)),
            SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.AddAssignmentExpression, count, i))),
        SyntaxFactory.ReturnStatement(count)
    );
~~~

#### 5.2 简化方式
~~~csharp
var i = SyntaxFactory.IdentifierName("i");
var num = SyntaxFactory.IdentifierName("num");
var count = SyntaxFactory.IdentifierName("count");
var method = SyntaxGenerator.IntType.Method("Total", SyntaxGenerator.IntType.Parameter(num.Identifier))
    .ToBuilder()
    .Declare(SyntaxGenerator.IntType.Variable(count.Identifier, SyntaxGenerator.Literal(0)))
    .For(i, num)
        .Add(count.AddAssign(i))
    .End()
    .Return(count);
~~~

#### 5.3 生成的代码
~~~csharp
int Total(int num)
{
    int count = 0;
    for (var i = 0; i < num; i++)
        count += i;
    return count;
}
~~~

### 6. while循环
#### 6.1 原始方式
~~~csharp
var readerType = SyntaxFactory.IdentifierName("DbDataReader");
var reader = SyntaxFactory.IdentifierName("reader");
var listType = SyntaxFactory.GenericName("List")
    .AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));
var list = SyntaxFactory.IdentifierName("list");
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
~~~

#### 6.2 简化方式
~~~csharp
 var readerType = SyntaxFactory.IdentifierName("DbDataReader");        
 var reader = SyntaxFactory.IdentifierName("reader");
 var listType = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
 var list = SyntaxFactory.IdentifierName("list");
 var getFieldValue = SyntaxGenerator.Generic("GetFieldValue", SyntaxGenerator.IntType).Qualified(reader);
 var method = listType.Method("GetIds", readerType.Parameter(reader.Identifier))
     .ToBuilder()
     .Declare(listType.Variable(list.Identifier, SyntaxFactory.CollectionExpression()))
     .While(reader.Access("Read").Invocation())
         .Add(list.Access("Add").Invocation([getFieldValue.Invocation([SyntaxGenerator.Literal(0)])]))
     .End()
     .Return(list);
~~~

#### 6.3 生成的代码
~~~csharp
List<int> GetIds(DbDataReader reader)
{
    List<int> list = [];
    while (reader.Read())
        list.Add(reader.GetFieldValue<int>(0));
    return list;
}
~~~

### 6. do/while循环
#### 6.1 原始方式
~~~csharp
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
~~~

#### 6.2 简化方式
~~~csharp
var console = SyntaxFactory.IdentifierName("Console");
var writeLine = console.Access("WriteLine");
var thing = SyntaxFactory.IdentifierName("thing");
var method = SyntaxGenerator.VoidType.Method("DoSTh")
    .ToBuilder()
    .Declare(SyntaxGenerator.StringType.Nullable().Variable(thing.Identifier))
    .Add(writeLine.Invocation([SyntaxGenerator.Literal("Enter some things:")]))
    .Do(thing.NotEqual(SyntaxGenerator.Literal("exit")))
        .Add(thing.Assign(console.Access("ReadLine").Invocation()))
        .Add(console.Access("Write").Invocation([SyntaxGenerator.Literal("Do ")]))
        .Add(writeLine.Invocation([thing]))
        .End()
    .End();
~~~

#### 6.3 生成的代码
~~~csharp
void DoSTh()
{
    string? thing;
    Console.WriteLine("Enter some things:");
    do
    {
        thing = Console.ReadLine();
        Console.Write("Do ");
        Console.WriteLine(thing);
    }
    while (thing != "exit");
}
~~~

## 五、生成类完整的Case
>* SyntaxGenerator.Create用于组装类的成员

### 1. 简化方式
~~~csharp
var type = SyntaxFactory.ClassDeclaration("UserId")
    .Public()
    .Partial();
var original = SyntaxFactory.IdentifierName("original");
var _original = SyntaxFactory.IdentifierName("_original");
var field = SyntaxGenerator.IntType.Field(_original.Identifier)
    .Private()
    .ReadOnly();
var property = SyntaxGenerator.IntType.GetOnlyProperty("Original", _original)
    .Public();
var constructor = type.Constructor(SyntaxGenerator.IntType.Parameter(original.Identifier))
    .Public()
    .ToBuilder()
    .Add(_original.Assign(original))
    .End();

var builder = SyntaxGenerator.Create("Models", type, constructor, field, property);
var result = builder.Build();
~~~

### 2 生成的代码
~~~csharp
// <auto-generated/>
namespace Models
{
    public partial class UserId
    {
        public UserId(int original)
        {
            _original = original;
        }

        private readonly int _original;
        public int Original
        {
            get
            {
                return _original;
            }
        }
    }
}
~~~

## 六、总结
>* SyntaxFactory是个很强大的工具
>* 可以生成几乎所有的代码
>* 只是树形结构细节太多影响代码审阅
>* 为此笔者开源项目对部分SyntaxFactory进行简化
>* dotnet add package Hand.Generators.EasySyntax --version 0.1.0.1-alpha

源码托管地址: https://github.com/donetsoftwork/Hand.Generators ，欢迎大家直接查看源码。
gitee同步更新:https://gitee.com/donetsoftwork/hand.-generators

如果大家喜欢请动动您发财的小手手帮忙点一下Star,谢谢！！！