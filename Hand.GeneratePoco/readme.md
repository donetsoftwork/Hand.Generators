# 生成类型成员及转化器

## 1 GeneratePoco生成构造函数
## 1 生成构造函数默认样式
### 1.1 配置代码
>* Initializer配置为InitializeKind.Constructor

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor)]
public partial class UserDto;
~~~

### 1.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public User ToUser() => new(Id, Name);
}
~~~

## 2 生成构造函数含字段样式
### 2.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Field)]
public partial class UserDto;
~~~

### 2.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    private readonly int _id = id;
    private readonly string _name = name;
    public int Id => _id;
    public string Name => _name;
    public User ToUser() => new(_id, _name);
}
~~~

## 3 生成构造函数设置属性样式
### 3.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Property

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property)]
public partial class UserDto;
~~~

### 3.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public User ToUser() => new(Id, Name);
}
~~~

## 4 生成构造函数设置字段及属性样式
### 4.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Property | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property | InitializeKind.Field)]
public partial class UserDto;
~~~

### 4.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    private int _id = id;
    private string _name = name;
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public GeneratePocoTests.User ToUser() => new(_id, _name);
}
~~~

## 5 生成属性默认样式
### 5.1 配置代码
>* Initializer配置为InitializeKind.Constructor

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Property)]
public partial class UserDto;
~~~

### 5.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public User ToUser() => new(Id, Name);
}
~~~

## 6 生成属性Init样式
### 6.1 配置代码
>* Initializer配置为InitializeKind.Init

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Init)]
public partial class UserDto;
~~~

### 6.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public User ToUser() => new(Id, Name);
}
~~~

## 7 生成属性含字段样式
### 7.1 配置代码
>* Initializer配置为InitializeKind.Property | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Property | InitializeKind.Field)]
public partial class UserDto;
~~~

### 7.2 生成代码
~~~csharp
partial class UserDto
{
    private int _id;
    private string _name;
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public GeneratePocoTests.User ToUser() => new(_id, _name);
}
~~~

## 8 生成属性Init含字段样式
### 8.1 配置代码
>* Initializer配置为InitializeKind.Init | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Init | InitializeKind.Field)]
public partial class UserDto;
~~~

### 8.2 生成代码
~~~csharp
partial class UserDto
{
    private int _id;
    private string _name;
    public int Id { get => _id; init => _id = value; }
    public string Name { get => _name; init => _name = value; }
    public User ToUser() => new(_id, _name);
}
~~~

## 9 生成字段样式
### 9.1 配置代码
>* Initializer配置为InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Field)]
public partial class UserDto;
~~~

### 9.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id;
    public string Name;
    public User ToUser() => new(Id, Name);
}
~~~

## 10 生成记录样式
### 10.1 配置代码
>* 需要目标类型为record(或struct record),Initializer不配置

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>()]
public partial record UserDto;
~~~

### 10.2 生成代码
~~~csharp
partial record UserDto(int Id, string Name)
{
    public User ToUser() => new(Id, Name);
}
~~~

## 11 GeneratePoco其他配置属性
## 11.1 ConvertFrom/ConvertTo配置
>* ConvertFrom默认false,为true时生成从源类型到目标类型的静态扩展转换方法
>* ConvertTo默认true,为true时生成从目标类型到源类型的实例转换方法
>* 前面已有生成两种方法的示例,这里不再赘述

## 11.2 Rules配置
>* 从源类型到目标类型的属性映射规则
>* 可以配置Include、Exclude等规则
>* 参看[SourceGenerator之扑风捉影](https://www.cnblogs.com/xiangji/p/19942266)

## 11.3 NullableRule配置
>* NullableRule配置哪些属性(及其相关的字段和参数)为可空类型
>* 参看[SourceGenerator之扑风捉影](https://www.cnblogs.com/xiangji/p/19942266)

## 11.4 GenerateAttribute配置
>* GenerateAttribute默认false,为true时源类型属性的特性会被复制到目标类型属性(及其相关的字段和参数)上
>* 会判断源Attribute类型的AttributeUsage,是否适合应用到目标类型属性(及其相关的字段和参数)上,如果不适合则不会复制

~~~csharp
// 配置代码
public class Product(int productId, string productName)
{
    public int ProductId { get; } = productId;
    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; } = productName;
}
[GeneratePoco<Product>(GenerateAttribute = true)]
public partial class ProductDto;
~~~

~~~csharp
// 生成代码
partial class ProductDto
{
    public int ProductId { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; set; }
    public GeneratePocoTests.Supports.Product ToProduct() => new(ProductId, ProductName);
}
~~~

## 11.5 Default配置
>* Default默认false,为true时生成默认默认值
>* 同一个属性及其相关的字段和参数之一生成默认值
>* 生成默认值的优先级是参数>字段>属性

~~~csharp
// 配置代码
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Property | InitializeKind.Field, Default = true)]
public partial class UserDto;
~~~

~~~csharp
// 生成代码
partial class UserDto
{
    private int _id = default;
    private string _name = "";
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public User ToUser() => new(_id, _name);
}
~~~

## 12 GeneratePoco不只是生成DTO
>* GeneratePoco实际上是生成十种不同的POCO类
>* 这十种样式并不全适合DTO
>* 也可以称之为影子类

### 3.2 GenerateTable生成器
>* GenerateTable生成器按实体类型生成表结构类
>* 生成的表结构类可以用于生成数据库表,也可以用于对数据表进行增删改查等操作
>* 一年前网友秦时明留言建议做源生成器,现在才补上会不会有点亡羊补牢的感觉
>* 可以参看[ShadowSql.net之正确使用方式](https://www.cnblogs.com/xiangji/p/18909458)

#### 3.2.1 GenerateTable配置代码
>* 通过Attribute配置表结构
>* Table配置表名
>* DatabaseGeneratedOption.Identity配置自增列
>* Key配置主键
>* Column配置了列名或数据库原始类型
>* Unique配置唯一索引(支持多列组合唯一)

~~~csharp
[Table("Products")]
public class Product(int id, string name)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; } = id;
    [Unique]
    [Column("ProductName")]
    public string Name { get; } = name;
    [Unique("CategoryModel")]
    public int CategoryId { get; set; }
    [Unique("CategoryModel")]
    public string Model { get; set; }
}
[GenerateTable<Product>]
public partial class ProductTable;
~~~

#### 3.2.2 GenerateTable生成代码
~~~csharp
partial class ProductTable : ShadowSql.Identifiers.Table
{
    public ProductTable(string tableName = "Products") : base(tableName)
    {
        Id = DefineColumn("Id");
        Name = DefineColumn("Name", "ProductName");
        CategoryId = DefineColumn("CategoryId");
        Model = DefineColumn("Model");
        AddInsertIgnore(Id);
        AddUpdateIgnore(Id);
    }

    public ShadowSql.Identifiers.IColumn Id { get; }
    public new ShadowSql.Identifiers.IColumn Name { get; }
    public ShadowSql.Identifiers.IColumn CategoryId { get; }
    public ShadowSql.Identifiers.IColumn Model { get; }
}
~~~
