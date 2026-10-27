# C#.NET源生成器作用域及“千人千面”

## 一、C#.NET源生成器作用域
>* 依赖开源项目nuget包: Hand.Generators.EasySyntax --version 0.3.1.1-alpha

### 1. 生成简单类
#### 1.1 简单类的示例
>* 命名空间Models会解析为作用域NamespaceScope(ISyntaxScope)
>* SyntaxGenerator对应一个类,可以给类添加成员

~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var type = SyntaxFactory.ClassDeclaration("User");
var generator = SyntaxGenerator.Create(scope, type);
var id = SyntaxGenerator.IntType.GetInitProperty("Id").Public();
generator.AddProperty(id);
var name = SyntaxGenerator.StringType.GetInitProperty("Name").Public();
generator.AddProperty(name);
~~~

#### 1.2 简单类示例生成代码如下
~~~csharp
namespace Models
{
    class User
    {
        public int Id { get; init; }
        public string Name { get; init; }
    }
}
~~~

### 2. 同作用域(命名空间)生成多个类
#### 2.1 多个类的示例
>* 一个作用域ISyntaxScope内可以包含多个类

~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var type = SyntaxFactory.ClassDeclaration("User");
var generator = SyntaxGenerator.Create(scope, type);
var userIdType = SyntaxFactory.IdentifierName("UserId");
var userIdGenerator = SyntaxGenerator.Create(generator.Container, SyntaxFactory.ClassDeclaration(userIdType.Identifier));
var userIdValue = SyntaxGenerator.IntType.GetInitProperty("Value").Public();
userIdGenerator.AddProperty(userIdValue);
var userNameType = SyntaxFactory.IdentifierName("UserName");
var userNameGenerator = SyntaxGenerator.Create(generator.Container, SyntaxFactory.ClassDeclaration(userNameType.Identifier));
var userNameValue = SyntaxGenerator.StringType.GetInitProperty("Value").Public();
userNameGenerator.AddProperty(userNameValue);
var id = userIdType.GetInitProperty("Id").Public();
generator.AddProperty(id);
var name = userNameType.GetInitProperty("Name").Public();
generator.AddProperty(name);
~~~

#### 2.2 多个类示例生成代码如下
~~~csharp
namespace Models
{
    class User
    {
        public UserId Id { get; init; }
        public UserName Name { get; init; }
    }

    class UserId
    {
        public int Value { get; init; }
    }

    class UserName
    {
        public string Value { get; init; }
    }
}
~~~

### 3. 生成嵌套内部类
#### 3.1 内部类的示例
>* 不指定命名空间也会构建作用域UnitScope(ISyntaxScope),对应语法树的CompilationUnitSyntax
>* SyntaxGenerator继承了接口ISyntaxScope
>* 以SyntaxGenerator(类)为作用域添加类就能生成内部类,SyntaxGenerator支持嵌套

~~~csharp
var outerClass = SyntaxFactory.ClassDeclaration("OuterClass");
var generator = SyntaxGenerator.Create(outerClass);
var outerValue = SyntaxGenerator.IntType.GetInitProperty("OuterValue").Public();
generator.AddProperty(outerValue);
var innerClass = SyntaxFactory.ClassDeclaration("InnerClass");
var innerGenerator = SyntaxGenerator.Create(generator, innerClass);
var innerValue = SyntaxGenerator.IntType.GetInitProperty("InnerValue").Public();
innerGenerator.AddProperty(innerValue);
~~~

#### 3.2 内部类示例生成代码如下
~~~csharp
class OuterClass
{
    public int OuterValue { get; init; }

    class InnerClass
    {
        public int InnerValue { get; init; }
    }
}
~~~

## 二、简单命名展示
>* 依赖nuget包: Hand.Generators.EasySyntax --version 0.3.1.1-alpha
>* 使用SyntaxGenerator.Simplify简化类型名

### 1. 自动using
#### 1.1 自动using命名空间示例
~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var type = SyntaxFactory.ClassDeclaration("Comment");
var generator = SyntaxGenerator.Create(scope, type);
var id = generator.Simplify(SyntaxGenerator.GuidType).GetInitProperty("Id").Public();
generator.AddProperty(id);
var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
generator.AddProperty(content);
var addTime = generator.Simplify(SyntaxGenerator.DateTimeType).GetInitProperty("AddTime").Public();
generator.AddProperty(addTime);
~~~

#### 1.2 自动using命名空间示例生成代码
>* 自动using命名System
>* 相同命名空间自动排重

~~~csharp
using System;

namespace Models
{
    class Comment
    {
        public Guid Id { get; init; }
        public string Content { get; init; }
        public DateTime AddTime { get; init; }
    }
}
~~~

### 2. 相同命名空间
#### 2.1 相同命名空间示例
~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var userIdType = SyntaxFactory.IdentifierName("UserId").Qualify(scope);
var userNameType = SyntaxFactory.IdentifierName("UserName").Qualify(scope);
var type = SyntaxFactory.ClassDeclaration("User");
var generator = SyntaxGenerator.Create(scope, type);
var id = generator.Simplify(userIdType).GetInitProperty("Id").Public();
generator.AddProperty(id);
var name = generator.Simplify(userNameType).GetInitProperty("Name").Public();
generator.AddProperty(name);
~~~

#### 2.2 相同命名空间示例生成代码
>* 相同命名不用增加using

~~~csharp
namespace Models
{
    class User
    {
        public UserId Id { get; init; }
        public UserName Name { get; init; }
    }
}
~~~

### 3. 同名类处理
#### 3.1 同名类示例
~~~csharp
var customerType = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("Customers"));
var managerType = SyntaxFactory.IdentifierName("User").Qualify(SyntaxFactory.IdentifierName("Managers"));
var type = SyntaxFactory.ClassDeclaration("Flow");
var generator = SyntaxGenerator.Create(SyntaxFactory.IdentifierName("OA"), type);
var submitter = generator.Simplify(customerType).GetInitProperty("Submitter").Public();
generator.AddProperty(submitter);
var approver = generator.Simplify(managerType).GetInitProperty("Approver").Public();
generator.AddProperty(approver);
~~~

#### 3.2 同名类示例生成代码
>* Customers.User提交,简化为User并添加using Customers;
>* 管理用户审批,由于User已占用只能全名展示

~~~csharp
using Customers;

namespace OA
{
    class Flow
    {
        public User Submitter { get; init; }
        public Managers.User Approver { get; init; }
    }
}
~~~

### 4. 同名类异常情况
#### 4.1 同名类异常示例
~~~csharp
var customerScope = SyntaxFactory.IdentifierName("Customers");
var customerId = SyntaxFactory.IdentifierName("UserId").Qualify(customerScope);
var customerName = SyntaxFactory.IdentifierName("UserName").Qualify(customerScope);
var managerScope = SyntaxFactory.IdentifierName("Managers");
var managerId = SyntaxFactory.IdentifierName("UserId").Qualify(managerScope);
var managerName = SyntaxFactory.IdentifierName("UserName").Qualify(managerScope);
var type = SyntaxFactory.ClassDeclaration("Flow");
var generator = SyntaxGenerator.Create(SyntaxFactory.IdentifierName("OA"), type);
var submitterId = generator.Simplify(customerId).GetInitProperty("SubmitterId").Public();
generator.AddProperty(submitterId);
var approverName = generator.Simplify(managerName).GetInitProperty("ApproverName").Public();
generator.AddProperty(approverName);
var submitterName = generator.Simplify(customerName).GetInitProperty("SubmitterId").Public();
generator.AddProperty(submitterName);
var approverId = generator.Simplify(managerId).GetInitProperty("SubmitterId").Public();
generator.AddProperty(approverId);
~~~

#### 4.2 同名类异常示例生成代码
>* 虽然代码可以生成,但生成的代码无法通过编译
>* SubmitterId和ApproverName的类重名导致不明确

~~~csharp
using Customers;
using Managers;

namespace OA
{
    class Flow
    {
        public UserId SubmitterId { get; init; }
        public UserName ApproverName { get; init; }
        public Customers.UserName SubmitterId { get; init; }
        public Managers.UserId SubmitterId { get; init; }
    }
}
~~~

### 5. 简单命名功能总结
>* 集成在SyntaxGenerator中,自动适应当前类型作用域(命名空间)
>* 使用简单
>* 生成代码最简洁
>* 有同名类(不同命名空间)冲突机制
>* 相同命名空间多个同名类还是可能生成编译无法通过的代码
>* 包含命名空间下未使用的同名类也可能导致无法编译

## 三、差异命名展示
>* 依赖开源项目nuget包: Hand.Generators.Tidy --version 0.3.1.1-alpha

### 1. 命名空间不同
#### 1.1 命名空间不同示例
>* 按命名空间生成作用域TypeSimplifier(ITypeSimplifier)对象
>* Simplify方法根据两个命名空间最短距离简化类名展示

~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var type = SyntaxFactory.ClassDeclaration("Comment");
var generator = SyntaxGenerator.Create(scope, type);
var simplifier = TypeSimplifier.Create(scope);
var id = simplifier.Simplify(SyntaxGenerator.GuidType).GetInitProperty("Id").Public();
generator.AddProperty(id);
var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
generator.AddProperty(content);
var addTime = simplifier.Simplify(SyntaxGenerator.DateTimeType).GetInitProperty("AddTime").Public();
generator.AddProperty(addTime);
~~~

#### 1.2 命名空间不同示例生成代码
>* 命名空间Models和System没有相同的根,无法简化,完整展示

~~~csharp
namespace Models
{
    class Comment
    {
        public System.Guid Id { get; init; }
        public string Content { get; init; }
        public System.DateTime AddTime { get; init; }
    }
}
~~~

### 2. 命名空间相同
#### 2.1 命名空间相同示例
~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var type = SyntaxFactory.ClassDeclaration("Comment");
var generator = SyntaxGenerator.Create(scope, type);
var simplifier = TypeSimplifier.Create(scope);
var id = simplifier.Simplify(SyntaxGenerator.GuidType).GetInitProperty("Id").Public();
generator.AddProperty(id);
var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
generator.AddProperty(content);
var addTime = simplifier.Simplify(SyntaxGenerator.DateTimeType).GetInitProperty("AddTime").Public();
generator.AddProperty(addTime);
~~~

#### 2.2 命名空间相同示例生成代码
>* User的作用域与UserId和UserName重叠,命名空间完全简化

~~~csharp
namespace Models
{
    class User
    {
        public UserId Id { get; init; }
        public UserName Name { get; init; }
    }
}
~~~

### 3. 命名空间包含
#### 3.1 命名空间包含示例
~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var userIdType = SyntaxFactory.IdentifierName("UserId").Qualify(scope);
var scope2 = SyntaxFactory.IdentifierName("CMS").Qualify(scope);
var type = SyntaxFactory.ClassDeclaration("Comment");
var generator = SyntaxGenerator.Create(scope2, type);
var simplifier = TypeSimplifier.Create(scope2);
var userId = simplifier.Simplify(userIdType).GetInitProperty("UserId").Public();
generator.AddProperty(userId);
var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
generator.AddProperty(content);
~~~

#### 3.2 命名空间包含示例生成代码
>* Comment作用域Models.CMS包含于UserId的作用域Models,UserId的命名空间可以完全简化

~~~csharp
namespace Models.CMS
{
    class Comment
    {
        public UserId UserId { get; init; }
        public string Content { get; init; }
    }
}
~~~

### 4. 命名空间部分包含
#### 4.1 命名空间部分包含示例
~~~csharp
var scope0 = SyntaxFactory.IdentifierName("Models");
var scope1 = SyntaxFactory.IdentifierName("Accounts").Qualify(scope0);
var userIdType = SyntaxFactory.IdentifierName("UserId").Qualify(scope1);
var scope2 = SyntaxFactory.IdentifierName("CMS").Qualify(scope0);
var type = SyntaxFactory.ClassDeclaration("Comment");
var generator = SyntaxGenerator.Create(scope2, type);
var simplifier = TypeSimplifier.Create(scope2);
var userId = simplifier.Simplify(userIdType).GetInitProperty("UserId").Public();
generator.AddProperty(userId);
var content = SyntaxGenerator.StringType.GetInitProperty("Content").Public();
generator.AddProperty(content);
~~~

#### 4.2 命名空间部分包含示例生成代码
>* Comment和UserId有公共的根Models,公共部分可以简化

~~~csharp
namespace Models.CMS
{
    class Comment
    {
        public Accounts.UserId UserId { get; init; }
        public string Content { get; init; }
    }
}
~~~

### 5. 差异命名功能总结
>* 这里的差异命名是vs的代码提示几乎一致
>* 存在以上情况Visual Studio也会把可以简化部分变灰并给出提示

## 四、全局命名展示
>* 全局命名展示功能也包含在nuget包: Hand.Generators.Tidy --version 0.3.1.1-alpha

### 1. 简单全名
#### 1.1 简单全名示例
~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var userIdType = SyntaxFactory.IdentifierName("UserId");
var userNameType = SyntaxFactory.IdentifierName("UserName");
var type = SyntaxFactory.ClassDeclaration("User");
var generator = SyntaxGenerator.Create(scope, type);
var simplifier = GlobalScope.Instance;
var id = simplifier.Simplify(userIdType).GetInitProperty("Id").Public();
generator.AddProperty(id);
var name = simplifier.Simplify(userNameType).GetInitProperty("Name").Public();
generator.AddProperty(name);
~~~

#### 1.2 简单全名示例生成代码
~~~csharp
namespace Models
{
    class User
    {
        public global::UserId Id { get; init; }
        public global::UserName Name { get; init; }
    }
}
~~~

### 2. 泛型全名
#### 2.1 泛型全名示例
~~~csharp
var scope = SyntaxFactory.IdentifierName("Models");
var orderItemType = SyntaxFactory.IdentifierName("OrderItem").Qualify(scope);
var orderItemsType = SyntaxGenerator.ListType(orderItemType);
var type = SyntaxFactory.ClassDeclaration("Order");
var generator = SyntaxGenerator.Create(scope, type);
var simplifier = GlobalScope.Instance;
var items = simplifier.Simplify(orderItemsType).GetInitProperty("Items").Public();
generator.AddProperty(items);
~~~

#### 2.2 泛型全名示例生成代码
~~~csharp
namespace Models
{
    class Order
    {
        public global::System.Collections.Generic.List<global::Models.OrderItem> Items { get; init; }
    }
}
~~~

## 五、示例说明
### 1. 示例执行说明
>* 以上代码执行依赖开源项目Hand.Generators.EasySyntax和Hand.Generators.Tidy
>* Hand.Generators.EasySyntax --version 0.3.1.1-alpha
>* Hand.Generators.Tidy --version 0.3.1.1-alpha

### 2. Hand.Generators.EasySyntax和Hand.Generators.Tidy示例代码及开源源码
>* github仓库: https://github.com/donetsoftwork/HandCore.net
>* gitee同步更新: https://gitee.com/donetsoftwork/HandCore.net

## 六、最佳实践总结
### 1. 两种作用域
>* ISyntaxScope作用域用来包含类
>* ITypeSimplifier作用域用来计算命名(类及命空间的路径)
>* 其中SyntaxGenerator实现的以上两个接口

### 2. 三种命名方式对比
>* 以下是三种命名规则的对比
>* 哪种规则更适合需要结合具体情况
>* 当然多种命名方式混合使用也是可以的

|命名方式|简洁度|安全性|
|:--|:--|:--|
|简单命名|最简洁|重名类过多风险搞|
|相对命名|较为简洁|也存在重名类风险,风险较小|
|全局命名|不简洁|没有风险|
