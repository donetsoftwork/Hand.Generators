# SourceGenerator之扑风捉影

上一篇博文提高用投影重构PocoEmit.Mapper
> [开源项目PocoEmit.Mapper重构之扑风捉影](https://www.cnblogs.com/xiangji/p/19899326)

这一篇演示投影在SourceGenerator中应用
>* 这是一个基于模型类生成附属类型(比如DTO)的实用开源小工具
>* dotnet add package Hand.GeneratePoco --version 0.2.0.2-alpha


## 一、首先来个简单的Case

### 1. 原始模型类代码
```csharp
public record User(int Id, string Name);
```

### 2. 用GeneratePocoAttribute标记触发代码生成的规则
>

```csharp
[GeneratePoco(typeof(User))]
public partial class UserDto;
```

### 3. 生成代码如下
```csharp
partial class UserDto
{
	public int Id { get; set; }
	public string Name { get; set; }
}
```

### 4. 投影图如下
~~~mermaid
graph LR
    subgraph User
        source-Id[Id]
        source-Name[Name]
    end

    subgraph UserDto
        projection-Id[Id]
        projection-Name[Name]
    end

    source-Id -->| | projection-Id
    source-Name -->| | projection-Name
~~~

### 5. 生成代码的作用
>* 避免重复代码
>* 一个系统往往属性相同或相似的类型很多
>* 而且如果如果其一修改了属性，其他类也往往要联动修改
>* 这时用代码生成就可以避免重复代码和联动修改

## 二、使用投影来调整代码
>* 通过Rules配置投影规则

### 1 增加前缀的Case
#### 1.1 增加投影规则
>* Prefix User就是生成的属性前缀是User

```csharp
[GeneratePoco(typeof(User), Rules = ["Prefix User"])]
public partial class UserDto;
```

#### 1.2 生成代码如下
```csharp
partial class UserDto
{
	public int UserId { get; set; }
	public string UserName { get; set; }
}
```

#### 1.3 投影图如下
~~~mermaid
graph LR
    subgraph User
        source-Id[Id]
        source-Name[Name]
    end

    subgraph UserDto
        projection-UserId[UserId]
        projection-UserName[UserName]
    end

    source-Id -->| AddUser | projection-UserId
    source-Name -->| AddUser | projection-UserName
~~~

#### 1.4 投影的作用
>* 出于代码规范或其他原因
>* 目标类型属性名可能与原始类型不太一样
>* 这时就可以用投影来解决
>* 投影可以轻松处理前缀、后缀、大小写等各种变换

### 2. 增加过滤的Case
#### 2.1 增加过滤投影规则
>* Exclude: Id就是排除Id属性
>* Prefix User还是生成的属性前缀是User

```csharp
[GeneratePoco(typeof(User), Rules = ["Exclude: Id", "Prefix User"])]
public partial class NewUserDto;
```

#### 2.2 生成代码如下
>* 过滤掉Id属性后
>* 所以只生成了UserName属性

```csharp
partial class NewUserDto
{
	public string UserName { get; set; }
}
```

#### 2.3 投影图如下
~~~mermaid
graph LR
    subgraph User
        source-Id[Id]
        source-Name[Name]
    end

    subgraph tmp
        tmp-Name[Name]
    end

    subgraph NewUserDto
        projection-UserName[UserName]
    end

    source-Name -->| Filter | tmp-Name
    tmp-Name -->| AddUser | projection-UserName
~~~

### 3. 多投影顺序问题
#### 3.1 调整影规则的顺序
>* Prefix User还是生成的属性前缀是User
>* Exclude: UserId就是排除UserId属性

```csharp
[GeneratePoco(typeof(User), Rules = ["Prefix User", "Exclude: UserId"])]
public partial class NewUserDto;
```

#### 3.2. 生成代码如下
>* 生成代码与上一个Case是一样的

```csharp
partial class NewUserDto
{
	public string UserName { get; set; }
}
```

#### 3.3. 投影图如下
~~~mermaid
graph LR
    subgraph User
        source-Id[Id]
        source-Name[Name]
    end

    subgraph tmp
        tmp-UserId[UserId]
        tmp-UserName[UserName]
    end

    subgraph NewUserDto
        projection-UserName[UserName]
    end

    source-Id -->| AddUser | tmp-UserId
    source-Name -->| AddUser | tmp-UserName
    tmp-UserName -->| | projection-UserName
~~~

#### 3.4. 多投影调整顺序的作用
>* 每个投影都会生成一个结果,多投影就会产生临时结果
>* 多投影配置需要注意,需要基于前一个投影的结果来配置
>* 就像这个Case,Prefix User规则后,如果Exclude: Id是没有作用的,因为Id已经被投影为UserId了

### 4. Cross投影
#### 4.1 Cross投影规则
```csharp
[GeneratePoco(typeof(User), Rules = ["Cross: Prefix User"])]
public partial class UserDto;
```

#### 4.2 生成代码如下
```csharp
partial class UserDto
{
	public int Id { get; set; }
	public string Name { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
}
```

#### 4.3 投影图如下
~~~mermaid
graph LR
    subgraph User
        source-Id[Id]
        source-Name[Name]
    end
    subgraph UserDto
        projection-Id[Id]
        projection-Name[Name]
        projection-UserId[UserId]
        projection-UserName[UserName]
    end
    source-Id -->| | projection-Id
    source-Name -->| | projection-Name
    source-Id -->| AddUser | projection-UserId
    source-Name -->| AddUser | projection-UserName
~~~

#### 4.4 Cross投影的作用
>* Cross投影在投影时会保留原始属性,产生叠加的效果
>* 这里实际介绍了投影的3种效果,本代码生成器默认的是Through效果
>* 与[前一篇文章](https://www.cnblogs.com/xiangji/p/19899326)介绍的投影效果是一样的
>* 本次展示了投影在不同的场景下的应用

## 三、 属性类型简化
>* 领域模型的属性往往封装了DP(Domain Primitive),而目标类型更适合使用原始类型
>* 这也是该项目名叫GeneratePoco的原因之一,能简就简

### 1. 领域模型代码
>* 领域模型的属性往往使用DP(Domain Primitive)来封装
>* IEntityId和IEntityProperty接口应用于封装DP可以方便与原始类型的相互转换

```csharp
public class UserEntity(UserId id, UserName name)
    : IEntity<UserId>
{
    public UserId Id { get; } = id;
    public UserName Name { get; } = name;
}

public record struct UserId(long Original) : IEntityId;
public record struct UserName(string Original) : IEntityProperty<string>;
```

### 2. 目标类型代码
```csharp
[GeneratePoco(typeof(UserEntity), Rules = ["Prefix User"])]
public partial class UserViews;
```

### 3. 生成代码如下
>* 领域模型的属性如果封装了DP,会把DP的Original属性投影到目标类对应属性上
>* 在领域建模之外使用原始类型更方便

```csharp
partial class UserViews
{
    public long UserId { get; set; }
    public string UserName { get; set; }
}
```

## 四、配置可空类型
>* 特别是DTO参数类型,非必填参数可以设置为可空,后端逻辑再按一定规则填充默认值
>* NullableRule配置

### 1. 先预设模型类如下
```csharp
public class User(int id, string name, string email, int sex)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Email { get; } = email;
    public int Sex { get; } = sex;
}
```

### 2. 配置可空字段列表的Case
#### 2.1 目标类型代码
>* 以下配置UserEmail和UserSex为可空

```csharp
[GeneratePoco(typeof(User), Rules =
[
    "Prefix User"
], NullableRule = "UserEmail UserSex")]
public partial class UserDto;
```

#### 2.2 生成代码如下
```csharp
partial class UserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string? UserEmail { get; set; }
    public int? UserSex { get; set; }
}
```

### 3. 配置所有属性都可空的Case
#### 3.1 目标类型代码
>* NullableRule配置为ALL

```csharp
[GeneratePoco(typeof(User), Rules =
[
    "Prefix User"
], NullableRule = "ALL")]
public partial class UserDto;
```

#### 3.2 生成代码如下
```csharp
partial class UserDto
{
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public int? UserSex { get; set; }
}
```

### 4. 反向配置
>* 如果需要配置为空的字段太多,可以使用反向配置
>* 通过Exclude: 前缀进行方向配置

#### 4.1 目标类型代码
>* NullableRule配置为Exclude: UserId

```csharp
[GeneratePoco(typeof(User), Rules =
[
    "Prefix User"
], NullableRule = "Exclude: UserId")]
public partial class UserDto;
```

#### 4.2 生成代码如下
>* 除反向配置的UserId外其他都设置为可空类型

```csharp
partial class UserDto
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public int? UserSex { get; set; }
}
```

## 五、 处理Attribute
### 1. 源类型代码
```csharp
public class User(int id, string name, string email)
{
    public int Id { get; } = id;
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Name { get; } = name;
    [EmailAddress]
    public string Email { get; } = email;
}
```

### 2. 目标类型代码
```csharp
[GeneratePoco(typeof(User), Rules = ["Prefix User"])]
public partial class UserDto;
```

### 3. 生成的代码
>* 源类对应属性如果有Attribute,会把这些Attribute也投影到目标类对应属性上

```csharp
partial class UserDto
{
    public int UserId { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string UserName { get; set; }
    [EmailAddress]
    public string UserEmail { get; set; }
}
```

## 六、例外处理
>* 某些情况无法或者很难用规则配置,就使用例外处理

### 1. 先预设模型类如下
```csharp
public class User(int id, string name, string email, int sex)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Email { get; } = email;
    public int Sex { get; } = sex;
}
```

### 2. 目标类型代码
>* Sex原始类型是int,接收参数是可空字符串类型
>* 这时直接写死就好了
>* 代码生成器会忽略已经重名的属性

```csharp
[GeneratePoco(typeof(User), Rules = [ "Prefix User"])]
public partial class UserDto
{
    public string? UserSex { get; set; }
}
```

### 3. 生成代码如下
```csharp
partial class UserDto
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}
```

## 八、实现原理
### 1. 使用SyntaxTree简化语法
>* 参看以前的文章[.NET源码生成器使用SyntaxTree生成代码及简化语法](https://www.cnblogs.com/xiangji/p/19688804)

### 2. 基于partial范式
>* 参看以前的文章[SourceGenerator之partial范式及测试](https://www.cnblogs.com/xiangji/p/19737143)

### 3. 集成投影模块
#### 3.1 MemberValidation模块解析配置
>* MemberRecognizeParsere类解析配置字符串,投影规则
>* Cross:开头的解析为Cross投影
>* Through:开头的解析为Through投影
>* Filter:开头的解析为Filter投影
>* 非以上前缀的尝试解析为Through投影
>* 解析失败的再尝试解析为IValidation规则(相当于Filter)

## 九、总结
>* 以上规则基本都是可以排列组合使用的
>* 通过代码生成可以减少重复属性的定义,同时减少重复属性重构时要修改多个类的问题
>* 间接解决DTO类型复杂的继承关系(修改DTO继承新建DTO等)
>* 所有的依赖模型类来生成,代码更简单明了

投影及规则解析库的源码地址:
github: https://github.com/donetsoftwork/HandCore.net
gitee同步更新:https://gitee.com/donetsoftwork/HandCore.net

代码生成器及partial范式和SyntaxTree简化语法源码地址
源码托管地址: https://github.com/donetsoftwork/Hand.Generators
gitee同步更新:https://gitee.com/donetsoftwork/hand.-generators

感兴趣的同学可以去看看源码,欢迎star和pr