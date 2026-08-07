# Roslyn生成DTO及影子编程的尝试

## 1. DTO含有大量重复代码

### 1.1 实体类Comment代码如下
~~~csharp
/// <summary>
/// 评论
/// </summary>
/// <param name="id"></param>
/// <param name="createTime"></param>
public class Comment(CommentId id, CommentCreateTime createTime)
{
    /// <summary>
    /// 评论标识
    /// </summary>
    public CommentId Id { get; } = id;
    /// <summary>
    /// 评论内容
    /// </summary>
    public CommentContent Content { get; set; }
    /// <summary>
    /// 评论创建时间
    /// </summary>
    public CommentCreateTime CreateTime { get; } = createTime;
    /// <summary>
    /// 评论更新时间
    /// </summary>
    public CommentUpdateTime UpdateTime { get; set; }
}
/// <summary>
/// 评论标识
/// </summary>
/// <param name="Original"></param>
public readonly record struct CommentId(long Original) : IEntityId;
/// <summary>
/// 评论内容
/// </summary>
/// <param name="Original"></param>
public readonly record struct CommentContent(string Original) : IEntityProperty<string>;
/// <summary>
/// 评论创建时间
/// </summary>
/// <param name="Original"></param>
public readonly record struct CommentCreateTime(DateTime Original) : IEntityProperty<DateTime>;
/// <summary>
/// 评论更新时间
/// </summary>
/// <param name="Original"></param>
public readonly record struct CommentUpdateTime(DateTime Original) : IEntityProperty<DateTime>;
~~~

### 1.2 该CURD的DTO类型可能就会有很多重复代码
>* 该示例有9个DTO类与Comment有同名属性

~~~csharp
public sealed class CreateRequest
{
    public string Content { get; set; }
}
public sealed class CreateResponse : ResponseBase
{
    public long Id { get; set; }
}
public sealed class DeleteRequest
{
    public long Id { get; set; }
}
public sealed class DeleteResponse : ResponseBase
{
    public long Id { get; set; }
}
public sealed class UpdateRequest
{
    public long Id { get; set; }
}
public sealed class UpdateResponse : ResponseBase
{
    public long Id { get; set; }
}
public sealed class DetailRequest
{
    public long Id { get; set; }
}
public sealed class DetailResponse : ResponseBase
{
    public long Id { get; set; }
    public string Content { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
public sealed class ListRequest
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
public sealed class ListResponse : ResponseBase
{
    public int Total { get; set; }
    public CommentItem[] Items { get; set; }
}
public class CommentItem
{
    public long Id { get; set; }
    public string Content { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
}
~~~

### 1.3 使用代码生成器可以减少重复代码
>* 如下GeneratePoco生成器为5个DTO类生成属性,节省了大量重复代码
>* 如果属性太少生成器的收益不大
>* GeneratePoco的泛型参数标记了该类型与哪个实体类相关联,也就是说是哪个类型的附属类或者影子类
>* 通过ConvertFrom和ConvertTo配置是否生成从实体类到DTO的转换方法,或者从DTO到实体类的转换方法

~~~csharp
[GeneratePoco<Comment>(ConvertFrom = true, ConvertTo = false)]
public partial class CreateResponse : ResponseBase;
[GeneratePoco<Comment>(Rules = [$"Include: {nameof(Comment.Id)}"], ConvertFrom = false, ConvertTo = false)]
public partial class DeleteResponse : ResponseBase;
[GeneratePoco<Comment>(Rules = [$"Include: {nameof(Comment.Id)}"], ConvertFrom = false, ConvertTo = false)]
public partial class UpdateResponse : ResponseBase;
[GeneratePoco<Comment>(ConvertFrom = true, ConvertTo = false)]
public partial class DetailResponse : ResponseBase;
[GeneratePoco<Comment>(ConvertFrom = true, ConvertTo = false)]
public partial class CommentItem;
~~~

### 1.4 生成代码如下
>* 由源实体的强类型属性生成基本类型的DTO属性,GeneratePoco的作用之一
>* ConvertFrom的方法生成为源类型的静态扩展方法
>* ConvertTo的方法生成为目标类型的实例方法
>* 如果源类型属性有注释，生成的DTO属性也会包含相应的注释
>* 如果DTO属性名与源类型属性名不一致，可以通过Rules配置,参看[SourceGenerator之扑风捉影](https://www.cnblogs.com/xiangji/p/19942266)

~~~csharp
namespace GenerateApp.Create
{
    partial class CreateResponse
    {
        ///<summary>
        ///评论标识
        ///</summary>
        public long Id { get; set; }
        ///<summary>
        ///评论内容
        ///</summary>
        public string Content { get; set; }
        ///<summary>
        ///评论创建时间
        ///</summary>
        public DateTime CreateTime { get; set; }
        ///<summary>
        ///评论更新时间
        ///</summary>
        public DateTime UpdateTime { get; set; }
    }
}
namespace CommentModels
{
    internal static partial class CommentExtensions
    {
        ///<summary>
        ///转化
        ///</summary>
        ///<param name = "message"></param>
        public static global::GenerateApp.Create.CreateResponse ToCreateResponse(this Comment @this, string message = "") => new()
        {
            Message = message,
            Id = @this.Id.Original,
            Content = @this.Content.Original,
            CreateTime = @this.CreateTime.Original,
            UpdateTime = @this.UpdateTime.Original
        };
    }
}
namespace GenerateApp.Delete
{
    partial class DeleteResponse
    {
        ///<summary>
        ///评论标识
        ///</summary>
        public long Id { get; set; }
    }
}
namespace GenerateApp.Update
{
    partial class UpdateResponse
    {
        ///<summary>
        ///评论标识
        ///</summary>
        public long Id { get; set; }
    }
}
namespace GenerateApp.Detail
{
    partial class DetailResponse
    {
        ///<summary>
        ///评论标识
        ///</summary>
        public long Id { get; set; }
        ///<summary>
        ///评论内容
        ///</summary>
        public string Content { get; set; }
        ///<summary>
        ///评论创建时间
        ///</summary>
        public DateTime CreateTime { get; set; }
        ///<summary>
        ///评论更新时间
        ///</summary>
        public DateTime UpdateTime { get; set; }
    }
}
namespace CommentModels
{
    internal static partial class CommentExtensions
    {
        ///<summary>
        ///转化
        ///</summary>
        ///<param name = "message"></param>
        public static global::GenerateApp.Detail.DetailResponse ToDetailResponse(this Comment @this, string message = "") => new()
        {
            Message = message,
            Id = @this.Id.Original,
            Content = @this.Content.Original,
            CreateTime = @this.CreateTime.Original,
            UpdateTime = @this.UpdateTime.Original
        };
    }
}
namespace GenerateApp.List
{
    partial class CommentItem
    {
        ///<summary>
        ///评论标识
        ///</summary>
        public long Id { get; set; }
        ///<summary>
        ///评论内容
        ///</summary>
        public string Content { get; set; }
        ///<summary>
        ///评论创建时间
        ///</summary>
        public DateTime CreateTime { get; set; }
        ///<summary>
        ///评论更新时间
        ///</summary>
        public DateTime UpdateTime { get; set; }
    }
}
namespace CommentModels
{
    internal static partial class CommentExtensions
    {
        ///<summary>
        ///转化
        ///</summary>
        public static global::GenerateApp.List.CommentItem ToItem(this Comment @this) => new()
        {
            Id = @this.Id.Original,
            Content = @this.Content.Original,
            CreateTime = @this.CreateTime.Original,
            UpdateTime = @this.UpdateTime.Original
        };
    }
}
~~~

### 1.5 应用业务逻辑
### 1.5.1 原始的业务逻辑代码
~~~csharp
    private readonly ICommentService _service = service;

    public override Task<CreateResponse> ExecuteAsync(CreateRequest req, CancellationToken ct)
    {
        var content = new CommentContent(req.Content);
        var comment = _service.AddComment(content);
        var res = new CreateResponse()
        {
            Id = comment.Id.Original,
            Content = comment.Content.Original,
            CreateTime = comment.CreateTime.Original,
            UpdateTime = comment.UpdateTime.Original,
        };
        return Task.FromResult(res);
    }
~~~

### 1.5.2 使用代码生成的DTO的业务逻辑代码
>* GeneratePoco生成的类型转化方法代替了Mapper,简化了业务逻辑代码

~~~csharp
    private readonly ICommentService _service = service;

    public override Task<CreateResponse> ExecuteAsync(CreateRequest req, CancellationToken ct)
    {
        var content = new CommentContent(req.Content);
        var comment = _service.AddComment(content);
        return Task.FromResult(comment.ToCreateResponse("Success"));
    }
~~~

### 1.6 Roslyn生成DTO现存的问题
>* 生成器执行顺序问题
>* DTO可以通过JsonSourceGenerator生成序列化的代码来提高性能和AOT支持等
>* 但是JsonSourceGenerator的执行顺序在GeneratePoco之前,生成的序列化代码不包含GeneratePoco生成的属性
>* 所以使用GeneratePoco的类型不能使用JsonSourceGenerator生成器
>* 这个问题将来某个时间微软应该会解决
>* 暂时我也没有找到解决方案

### 1.7 以上示例代码已经上传到github,可以直接查看
>* https://github.com/donetsoftwork/GeneratePocoDemo
>* gitee同步跟新: https://gitee.com/donetsoftwork/GeneratePocoDemo

## 2. 影子编程

### 2.1 GeneratePoco生成构造函数
#### 2.1.1 生成构造函数默认样式
##### 2.1.1.1 配置代码
>* Initializer配置为InitializeKind.Constructor

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor)]
public partial class UserDto;
~~~

##### 2.1.1.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public User ToUser() => new(Id, Name);
}
~~~

#### 2.1.2 生成构造函数含字段样式
##### 2.1.2.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 2.1.2.2 生成代码
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

#### 2.1.3 生成构造函数设置属性样式
##### 2.1.3.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Property

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property)]
public partial class UserDto;
~~~

##### 2.1.3.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public User ToUser() => new(Id, Name);
}
~~~

#### 2.1.4 生成构造函数设置字段及属性样式
##### 2.1.4.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Property | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 2.1.4.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    private int _id = id;
    private string _name = name;
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public global::GeneratePocoTests.User ToUser() => new(_id, _name);
}
~~~

#### 2.1.5 生成属性默认样式
##### 2.1.5.1 配置代码
>* Initializer配置为InitializeKind.Constructor

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Property)]
public partial class UserDto;
~~~

##### 2.1.5.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public User ToUser() => new(Id, Name);
}
~~~

#### 2.1.6 生成属性Init样式
##### 2.1.6.1 配置代码
>* Initializer配置为InitializeKind.Init

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Init)]
public partial class UserDto;
~~~

##### 2.1.6.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public User ToUser() => new(Id, Name);
}
~~~

#### 2.1.7 生成属性含字段样式
##### 2.1.7.1 配置代码
>* Initializer配置为InitializeKind.Property | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Property | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 2.1.7.2 生成代码
~~~csharp
partial class UserDto
{
    private int _id;
    private string _name;
    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public global::GeneratePocoTests.User ToUser() => new(_id, _name);
}
~~~

#### 2.1.8 生成属性Init含字段样式
##### 2.1.8.1 配置代码
>* Initializer配置为InitializeKind.Init | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Init | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 2.1.8.2 生成代码
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

#### 2.1.9 生成字段样式
##### 2.1.9.1 配置代码
>* Initializer配置为InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Field)]
public partial class UserDto;
~~~

##### 2.1.9.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id;
    public string Name;
    public User ToUser() => new(Id, Name);
}
~~~

#### 2.1.10 生成记录样式
##### 2.1.10.1 配置代码
>* 需要目标类型为record(或struct record),Initializer不配置

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>()]
public partial record UserDto;
~~~

##### 2.1.10.2 生成代码
~~~csharp
partial record UserDto(int Id, string Name)
{
    public User ToUser() => new(Id, Name);
}
~~~

#### 2.1.11 GeneratePoco其他配置属性
#### 2.1.11.1 ConvertFrom/ConvertTo配置
>* ConvertFrom默认false,为true时生成从源类型到目标类型的静态扩展转换方法
>* ConvertTo默认true,为true时生成从目标类型到源类型的实例转换方法
>* 前面已有生成两种方法的示例,这里不再赘述

#### 2.1.11.2 Rules配置
>* 从源类型到目标类型的属性映射规则
>* 可以配置Include、Exclude等规则
>* 参看[SourceGenerator之扑风捉影](https://www.cnblogs.com/xiangji/p/19942266)

#### 2.1.11.3 NullableRule配置
>* NullableRule配置哪些属性(及其相关的字段和参数)为可空类型
>* 参看[SourceGenerator之扑风捉影](https://www.cnblogs.com/xiangji/p/19942266)

#### 2.1.11.4 GenerateAttribute配置
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
    public global::GeneratePocoTests.Supports.Product ToProduct() => new(ProductId, ProductName);
}
~~~

#### 2.1.11.5 Default配置
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

#### 2.1.12 GeneratePoco不只是生成DTO
>* GeneratePoco实际上是生成十种不同的POCO类
>* 这十种样式并不全适合DTO
>* 也可以称之为影子类

### 2.2 GenerateTable生成器
>* GenerateTable生成器按实体类型生成表结构类
>* 生成的表结构类可以用于生成数据库表,也可以用于对数据表进行增删改查等操作

