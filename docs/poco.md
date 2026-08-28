# Roslyn生成DTO及影子编程的尝试
>Roslyn支持源生成器机制大大扩展了.net的能力,大量的重复、固定和稳定的代码都可以交给源生成器

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
>* 请查看示例的[github代码库](https://github.com/donetsoftwork/GeneratePocoDemo)
>* [gitee代码库](https://gitee.com/donetsoftwork/GeneratePocoDemo)也同步更新

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
>* 如下开源项目GeneratePoco生成器为5个DTO类生成属性,节省了大量重复代码
>* 如果属性太少生成器的收益不大
>* GeneratePoco的泛型参数标记了该类型与哪个实体类相关联,也就是说是哪个类型的附属类或者影子类
>* 通过ConvertFrom和ConvertTo配置是否生成从实体类到DTO的转换方法,或者从DTO到实体类的转换方法

~~~csharp
[GeneratePoco<Comment>(ConvertFrom = true, ConvertTo = false)]
public partial class CreateResponse : ResponseBase;
[GeneratePoco<Comment>(Rules = [$"Include: {nameof(Comment.Id)}"], ConvertTo = false)]
public partial class DeleteResponse : ResponseBase;
[GeneratePoco<Comment>(Rules = [$"Include: {nameof(Comment.Id)}"], ConvertTo = false)]
public partial class UpdateResponse : ResponseBase;
[GeneratePoco<Comment>(ConvertFrom = true, ConvertTo = false)]
public partial class DetailResponse : ResponseBase;
[GeneratePoco<Comment>(ConvertFrom = true, ConvertTo = false)]
public partial class CommentItem;
~~~

### 1.4 生成代码如下
>* 由源实体的强类型属性生成基本类型的DTO属性,是GeneratePoco的应用场景之一
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
    using GenerateApp.Create;
    internal static partial class CommentExtensions
    {
        ///<summary>
        ///转化
        ///</summary>
        ///<param name = "message"></param>
        public static CreateResponse ToCreateResponse(this Comment @this, string message = "") => new()
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
    using GenerateApp.Detail;
    internal static partial class CommentExtensions
    {
        ///<summary>
        ///转化
        ///</summary>
        ///<param name = "message"></param>
        public static DetailResponse ToDetailResponse(this Comment @this) => new()
        {
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
    using GenerateApp.List;
    internal static partial class CommentExtensions
    {
        ///<summary>
        ///转化
        ///</summary>
        public static CommentItem ToItem(this Comment @this) => new()
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
>* 开源项目GeneratePoco的Mapper功能是通过引用开源项目GenerateConvert来实现的
>* GenerateConvert本身可以单独用来生成Mapper代码,有点类似开源项目mapperly

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


## 2. 可以用GenerateConvert来做类型转化
#### 2.1 转化为DTO的Case
>* Car转化为CarDto

#### 2.1.1 Car及相关类
~~~csharp
public class Car
{
    public string Name { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public CarColor Color { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public List<Tire> Tires { get; set; } = [];
    public LicensePlate LicensePlate { get; set; }
}
public enum CarColor
{
    Black = 1,
    Blue = 2,
    White = 3,
}
public class Manufacturer(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
public class Tire
{
    public string Description { get; set; } = string.Empty;
}
public readonly struct LicensePlate(string  value)
{
    public string Value { get;} = value;

    public override readonly string ToString() => Value;
}
~~~

#### 2.1.1 CarDto及相关类
~~~csharp
public class CarDto
{
    public string Name { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public CarColorDto Color { get; set; }
    public ProducerDto? Producer { get; set; }
    public List<TireDto>? Tires { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
}
public enum CarColorDto
{
    Yellow = 1,
    Green = 2,
    Black = 3,
    Blue = 4,
}
public class ProducerDto(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
}
public class TireDto(string description)
{
    public string Description { get; set; } = description;
}
~~~

#### 2.2 可以用GenerateConvert来做类型转化
#### 2.2.1 GenerateConvert配置代码
>* 把CarDto设置为partial
>* 增加GenerateConvert\<Car\>标记
>* 配置ConvertFrom = true生成Car转化为
>* Rules配置同前面GeneratePoco的Rules,是通过投影规则定义成员映射规则
>* 这里定义了CarDto的Producer隐射到Car的Manufacturer

~~~csharp
[GenerateConvert<Car>(
    Rules = [
        $"Map {nameof(Producer)}, {nameof(Car.Manufacturer)}"
    ],
    ConvertTo = false,
    ConvertFrom = true
)]
public partial class CarDto
{
    public string Name { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public CarColorDto Color { get; set; }
    public ProducerDto? Producer { get; set; }
    public List<TireDto>? Tires { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
}
~~~

#### 2.2.2 GenerateConvert生成的代码
>* 生成了4个扩展方法
>* 其中Car.ToDto是入口方法
>* 这些方法都很简洁,几乎和手写的一样
>* 每个方法都有summary注释

~~~csharp
using ConvertDemo.Models;

internal static partial class CarExtensions
{
    ///<summary>
    ///转化为CarDto
    ///</summary>
    public static CarDto ToDto(this Car @this) => new()
    {
        Name = @this.Name,
        NumberOfSeats = @this.NumberOfSeats,
        Color = @this.Color.ToDto(),
        Producer = @this.Manufacturer == null ? default : @this.Manufacturer.ToProducerDto(),
        Tires = @this.Tires.ConvertAll(item => item.ToDto()),
        LicensePlate = @this.LicensePlate.Value
    };
}
internal static partial class CarColorExtensions
{
    ///<summary>
    ///转化为CarColorDto
    ///</summary>
    public static CarColorDto ToDto(this CarColor @this) => @this switch
    {
        CarColor.Black => CarColorDto.Black,
        CarColor.Blue => CarColorDto.Blue,
        _ => default
    };
}
internal static partial class ManufacturerExtensions
{
    ///<summary>
    ///转化为ProducerDto
    ///</summary>
    public static ProducerDto? ToProducerDto(this Manufacturer @this) => new(@this.Id, @this.Name);
}
internal static partial class TireExtensions
{
    ///<summary>
    ///转化为TireDto
    ///</summary>
    public static TireDto ToDto(this Tire @this) => new(@this.Description);
}
~~~

#### 2.3 也可以用mapperly实现这个需求
#### 2.3.1 mapperly配置代码
>* 新建partial类CarMapper
>* 增加Mapper特性配置
>* 增加一个Car转化为CarDto的partial方法
>* 通过MapProperty配置映射规则

~~~csharp
[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class CarMapper
{
    [MapProperty(nameof(Car.Manufacturer), nameof(CarDto.Producer))] 
    public static partial CarDto MapCarToDto(Car car);
}
~~~

#### 2.3.2 mapperly生成代码
>* 生成了5个静态方法
>* 1个公开的入口,4个私有的静态方法,并不能单独使用
>* 类名都用global+全命名空间

~~~csharp
public static partial class CarMapper
{
    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    public static partial global::MapperlyDemo.Models.CarDto MapCarToDto(global::Demo.Models.Car car)
    {
        var target = new global::MapperlyDemo.Models.CarDto();
        target.Name = car.Name;
        target.NumberOfSeats = car.NumberOfSeats;
        target.Color = MapToCarColorDto(car.Color);
        if (car.Manufacturer != null)
        {
            target.Producer = MapToProducerDto(car.Manufacturer);
        }
        else
        {
            target.Producer = null;
        }
        target.Tires = MapToListOfTireDto(car.Tires);
        target.LicensePlate = car.LicensePlate.ToString();
        return target;
    }

    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::MapperlyDemo.Models.CarColorDto MapToCarColorDto(global::Demo.Models.CarColor source)
    {
        return source switch
        {
            global::Demo.Models.CarColor.Black => global::MapperlyDemo.Models.CarColorDto.Black,
            global::Demo.Models.CarColor.Blue => global::MapperlyDemo.Models.CarColorDto.Blue,
            _ => throw new global::System.ArgumentOutOfRangeException(nameof(source), source, "The value of enum CarColor is not supported"),
        };
    }

    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::MapperlyDemo.Models.ProducerDto MapToProducerDto(global::Demo.Models.Manufacturer source)
    {
        var target = new global::MapperlyDemo.Models.ProducerDto(source.Id, source.Name);
        return target;
    }

    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::MapperlyDemo.Models.TireDto MapToTireDto(global::Demo.Models.Tire source)
    {
        var target = new global::MapperlyDemo.Models.TireDto();
        target.Description = source.Description;
        return target;
    }

    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::System.Collections.Generic.List<global::MapperlyDemo.Models.TireDto> MapToListOfTireDto(global::System.Collections.Generic.IReadOnlyCollection<global::Demo.Models.Tire> source)
    {
        var target = new global::System.Collections.Generic.List<global::MapperlyDemo.Models.TireDto>(source.Count);
        foreach (var item in source)
        {
            target.Add(MapToTireDto(item));
        }
        return target;
    }
}
~~~

#### 2.4 用GenerateConvert实现互转
#### 2.4.1 GenerateConvert互转配置代码
>* 与2.2示例唯一区别就是ConvertTo配置为true

~~~csharp
[GenerateConvert<Car>(
    Rules = [
        $"Map {nameof(Producer)}, {nameof(Car.Manufacturer)}"
    ],
    ConvertTo = true,
    ConvertFrom = true
)]
public partial class CarDto
{
    public string Name { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public CarColorDto Color { get; set; }
    public ProducerDto? Producer { get; set; }
    public List<TireDto>? Tires { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
}
~~~

#### 2.4.2 GenerateConvert互转生成的代码
>* 比2.2示例多生成以下4个方法
>* 其中2个实例方法2个扩展方法
>* 如果转化源类为当前程序集的partial类就会生成实例方法,否则生成扩展方法

~~~csharp
namespace ConvertDemo.Models;
partial class CarDto
{
    ///<summary>
    ///转化为Car
    ///</summary>
    public Car ToCar() => new()
    {
        Name = Name,
        NumberOfSeats = NumberOfSeats,
        Color = Color.ToCarColor(),
        Manufacturer = Producer == null ? default : Producer.ToManufacturer(),
        Tires = Tires == null ? [] : Tires.ConvertAll(item => item.ToTire()),
        LicensePlate = new LicensePlate(LicensePlate)
    };
}
public static partial class CarColorDtoExtensions
{
    ///<summary>
    ///转化为CarColor
    ///</summary>
    public static CarColor ToCarColor(this CarColorDto @this) => @this switch
    {
        CarColorDto.Black => CarColor.Black,
        CarColorDto.Blue => CarColor.Blue,
        _ => default
    };
}
partial class ProducerDto
{
    ///<summary>
    ///转化为Manufacturer
    ///</summary>
    public Manufacturer? ToManufacturer() => new(Id, Name);
}
public static partial class TireDtoExtensions
{
    ///<summary>
    ///转化为Tire
    ///</summary>
    public static Tire ToTire(this TireDto @this) => new()
    {
        Description = @this.Description
    };
}
~~~

#### 2.5 用mapperly实现互转
#### 2.5.1 mapperly互转配置代码
>* 比2.3示例多1个方法MapDtoToCar及其MapProperty特性配置

~~~csharp
[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class CarMapper
{
    [MapProperty(nameof(Car.Manufacturer), nameof(CarDto.Producer))]
    public static partial CarDto MapCarToDto(Car car);

    [MapProperty(nameof(CarDto.Producer), nameof(Car.Manufacturer))]
    public static partial Car MapDtoToCar(CarDto car);
}
~~~

#### 2.5.2 mapperly互转生成代码
>* 比2.3示例多生成了5个静态方法
>* 其中1个公开的入口方法及4个私有的静态方法
>* 类名都用global+全命名空间

~~~csharp
public static partial class CarMapper
{
    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    public static partial global::Demo.Models.Car MapDtoToCar(global::MapperlyDemo.Models.CarDto car)
    {
        var target = new global::Demo.Models.Car();
        target.Name = car.Name;
        target.NumberOfSeats = car.NumberOfSeats;
        target.Color = MapToCarColor(car.Color);
        if (car.Producer != null)
        {
            target.Manufacturer = MapToManufacturer(car.Producer);
        }
        else
        {
            target.Manufacturer = null;
        }
        if (car.Tires != null)
        {
            target.Tires = MapToListOfTire(car.Tires);
        }
        target.LicensePlate = new global::Demo.Models.LicensePlate(car.LicensePlate);
        return target;
    }
    
    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::Demo.Models.CarColor MapToCarColor(global::MapperlyDemo.Models.CarColorDto source)
    {
        return source switch
        {
            global::MapperlyDemo.Models.CarColorDto.Black => global::Demo.Models.CarColor.Black,
            global::MapperlyDemo.Models.CarColorDto.Blue => global::Demo.Models.CarColor.Blue,
            _ => throw new global::System.ArgumentOutOfRangeException(nameof(source), source, "The value of enum CarColorDto is not supported"),
        };
    }
    
    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::Demo.Models.Manufacturer MapToManufacturer(global::MapperlyDemo.Models.ProducerDto source)
    {
        var target = new global::Demo.Models.Manufacturer(source.Id, source.Name);
        return target;
    }
    
    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::Demo.Models.Tire MapToTire(global::MapperlyDemo.Models.TireDto source)
    {
        var target = new global::Demo.Models.Tire();
        target.Description = source.Description;
        return target;
    }
    
    [global::System.CodeDom.Compiler.GeneratedCode("Riok.Mapperly", "4.3.1.0")]
    private static global::System.Collections.Generic.List<global::Demo.Models.Tire> MapToListOfTire(global::System.Collections.Generic.IReadOnlyCollection<global::MapperlyDemo.Models.TireDto> source)
    {
        var target = new global::System.Collections.Generic.List<global::Demo.Models.Tire>(source.Count);
        foreach (var item in source)
        {
            target.Add(MapToTire(item));
        }
        return target;
    }
}
~~~

### 2.6 GenerateConvert和mapperly的对比
>* 以下是GenerateConvert和mapperly的对比,你喜欢哪种

#### 2.6.1 命名空间处理
|特性|GenerateConvert|mapperly|
|--|:--|:--|
|同名类处理|有同名类型才增加命名空间|直接global+命名空间+类名，不用单独处理|
|引用|把命名空间提取为using|无using|
|简洁性|更好|大量重复代码|
|准确性|同名类且命名空间包含可能导致错误|同名类只要命名空间不完全相同就不会出错|

#### 2.6.2 映射配置
|特性|GenerateConvert|mapperly|
|--|:--|:--|
|完善性|支持同名、前缀、后缀、替换、一对一等及Filter、Through和Cross投影|同名、一对一、忽略等|
|复用性|互转可共用配置且与集成的GeneratePoco共用|每个转化方法单独配置|

#### 2.6.3 调用方法
|特性|GenerateConvert|mapperly|
|--|:--|:--|
|入口方法|实例方法或扩展方法,按实例调用|调用Mapper的方法|
|辅助方法|也是方法或扩展方法,按实例调用|私有,不可单独调用|

### 2.7 GenerateConvert项目信息
>* nuget dotnet add package Hand.GenerateConvert --version 0.2.1.17-alpha
>* github https://github.com/donetsoftwork/Hand.Generators/tree/master/Hand.GenerateConvert
>* gitee https://gitee.com/donetsoftwork/hand.-generators/tree/master/Hand.GenerateConvert


## 3. 更多生成器的应用

### 3.1 GeneratePoco生成构造函数
#### 3.1.1 生成构造函数默认样式
##### 3.1.1.1 配置代码
>* Initializer配置为InitializeKind.Constructor

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor)]
public partial class UserDto;
~~~

##### 3.1.1.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public User ToUser() => new(Id, Name);
}
~~~

#### 3.1.2 生成构造函数含字段样式
##### 3.1.2.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 3.1.2.2 生成代码
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

#### 3.1.3 生成构造函数设置属性样式
##### 3.1.3.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Property

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property)]
public partial class UserDto;
~~~

##### 3.1.3.2 生成代码
~~~csharp
partial class UserDto(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public User ToUser() => new(Id, Name);
}
~~~

#### 3.1.4 生成构造函数设置字段及属性样式
##### 3.1.4.1 配置代码
>* Initializer配置为InitializeKind.Constructor | InitializeKind.Property | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Constructor | InitializeKind.Property | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 3.1.4.2 生成代码
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

#### 3.1.5 生成属性默认样式
##### 3.1.5.1 配置代码
>* Initializer配置为InitializeKind.Constructor

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Property)]
public partial class UserDto;
~~~

##### 3.1.5.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public User ToUser() => new(Id, Name);
}
~~~

#### 3.1.6 生成属性Init样式
##### 3.1.6.1 配置代码
>* Initializer配置为InitializeKind.Init

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Init)]
public partial class UserDto;
~~~

##### 3.1.6.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public User ToUser() => new(Id, Name);
}
~~~

#### 3.1.7 生成属性含字段样式
##### 3.1.7.1 配置代码
>* Initializer配置为InitializeKind.Property | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Property | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 3.1.7.2 生成代码
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

#### 3.1.8 生成属性Init含字段样式
##### 3.1.8.1 配置代码
>* Initializer配置为InitializeKind.Init | InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Init | InitializeKind.Field)]
public partial class UserDto;
~~~

##### 3.1.8.2 生成代码
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

#### 3.1.9 生成字段样式
##### 3.1.9.1 配置代码
>* Initializer配置为InitializeKind.Field

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>(Initializer = InitializeKind.Field)]
public partial class UserDto;
~~~

##### 3.1.9.2 生成代码
~~~csharp
partial class UserDto
{
    public int Id;
    public string Name;
    public User ToUser() => new(Id, Name);
}
~~~

#### 3.1.10 生成记录样式
##### 3.1.10.1 配置代码
>* 需要目标类型为record(或struct record),Initializer不配置

~~~csharp
public record User(int Id, string Name);
[GeneratePoco<User>()]
public partial record UserDto;
~~~

##### 3.1.10.2 生成代码
~~~csharp
partial record UserDto(int Id, string Name)
{
    public User ToUser() => new(Id, Name);
}
~~~

#### 3.1.11 GeneratePoco其他配置属性
#### 3.1.11.1 ConvertFrom/ConvertTo配置
>* ConvertFrom默认false,为true时生成从源类型到目标类型的静态扩展转换方法
>* ConvertTo默认true,为true时生成从目标类型到源类型的实例转换方法
>* 前面已有生成两种方法的示例,这里不再赘述

#### 3.1.11.2 Rules配置
>* 从源类型到目标类型的属性映射规则
>* 可以配置Include、Exclude等规则
>* 参看[SourceGenerator之扑风捉影](https://www.cnblogs.com/xiangji/p/19942266)

#### 3.1.11.3 NullableRule配置
>* NullableRule配置哪些属性(及其相关的字段和参数)为可空类型
>* 参看[SourceGenerator之扑风捉影](https://www.cnblogs.com/xiangji/p/19942266)

#### 3.1.11.4 GenerateAttribute配置
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
using GeneratePocoTests.Supports;
// 生成代码
partial class ProductDto
{
    public int ProductId { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public string ProductName { get; set; }
    public Product ToProduct() => new(ProductId, ProductName);
}
~~~

#### 3.1.11.5 Default配置
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

#### 3.1.12 GeneratePoco不只是生成DTO
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
using ShadowSql.Identifiers;
partial class ProductTable : Table
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

    public IColumn Id { get; }
    public new IColumn Name { get; }
    public IColumn CategoryId { get; }
    public IColumn Model { get; }
}
~~~

#### 3.2.2 GenerateTable项目信息
>* nuget dotnet add package ShadowSql.GenerateTable --version 0.9.1.1-alpha
>* github https://github.com/donetsoftwork/Shadow/tree/master/Generators/ShadowSql.GenerateTable
>* gitee https://gitee.com/donetsoftwork/Shadowtree/master/Generators/ShadowSql.GenerateTable

## 4. 部分其他源生成器开源项目
### 4.1 AutoDto
>* 生成DTO属性
>* https://github.com/Ohorodnikov/AutoDto

### 4.2 mapperly
>* 生成类型转化方法
>* https://github.com/riok/mapperly

### 4.3 MVVM Toolkit
>* MVVM模式开发桌面应用工具包
>* https://github.com/CommunityToolkit/dotnet

## 5. 影子编程
>* 源生成器可以生成一个类的辅助类型(DTO、序列化、表结构等)
>* 这些辅助类型就相当于源类型的影子
>* 把周边功能影子化不仅减少了代码量,还提高了代码的可预测性和内聚性,也提高了代码的可读性。

## 6. 源生成器开发技巧总结

### 6.1 Roslyn简易语法
>* 参看: https://www.cnblogs.com/xiangji/p/19688804

### 6.2 .net源生成器必须知道的4套类型
>* 参看: https://www.cnblogs.com/xiangji/p/22359584

### 6.3 C#.NET源生成器如何处理Attribute
>* 参看: https://www.cnblogs.com/xiangji/p/22439407

### 6.4 C#.NET源生成器如何处理XML注释文档
>* 参看: https://www.cnblogs.com/xiangji/p/22488819

### 6.5 Roslyn语法的模式匹配之EasySyntax增加模式匹配支持
>* 参看: https://www.cnblogs.com/xiangji/p/20691808

### 6.6 源生成器partial范式及单元测试
>* https://www.cnblogs.com/xiangji/p/19737143

### 6.7 源生成器nuget打包
>* https://www.cnblogs.com/xiangji/p/19781120

## 7. 示例说明
### 7.1 示例执行说明
以上代码执行依赖开源项目Hand.GeneratePoco和Hand.GenerateConvert及ShadowSql.GenerateTable,nuget包如下
Hand.GeneratePoco --version 0.2.1.19-alpha
Hand.GenerateConvert --version 0.2.1.17-alpha
ShadowSql.GenerateTable --version 0.9.1.1-alpha

### 7.2 示例代码及开源源码
#### 7.2.1 GeneratePoco示例
>* ggithub仓库: https://gitee.com/donetsoftwork/GeneratePocoDemo
>* ggitee同步更新:https://gitee.com/donetsoftwork/GeneratePocoDemo

#### 7.2.2 GenerateConvert示例
>* ggithub仓库: https://gitee.com/donetsoftwork/GenerateConvertDemo
>* ggitee同步更新:https://gitee.com/donetsoftwork/GenerateConvertDemo

#### 7.2.3 Hand.GeneratePoco和Hand.GenerateConvert项目源码
>* ggithub仓库: https://github.com/donetsoftwork/Hand.Generators
>* ggitee同步更新:https://gitee.com/donetsoftwork/hand.-generators

#### 7.2.4 ShadowSql.GenerateTable项目源码
>* github仓库: https://github.com/donetsoftwork/Shadow/tree/master/Generators/ShadowSql.GenerateTable
>* gitee同步更新: https://gitee.com/donetsoftwork/Shadowtree/master/Generators/ShadowSql.GenerateTable

感兴趣的同学可以去看看源码,欢迎star和pr