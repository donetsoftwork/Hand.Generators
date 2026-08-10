# .net源生成器必须知道的4套类型
>* 熟悉.net的4套类型系统才能更好的掌握源生成器技巧,对普通.net开发理解类型也是助益良多,这里藏着我们写的代码的“前世、今生和未来”

## 前言
>* .net有4套类型
>* 每一套类型有各自的作用
>* 部分类型之间可以相互转化

## 1. TypeSyntax
>* 全称Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax
>* 用于在SyntaxTree中引用类型

### 1.1 TypeSyntax常用子类
>* PredefinedTypeSyntax
>* IdentifierNameSyntax
>* QualifiedNameSyntax
>* GenericNameSyntax
>* ArrayTypeSyntax
>* NullableTypeSyntax
>* OmittedTypeArgumentSyntax

### 1.2 PredefinedTypeSyntax
>* 系统预定义类型

#### 1.2.1 预定义类型有16种
>* 由以下16种SyntaxKind定义预定义类型
>* SyntaxKind.BoolKeyword
>* SyntaxKind.ByteKeyword
>* SyntaxKind.SByteKeyword
>* SyntaxKind.IntKeyword
>* SyntaxKind.UIntKeyword
>* SyntaxKind.ShortKeyword
>* SyntaxKind.UShortKeyword
>* SyntaxKind.LongKeyword
>* SyntaxKind.ULongKeyword
>* SyntaxKind.FloatKeyword
>* SyntaxKind.DoubleKeyword
>* SyntaxKind.DecimalKeyword
>* SyntaxKind.StringKeyword
>* SyntaxKind.CharKeyword
>* SyntaxKind.ObjectKeyword
>* SyntaxKind.VoidKeyword

#### 1.2.2 EasySyntax语法
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

#### 1.2.3 Roslyn原始语法
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

### 1.3 IdentifierNameSyntax
>* 标识名,这里作为类型标识名

#### 1.3.1 简单类名
>* SyntaxFactory.IdentifierName("User")

#### 1.3.2 含命名空间类名
>* SyntaxFactory.IdentifierName("Models.User")
>* 用于指定命名空间

#### 1.3.3 含全局限定类名
>* SyntaxFactory.IdentifierName("global::Models.User")
>* 用于排除命名空间不明确
>* 比如在A、B、C三个命名空间下,Models.User的4种分别是(A.Models.User、B.Models.User、C.Models.User和Models.User)
>* 使用global::Models.User就没有歧义


### 1.4 QualifiedNameSyntax
>* 前缀类型(含命名空间)

#### 1.4.1 EasySyntax语法
>* SyntaxFactory.IdentifierName("User").Qualified("Models");

#### 1.4.2 Roslyn原始语法
>* SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("Models"), "User")

### 1.5 GenericNameSyntax
#### 1.5.1 EasySyntax语法
>* SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);

#### 1.5.2 Roslyn原始语法
>* SyntaxFactory.GenericName(SyntaxFactory.Identifier("List"), SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList([SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))])))

### 1.6 ArrayTypeSyntax
>* 数组类型

#### 1.6.1 EasySyntax语法
>* SyntaxGenerator.IntType.Array();

#### 1.6.2 Roslyn原始语法
>* SyntaxFactory.ArrayType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)), SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression()))))

### 1.7 NullableTypeSyntax
>* 可空类型

#### 1.7.1 EasySyntax语法
>* SyntaxFactory.NullableType(SyntaxGenerator.IntType);

#### 1.7.2 Roslyn原始语法
>* SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)));

### 1.8 OmittedTypeArgumentSyntax
>* 泛型匿名类型参数
>* 使用方法SyntaxFactory.OmittedTypeArgument()创建
>* 需要结合泛型来使用

~~~csharp
var listIntType = SyntaxGenerator.Generic("List", SyntaxFactory.OmittedTypeArgument());
var code = listIntType.ToFullString();
Assert.Equal("List<>", code);
~~~

### 1.9 TypeSyntax的用法灵活
>* 同个功能有多种表示方法,甚至可以用不用子类来表示
>* 当然还可以用SyntaxFactory.ParseTypeName
>* 详情参看[.NET源码生成器之SyntaxTree踩坑](https://www.cnblogs.com/xiangji/p/19695022)

## 2. TypeDeclarationSyntax
>* 全称Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax
>* 用于定义类型

### 2.1 TypeDeclarationSyntax常用子类
>* ClassDeclarationSyntax
>* StructDeclarationSyntax
>* InterfaceDeclarationSyntax
>* RecordDeclarationSyntax
>* ExtensionBlockDeclarationSyntax

### 2.2 ClassDeclarationSyntax
>* 定义类
>* SyntaxFactory.ClassDeclaration("User")

### 2.3 StructDeclarationSyntax
>* 定义结构体
>* SyntaxFactory.StructDeclaration("User")

### 2.3 InterfaceDeclarationSyntax
>* 定义接口
>* SyntaxFactory.InterfaceDeclaration("IEntityId")

### 2.4 RecordDeclarationSyntax
>* 定义记录

#### 2.4.1 EasySyntax语法

~~~csharp
var record = SyntaxGenerator.RecordDeclaration("Person")
    .AddParameterListParameters(
        SyntaxGenerator.StringType.Parameter("Name")
    )
    .WithSemicolonToken();
~~~

#### 2.4.2 Roslyn原始语法
~~~csharp
var record = SyntaxFactory.RecordDeclaration(SyntaxFactory.Token(SyntaxKind.RecordKeyword), "Person")
    .AddParameterListParameters(
        SyntaxFactory.Parameter(SyntaxFactory.Identifier("Name"))
            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
    )
    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
~~~

### 2.5 ExtensionBlockDeclarationSyntax
>* 扩展块,是csharp14新增扩展方法语法
>* 使用SyntaxFactory.ExtensionBlockDeclaration创建

### 2.5.1 ExtensionBlockDeclarationSyntax示例
~~~csharp
var userIdType = SyntaxFactory.IdentifierName("UserId");
var userId = SyntaxFactory.IdentifierName("userId");
var incrementMethod = userIdType.Method("Increment")
    .ToBuilder()
    .Return(userIdType.New([userId.Access("Original").Add(SyntaxGenerator.Literal(1))]))
    .Public();
var decreaseMethod = userIdType.Method("Decrease")
    .ToBuilder()
    .Return(userIdType.New([userId.Access("Original").Subtract(SyntaxGenerator.Literal(1))]))
    .Public();

var extension = SyntaxFactory.ExtensionBlockDeclaration()
    .AddParameterListParameters(userIdType.Parameter(userId.Identifier))
    .AddMembers(incrementMethod, decreaseMethod)
    .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
    .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));
var type = SyntaxFactory.ClassDeclaration("UserIdExtension")
    .Public()
    .Static()
    .AddMembers(extension);
~~~

### 2.5.2 ExtensionBlockDeclarationSyntax示例生成代码
>* 静态类型UserIdExtension中增加一个扩展块
>* 扩展块中用实例方法定义扩展方法

~~~csharp
public static class UserIdExtension
{
    extension(UserId userId)
    {
        public UserId Increment()
        {
            return new UserId(userId.Original + 1);
        }

        public UserId Decrease()
        {
            return new UserId(userId.Original - 1);
        }
    }
}
~~~

## 3. ITypeSymbol
>* 全称Microsoft.CodeAnalysis.ITypeSymbol
>* 用于描述类型的编译符号的接口
>* 特别注意,ITypeSymbol和反射的Type还不是一回事

### 3.1 46种SpecialType
>* SpecialType与前面的预定义类型类似,就是系统类
>* 通过以下46种枚举SpecialType来定义
>* SpecialType.System_Runtime_CompilerServices_InlineArrayAttribute
>* SpecialType.System_Object
>* SpecialType.System_Enum
>* SpecialType.System_MulticastDelegate
>* SpecialType.System_Delegate
>* SpecialType.System_ValueType
>* SpecialType.System_Void
>* SpecialType.System_Boolean
>* SpecialType.System_Char
>* SpecialType.System_SByte
>* SpecialType.System_Byte
>* SpecialType.System_Int16
>* SpecialType.System_UInt16
>* SpecialType.System_Int32
>* SpecialType.System_UInt32
>* SpecialType.System_Int64
>* SpecialType.System_UInt64
>* SpecialType.System_Decimal
>* SpecialType.System_Single
>* SpecialType.System_Double
>* SpecialType.System_String
>* SpecialType.System_IntPtr
>* SpecialType.System_UIntPtr
>* SpecialType.System_Array
>* SpecialType.System_Collections_IEnumerable
>* SpecialType.System_Collections_Generic_IEnumerable_T
>* SpecialType.System_Collections_Generic_IList_T
>* SpecialType.System_Collections_Generic_ICollection_T
>* SpecialType.System_Collections_IEnumerator
>* SpecialType.System_Collections_Generic_IEnumerator_T
>* SpecialType.System_Collections_Generic_IReadOnlyList_T
>* SpecialType.System_Collections_Generic_IReadOnlyCollection_T
>* SpecialType.System_Nullable_T
>* SpecialType.System_DateTime
>* SpecialType.System_Runtime_CompilerServices_IsVolatile
>* SpecialType.System_IDisposable
>* SpecialType.System_TypedReference
>* SpecialType.System_ArgIterator
>* SpecialType.System_RuntimeArgumentHandle
>* SpecialType.System_RuntimeFieldHandle
>* SpecialType.System_RuntimeMethodHandle
>* SpecialType.System_RuntimeTypeHandle
>* SpecialType.System_IAsyncResult
>* SpecialType.System_AsyncCallback
>* SpecialType.System_Runtime_CompilerServices_RuntimeFeature
>* SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute

### 3.2 获取系统类
>* 通过Compilation.GetSpecialType获取系统类

~~~csharp
INamedTypeSymbol GetSpecialType(SpecialType specialType);
~~~

### 3.3 按类型名获取
>* 通过Compilation.GetTypeByMetadataName获取类

~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var source = @"namespace Tests;
    public enum MyColor : int
    {
        Red,
        Green,
        Blue
    };
    var color = MyColor.Red;";
var compilation = service.Compile(source);
var enumType = compilation.GetTypeByMetadataName("Tests.MyColor");
Assert.NotNull(enumType);
~~~

### 3.4 泛型
>* 通过Construct方法构造泛型

~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var intType = compilation.GetSpecialType(SpecialType.System_Int32);
// System.Collections.Generic.IList<int>
var genericType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T)
    .Construct(intType);
~~~

## 4. Type
>* 全称System.Type
>* 用于反射类型信息


### 4.1 反射获取Type
~~~csharp
Type type = typeof(User);
~~~

## 5. 4套类型的生命周期和转化流程
>* 第一阶段是语法树,存在TypeSyntax和TypeDeclarationSyntax
>* 第二阶段是编译,转化为ITypeSymbol
>* 第三阶段是链接符号生成程序集,转化为Type

## 6. TypeDeclarationSyntax转ITypeSymbol
>* 编译含TypeDeclarationSyntax的SyntaxTree
>* 从编译信息中获取INamedTypeSymbol
>* 以下示例使用CSharpCompilation.Create调用编译与日常用IDE编译是一回事
>* 这种场景适用源生成器的单元测试

~~~csharp
// record User(string Name);
var declaration = SyntaxGenerator.RecordDeclaration("User")
    .AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
var unit = SyntaxFactory.CompilationUnit()
    .AddMembers(declaration);
var compilation = CSharpCompilation.Create("Tests", [unit.SyntaxTree]);
INamedTypeSymbol? userSymbol = compilation.GetTypeByMetadataName("User");
Assert.NotNull(userSymbol);
~~~

## 7. TypeSyntax转ITypeSymbol
>* 用于按TypeSyntax查找原已经存在的类型符号
>* 也可用于单元测试

### 7.1 NullableTypeSyntax转化为SpecialType.System_Nullable_T的实现类
~~~csharp
NullableTypeSyntax nullableSyntax = SyntaxFactory.NullableType(SyntaxGenerator.IntType);
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
INamedTypeSymbol? symbol= compilation.GetSymbol(nullableSyntax);
Assert.NotNull(symbol);
var nullableSymbol = compilation.GetSpecialType(SpecialType.System_Nullable_T);
Assert.True(symbol.IsGenericType(nullableSymbol));
~~~

### 7.2 PredefinedTypeSyntax转化为系统类型
>* 16种PredefinedTypeSyntax都能转化为对应的ITypeSymbol的系统类型
>* 使用Compilation.GetSymbol扩展方法转化

~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var intType = SyntaxGenerator.IntType;
INamedTypeSymbol symbol = compilation.GetSymbol(intType);
Assert.Equal(SpecialType.System_Int32, symbol.SpecialType);
~~~

### 7.3 ArrayTypeSyntax转化为数组类型
>* 使用Compilation.GetSymbol扩展方法转化

~~~csharp
ArrayTypeSyntax arraySyntax = SyntaxGenerator.IntType.Array();
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
IArrayTypeSymbol? arraySymbol = compilation.GetSymbol(arraySyntax);
Assert.NotNull(arraySymbol);
~~~

### 7.4 GenericNameSyntax转化为泛型
>* GenericNameSyntax或含GenericNameSyntax的QualifiedNameSyntax转化为泛型符号

~~~csharp
GenericNameSyntax genericSyntax = SyntaxGenerator.Generic("System.Collections.Generic.IList", SyntaxGenerator.IntType);
var service = SyntaxTreeDriver.CreateDriver()
     .Reference<object>();
var compilation = service.Compile("");
if(compilation.GetSymbol(genericSyntax) is not INamedTypeSymbol genericSymbol)
{
    Assert.Fail();
    return;
}
var listSymbol = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T);
Assert.True(genericSymbol.IsGenericType(listSymbol));
~~~

## 8. ITypeSymbol转Type
>* Compilation.Emit把Compilation转化为Assembly,把ITypeSymbol转化为Type
>* 以下示例先把TypeDeclarationSyntax转ITypeSymbol,再把ITypeSymbol转化为Type
>* 其中SyntaxTreeDriver封装了CSharpCompilation,方便添加引用
>* 其中ScriptLoadContext封装了Compilation.Emit操作
>* 这种场景适用源生成器的单元测试
>* 也可用于开发插件系统

~~~csharp
// record User(string Name);
var declaration = SyntaxGenerator.RecordDeclaration("User")
    .AddParameterListParameters(SyntaxGenerator.StringType.Parameter("Name"));
var unit = SyntaxFactory.CompilationUnit()
    .AddMembers(declaration);
var driver = SyntaxTreeDriver.CreateDriver()
    .Reference<object>();
var compilation = driver.Compile(unit.SyntaxTree);
INamedTypeSymbol? userSymbol = compilation.GetTypeByMetadataName("User");
Assert.NotNull(userSymbol);
using var context = new ScriptLoadContext();
var assembly = context.GetAssembly(compilation);
Type? type = assembly.GetType("User");
Assert.NotNull(type);
~~~

## 9. ITypeSymbol转TypeSyntax
>* 用于按原类型符号生成TypeSyntax
>* 以便生成新代码与原代码高度兼容

### 9.1 EasySyntax语法默认转化方法
>* 调用EasySyntax语法的扩展方法ToSyntax转化

#### 9.1.1 可空类型转化为NullableTypeSyntax
~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var intType = compilation.GetSpecialType(SpecialType.System_Int32);
var nullable = compilation.GetSpecialType(SpecialType.System_Nullable_T)
    .Construct(intType);
if (nullable.ToSyntax() is not NullableTypeSyntax syntax)
{
    Assert.Fail();
    return;
}
var code = syntax.NormalizeWhitespace().ToFullString();
Assert.Equal("int?", code);
~~~

#### 9.1.2 预定义类型转化为PredefinedTypeSyntax
~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var intType = compilation.GetSpecialType(SpecialType.System_Int32);
if(intType.ToSyntax() is not PredefinedTypeSyntax syntax)
{
    Assert.Fail();
    return;
}
var code = syntax.NormalizeWhitespace().ToFullString();
Assert.Equal("int", code);
~~~

#### 9.1.3 数组转化为ArrayTypeSyntax
~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var intType = compilation.GetSpecialType(SpecialType.System_Int32);
var arrayType = compilation.CreateArrayTypeSymbol(intType, 1);
if (arrayType.ToSyntax() is not ArrayTypeSyntax syntax)
{
    Assert.Fail();
    return;
}
var code = syntax.NormalizeWhitespace().ToFullString();
Assert.Equal("int[]", code);
~~~

#### 9.1.4 泛型转化为GenericNameSyntax
>* 泛型转化为GenericNameSyntax或含GenericNameSyntax的QualifiedNameSyntax

~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var intType = compilation.GetSpecialType(SpecialType.System_Int32);
var genericType = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T)
    .Construct(intType);
var syntax = genericType.ToSyntax();
if (syntax is not GenericNameSyntax
    && (syntax is not QualifiedNameSyntax qualifiedSyntax || qualifiedSyntax.Right is not GenericNameSyntax))
{
    Assert.Fail();
    return;
}
var code = syntax.NormalizeWhitespace().ToFullString();
Assert.Equal("System.Collections.Generic.IList<int>", code);
~~~

### 9.2 确人在当前命名空间下使用类名即可
>* 使调用EasySyntax语法的扩展方法ToMiniName

~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var dateTimeType = compilation.GetSpecialType(SpecialType.System_DateTime);
var syntax = dateTimeType.ToMiniName();
var code = syntax.NormalizeWhitespace().ToFullString();
Assert.Equal("DateTime", code);
~~~

### 9.3 转化为含全局限定类名
>* 调用EasySyntax语法的扩展方法ToGlobalName

~~~csharp
var service = SyntaxTreeDriver.CreateDriver();
var compilation = service.Compile("");
var dateTimeType = compilation.GetSpecialType(SpecialType.System_DateTime);
var syntax = dateTimeType.ToGlobalName();
var code = syntax.NormalizeWhitespace().ToFullString();
Assert.Equal("global::System.DateTime", code);
~~~

## 10. 结语
>* 4套类型的生命周期就是我们代码的“前世、今生和未来”
>* 熟悉这些能更好的理解代码工作的机制
>* 开发.net源生成器这些更是必须掌握的技能
>* 以上例子由Roslyn及其简单封装来支持的
>* Roslyn主要组件: Microsoft.CodeAnalysis.CSharp、Microsoft.CodeAnalysis.CSharp.Scripting
>* 封装组件可以用于.net源生成器的开发: 
>* dotnet add package Hand.Generators.EasySyntax --version 0.2.1.3
>* dotnet add package Hand.GenerateCore --version 0.2.1.5
>* dotnet add package Hand.Generators.SyntaxScripting --version 0.2.1.3-alpha


源码托管地址: https://github.com/donetsoftwork/Hand.Generators ，欢迎大家直接查看源码。
gitee同步更新:https://gitee.com/donetsoftwork/hand.-generators

如果大家喜欢请动动您发财的小手手帮忙点一下Star,谢谢！！！
