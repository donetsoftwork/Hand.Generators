# EasySyntax定义类的7种写法与SG实现影子编程

EasySyntax是使用SyntaxTree生成代码的简化工具
具有很强的表达能力
本文展示用EasySyntax生成类的7种写法

# 一、定义类的7种写法
## 1. 传统样式
### 1.1 EasySyntax语法
```csharp
var Id = SyntaxGenerator.IntType.GetSetProperty("Id")
    .Public();
var Name = SyntaxGenerator.StringType.GetSetProperty("Name")
    .Public();
var User = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddMembers(Id, Name);
```

### 1.2 生成的代码如下
```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### 1.3 增加默认值的写法
>* 增加默认值后需要增加分号，否则存在语法错误
>* 通过WithInitializer方法增加默认值，通过WithSemicolonToken方法增加分号

```csharp
var Id = SyntaxGenerator.IntType.GetSetProperty("Id")            
    .Public()
    .WithInitializer(SyntaxGenerator.Literal(1))
    .WithSemicolonToken();
var Name = SyntaxGenerator.StringType.GetSetProperty("Name")            
    .Public()
    .WithInitializer(SyntaxGenerator.Literal(""))
    .WithSemicolonToken();
var User = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddMembers(Id, Name);
```

### 1.4 增加默认值生成的代码如下
```csharp
public class User
{
    public int Id { get; set; } = 1;
    public string Name { get; set; } = "";
}
```

## 2. 使用Init属性
### 2.1 EasySyntax语法
```csharp
var Id = SyntaxGenerator.IntType.GetInitProperty("Id")
    .Public();
var Name = SyntaxGenerator.StringType.GetInitProperty("Name")
    .Public();
var User = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddMembers(Id, Name);
```

### 2.2 生成的代码如下
```csharp
public class User
{
    public int Id { get; init; }
    public string Name { get; init; }
}
```

### 2.3 增加默认值的写法
```csharp
var Id = SyntaxGenerator.IntType.GetInitProperty("Id")
    .Public()
    .WithInitializer(SyntaxGenerator.Literal(1))
    .WithSemicolonToken();
var Name = SyntaxGenerator.StringType.GetInitProperty("Name")
    .Public()
    .WithInitializer(SyntaxGenerator.Literal(""))
    .WithSemicolonToken();
var User = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddMembers(Id, Name);
```

### 2.4 增加默认值生成的代码如下
```csharp
public class User
{
    public int Id { get; init; } = 1;
    public string Name { get; init; } = "";
}
```

## 3. 使用字段
### 3.1 EasySyntax语法
```csharp
var _id = SyntaxGenerator.IntType.Field("_id")
    .Private();
var Id = SyntaxGenerator.IntType.GetSetProperty("Id", _id.ToIdentifierName())
    .Public();
var _name = SyntaxGenerator.StringType.Field("_name")
    .Private();
var Name = SyntaxGenerator.StringType.GetSetProperty("Name", _name.ToIdentifierName())
    .Public();
var User = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddMembers(_id, _name, Id, Name);
```

### 3.2 生成的代码如下
```csharp
public class User
{
    private int _id;
    public int Id { get => _id; set => _id = value; }
    private string _name;
    public string Name { get => _name; set => _name = value; }
}
```

### 3.3 字段增加默认值的写法
```csharp
var _id = SyntaxGenerator.IntType.Field("_id", SyntaxGenerator.Literal(1))
    .Private();
var Id = SyntaxGenerator.IntType.GetSetProperty("Id", _id.ToIdentifierName())
    .Public();
var _name = SyntaxGenerator.StringType.Field("_name", SyntaxGenerator.Literal(""))
    .Private();
var Name = SyntaxGenerator.StringType.GetSetProperty("Name", _name.ToIdentifierName())
    .Public();
var User = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddMembers(_id, _name, Id, Name);
```

### 3.4 字段增加默认值生成的代码如下
```csharp
public class User
{
    private int _id = 1;
    public int Id { get => _id; set => _id = value; }
    private string _name = "";
    public string Name { get => _name; set => _name = value; }
}
```

## 4. 使用构造函数
### 4.1 EasySyntax语法
```csharp
public class User
{
    public User(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }
}
```

### 4.2 生成的代码如下
```csharp
public class User
{
    public User(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public int Id { get; }
    public string Name { get; }
}
```

### 4.3 构造函数和字段
```csharp
var id = SyntaxFactory.IdentifierName("id");
var _id = SyntaxFactory.IdentifierName("_id");
var _idField = SyntaxGenerator.IntType.Field(_id.Identifier)
    .Private()
    .ReadOnly();
var Id = SyntaxFactory.IdentifierName("Id");
var IdProperty = SyntaxGenerator.IntType.Property(Id.Identifier, _id)
    .Public();
var name = SyntaxFactory.IdentifierName("name");
var _name = SyntaxFactory.IdentifierName("_name");
var _nameField = SyntaxGenerator.StringType.Field(_name.Identifier)
    .Private()
    .ReadOnly();
var Name = SyntaxFactory.IdentifierName("Name");
var NameProperty = SyntaxGenerator.StringType.Property(Name.Identifier, _name)
    .Public();
var User = SyntaxFactory.Identifier("User");
var constructor = SyntaxGenerator.ConstructorDeclaration(
        User,
        SyntaxGenerator.IntType.Parameter(id.Identifier),
        SyntaxGenerator.StringType.Parameter(name.Identifier))
    .Public()
    .ToBuilder()
        .Add(_id.Assign(id))
        .Add(_name.Assign(name))
    .End();
var type = SyntaxFactory.ClassDeclaration("User")
     .Public()
     .AddMembers(constructor, _idField, _nameField, IdProperty, NameProperty);
```

### 4.4 构造函数和字段生成的代码如下
```csharp
public class User
{
    public User(int id, string name)
    {
        _id = id;
        _name = name;
    }
    private readonly int _id;
    private readonly string _name;
    public int Id => _id;
    public string Name => _name;
}
```

## 5. 使用主构造函数
### 5.1 EasySyntax语法
```csharp
var id = SyntaxFactory.IdentifierName("id");
var Id = SyntaxGenerator.IntType.GetOnlyProperty("Id")            
    .Public()
    .WithInitializer(id)
    .WithSemicolonToken();
var name =SyntaxFactory.IdentifierName("name");
var Name = SyntaxGenerator.StringType.GetOnlyProperty("Name")
    .Public()
    .WithInitializer(name)            
    .WithSemicolonToken();
var type = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddParameterListParameters(
        SyntaxGenerator.IntType.Parameter(id.Identifier),
        SyntaxGenerator.StringType.Parameter(name.Identifier))
    .AddMembers(Id, Name);
```

### 5.2 生成的代码如下
```csharp
public class User(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
```

### 5.3 主构造函数和字段
```csharp
var id = SyntaxFactory.IdentifierName("id");
var _id = SyntaxGenerator.IntType.Field("_id", id)
    .Private()
    .ReadOnly();
var Id = SyntaxGenerator.IntType.Property("Id", _id.ToIdentifierName())
    .Public();
var name = SyntaxFactory.IdentifierName("name");
var _name = SyntaxGenerator.StringType.Field("_name", name)
    .Private()
    .ReadOnly();
var Name = SyntaxGenerator.StringType.Property("Name", _name.ToIdentifierName())
    .Public();
var User = SyntaxFactory.ClassDeclaration("User")
    .Public()
    .AddParameterListParameters(
        SyntaxGenerator.IntType.Parameter(id.Identifier),
        SyntaxGenerator.StringType.Parameter(name.Identifier))
    .AddMembers(_id, _name, Id, Name);
```

### 5.4 主构造函数和字段生成的代码如下
```csharp
public class User(int id, string name)
{
    private readonly int _id = id;
    private readonly string _name = name;
    public int Id => _id;
    public string Name => _name;
}
```

## 6. 使用record
### 6.1 EasySyntax语法
```csharp
var type = SyntaxGenerator.RecordDeclaration("User")
    .Public()
    .AddParameterListParameters(
        SyntaxGenerator.IntType.Parameter("Id"),
        SyntaxGenerator.StringType.Parameter("Name"))
    .WithSemicolonToken();
```

## # 6.2 生成的代码如下
```csharp
public record User(int Id, string Name);
```

### 6.3 record和参数默认值
```csharp
var type = SyntaxGenerator.RecordDeclaration("User")
    .Public()
    .AddParameterListParameters(
        SyntaxGenerator.IntType.Parameter("Id", SyntaxGenerator.Literal(1)),
        SyntaxGenerator.StringType.Parameter("Name", SyntaxGenerator.Literal("")))
    .WithSemicolonToken();
```

### 6.4 record和参数默认值生成的代码如下
```csharp
public record User(int Id = 1, string Name = "");
```

## 7. 使用record struct
### 7.1 EasySyntax语法
```csharp
var type = SyntaxGenerator.RecordStructDeclaration("User")
    .Public()
    .AddParameterListParameters(
        SyntaxGenerator.IntType.Parameter("Id"),
        SyntaxGenerator.StringType.Parameter("Name"))
    .WithSemicolonToken();
```

#### 7.2 生成的代码如下
```csharp
public record struct User(int Id, string Name);
```

### 7.3 record struct和参数默认值
```csharp
var type = SyntaxGenerator.RecordStructDeclaration("User")
    .Public()
    .AddParameterListParameters(
        SyntaxGenerator.IntType.Parameter("Id", SyntaxGenerator.Literal(1)),
        SyntaxGenerator.StringType.Parameter("Name", SyntaxGenerator.Literal("")))
    .WithSemicolonToken();
```

### 7.4 record struct和参数默认值生成的代码如下
```csharp
public record struct User(int Id = 1, string Name = "");
```



