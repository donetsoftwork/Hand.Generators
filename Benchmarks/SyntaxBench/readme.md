# 正确使用方式

## 一、类型
### 基础类型
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("int")
>* PredefinedType代码为SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))
>* Int代码为SyntaxGenerator.Int
>* ParseTypeName性能差,内存占用大

| Method         | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| ParseTypeName  | 217.174 ns | 0.6011 ns | 0.6681 ns | 217.703 ns | 30.80 |    0.72 | 0.0575 |     992 B |       12.40 |
| PredefinedType |   7.056 ns | 0.1512 ns | 0.1681 ns |   6.929 ns |  1.00 |    0.03 | 0.0046 |      80 B |        1.00 |

### 普通类型
>* IdentifierName代码为SyntaxFactory.IdentifierName("User")
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("User")
>* ParseTypeName性能差,内存占用大

| Method         | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|--------------- |----------:|---------:|---------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| IdentifierName |  10.84 ns | 0.238 ns | 0.275 ns |  1.00 |    0.03 | 0.0083 |      - |      - |     144 B |        1.00 |
| ParseTypeName  | 508.55 ns | 0.100 ns | 0.111 ns | 46.95 |    1.15 | 0.0694 | 0.0163 | 0.0082 |    1056 B |        7.33 |

### 限定名类型
>* QualifiedName代码为SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("Models"), SyntaxFactory.IdentifierName("User"))
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("Models.User")
>* IdentifierName代码为SyntaxFactory.IdentifierName("Models.User")
>* ParseTypeName性能差,内存占用大

| Method         | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0   | Gen1   | Gen2   | Allocated | Alloc Ratio |
|--------------- |----------:|---------:|---------:|------:|--------:|-------:|-------:|-------:|----------:|------------:|
| QualifiedName  |  30.85 ns | 0.101 ns | 0.116 ns |  1.00 |    0.01 | 0.0231 |      - |      - |     400 B |        1.00 |
| ParseTypeName  | 707.37 ns | 0.029 ns | 0.033 ns | 22.93 |    0.08 | 0.0713 | 0.0194 | 0.0064 |    1120 B |        2.80 |
| IdentifierName |  10.69 ns | 0.090 ns | 0.104 ns |  0.35 |    0.00 | 0.0083 |      - |      - |     144 B |        0.36 |

### 泛型
>* GenericName代码为SyntaxFactory.GenericName("List").AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("List<int>")

| Method        | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------- |---------:|--------:|--------:|------:|--------:|-------:|-------:|----------:|------------:|
| GenericName   | 128.9 ns | 0.92 ns | 1.02 ns |  1.00 |    0.01 | 0.0607 |      - |   1.02 KB |        1.00 |
| ParseTypeName | 284.3 ns | 0.69 ns | 0.74 ns |  2.21 |    0.02 | 0.0658 | 0.0001 |   1.11 KB |        1.08 |

### 限定名泛型
>* QualifiedName代码为SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("System.Collections.Generic"), SyntaxFactory.GenericName("List").AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword))))
>* ParseTypeName代码为SyntaxFactory.ParseTypeName("System.Collections.Generic.List<int>")
>* IdentifierName代码为SyntaxFactory.GenericName("System.Collections.Generic.List").AddTypeArgumentListArguments(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))

| Method        | Mean     | Error   | StdDev  | Median   | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|-------------- |---------:|--------:|--------:|---------:|------:|--------:|-------:|-------:|----------:|------------:|
| QualifiedName | 155.1 ns | 1.99 ns | 2.30 ns | 156.3 ns |  1.00 |    0.02 | 0.0755 | 0.0001 |   1.27 KB |        1.00 |
| ParseTypeName | 423.1 ns | 5.66 ns | 6.51 ns | 423.2 ns |  2.73 |    0.06 | 0.0663 | 0.0001 |   1.12 KB |        0.88 |
| GenericName   | 131.2 ns | 1.53 ns | 1.70 ns | 129.8 ns |  0.85 |    0.02 | 0.0607 |      - |   1.02 KB |        0.80 |

## 访问成员
>* Qualified代码为SyntaxFactory.QualifiedName(owner, member)
>* Access代码为SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, owner, member)
>* 尽量使用Qualified

| Method    | Mean     | Error     | StdDev    | Ratio | Gen0   | Allocated | Alloc Ratio |
|---------- |---------:|----------:|----------:|------:|-------:|----------:|------------:|
| Qualified | 9.328 ns | 0.0473 ns | 0.0486 ns |  1.00 | 0.0065 |     112 B |        1.00 |
| Access    | 9.689 ns | 0.0834 ns | 0.0893 ns |  1.04 | 0.0065 |     112 B |        1.00 |