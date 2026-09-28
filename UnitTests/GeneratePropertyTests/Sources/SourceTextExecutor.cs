using Hand;
using Hand.Attributes;
using Hand.Executors;
using Hand.GenerateProperty;
using Hand.Generators;
using Hand.Naming;
using Hand.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GeneratePropertyTests.Sources;

/// <summary>
/// 实体属性执行器
/// </summary>
public class SourceTextExecutor : IGeneratorExecutor<AttributeContext>
{
    /// <inheritdoc />
    public void Execute(SourceProductionContext sourceContext, AttributeContext generatorContext)
    {
        var cancellation = sourceContext.CancellationToken;
//#if DEBUG
//        System.Diagnostics.Debugger.Launch();
//#endif
        var model = generatorContext.SemanticModel;
        //var root = generatorContext.Node.SyntaxTree.GetRoot();
        //// 查找所有类定义
        //var classDeclarations = root.DescendantNodes().OfType<TypeDeclarationSyntax>();
        //if (model.GetDeclaredSymbol(generatorContext.Node, cancellation) is not INamedTypeSymbol symbol)
        //    return;
        //if (generatorContext.Node is not AttributeSyntax attribute)
        //    return;
        if (model.GetDeclaredSymbol(generatorContext.TargetNode, cancellation) is not INamedTypeSymbol symbol)
            return;
        var compilation = model.Compilation;
        var originalSymbol = SymbolReflection.GetOriginalSymbol(compilation, symbol);
//#if DEBUG
//        System.Diagnostics.Debugger.Launch();
//#endif
        if (originalSymbol == null)
            return;
        var descriptor = CompositeProvider.DeclaredOnly(symbol);
        // 属性Original已存在,忽略
        if (descriptor.Contains("Original"))
            return;
        // 如果存在构造函数忽略
        if (descriptor.ConstructorContains([originalSymbol]))
            return;
        // 通过AttributeSyntax无法获取构造函数参数默认值
        // var rules = new SyntaxAttributeHelper(model).GetArgumentValue<string>(attribute, 0);
        var code = Build(compilation, symbol, descriptor, originalSymbol);
        sourceContext.AddSource($"{symbol.ToDisplayString()}.GenerateProperty.cs", code);
    }
    /// <summary>
    /// 定义类
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="descriptor"></param>
    /// <param name="originalSymbol"></param>
    /// <param name="rule"></param>
    public static void BuildClass(SourceTextBuilder builder, INamedTypeSymbol symbol, CompositeProvider descriptor, INamedTypeSymbol originalSymbol, PropertyRule rule)
    {
        //var symbol = descriptor.Symbol;
        var typeKind = GetTypeKind(symbol);
        var typeName = symbol.Name;
        var originalType = originalSymbol.ToDisplayString();
        // 定义类
        if (symbol.IsRecord)
        {
            if (rule.Constructor)
                builder.AppendLine($"partial {typeKind} {typeName}({originalType} Original)");
            else
                builder.AppendLine($"partial {typeKind} {typeName}");
            return;
        }
        if (rule.EqualsMethod || descriptor.MethodContains("Equals", [originalSymbol]))
        {
            builder.AppendLine($"partial {typeKind} {typeName} : IEquatable<{typeName}>");
        }
        else
        {
            builder.AppendLine($"partial {typeKind} {typeName}");
        }
    }
    /// <summary>
    /// 获取类别
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string GetTypeKind(INamedTypeSymbol symbol)
    {
        if (symbol.IsRecord)
        {
            if (symbol.TypeKind == TypeKind.Struct)
                return "record struct";
            else
                return "record";
        }
        if (symbol.TypeKind == TypeKind.Struct)
            return "struct";
        else
            return "class";
    }
    /// <summary>
    /// 定义属性(及构造函数和字段)
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="symbol"></param>
    /// <param name="originalSymbol"></param>
    /// <param name="rule"></param>
    /// <returns>成员名(有字段返回字段)</returns>
    public static string BuildProperty(SourceTextBuilder builder, INamedTypeSymbol symbol, CompositeProvider descriptor, INamedTypeSymbol originalSymbol, PropertyRule rule)
    {
        //var symbol = descriptor.Symbol;
        var typeName = symbol.Name;
        const string propertyName = "Original";
        var originalType = originalSymbol.ToDisplayString();
        if (symbol.IsRecord)
        {
            // record构造函数包含属性,忽略
            if (rule.Constructor || descriptor.Contains(propertyName))
                return propertyName;
            if (rule.Field)
            {
                // 定义字段
                if (symbol.IsReadOnly)
                    builder.BuildField("_original", originalType, "private", "readonly");
                else
                    builder.BuildField("_original", originalType, "private");
                // 定义属性
                builder.BuildPropertyByArgument(propertyName, originalType, "original", "init");
                return "_original";
            }
            else
            {
                builder.BuildPropertyByAccessor(propertyName, originalType, "get", "init");
                return propertyName;
            }
        }
        if (rule.Constructor)
        {
            if (rule.Field)
            {
                // 定义构造函数
                BuildConstructor(builder, typeName, originalType, "_original");
                // 定义字段
                if (symbol.IsReadOnly)
                    builder.BuildField("_original", originalType, "private", "readonly");
                else
                    builder.BuildField("_original", originalType, "private");
                // 定义属性
                builder.BuildPropertyByArgument(propertyName, originalType, "original", "");
                return "_original";
            }
            else
            {
                // 定义构造函数
                BuildConstructor(builder, typeName, originalType, "Original");
                builder.BuildPropertyByAccessor(propertyName, originalType, "get");
                return propertyName;
            }
        }
        if (rule.Field)
        {
            if (symbol.IsReadOnly)
                builder.BuildField("_original", originalType, "private", "readonly");
            else
                builder.BuildField("_original", originalType, "private");
            builder.BuildPropertyByArgument(propertyName, originalType, "original", "init");
            return "_original";
        }
        else
        {
            builder.BuildPropertyByAccessor("Original", originalType, "get", "init");
            return propertyName;
        }
    }
    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="descriptor"></param>
    /// <param name="originalSymbol"></param>
    /// <param name="ruleText"></param>
    /// <returns></returns>
    public static string Build(Compilation compilation, INamedTypeSymbol symbol, CompositeProvider descriptor, INamedTypeSymbol originalSymbol)
    {
        //var symbol = descriptor.Symbol;
        // 获取GenerateProperty的Rules属性
        var attribute = compilation.GetSymbol("Hand.Primitives.GeneratePropertyAttribute");
        var ruleText = SymbolAttributeHelper.GetArgumentValue<string>(symbol, attribute, 0);
        var rule = new PropertyRule(ruleText);
        var builder = new SourceTextBuilder();
        var originalType = originalSymbol.ToDisplayString();
        using (builder.NameSpace(symbol.ContainingNamespace))
        {
            // 定义类
            BuildClass(builder, symbol, descriptor, originalSymbol, rule);
            using (builder.Block())
            {
                var memberName = BuildProperty(builder, symbol, descriptor, originalSymbol, rule);
                var originalNullCondition = originalSymbol.IsNullable();
                // 定义 ToString
                if (rule.ToStringMethod)
                {
                    var toStringMethod = descriptor.MethodContains("ToString");
                    if (!toStringMethod)
                        BuildToString(builder, originalType, originalNullCondition, memberName);
                }
                // 定义 GetHashCode
                if (rule.GetHashCodeMethod && !descriptor.MethodContains("GetHashCode"))
                {
                    BuildGetHashCode(builder, originalNullCondition, memberName);
                }
                BuildEqualOperator(builder, symbol, descriptor, rule, originalNullCondition, memberName);
            }
        }

        return builder.ToString();
    }
    /// <summary>
    /// 生成构造函数
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="typeName"></param>
    /// <param name="originalType"></param>
    /// <param name="member"></param>
    public static void BuildConstructor(SourceTextBuilder builder, string typeName, string originalType, string member = "_original")
    {
        builder.BuildConstructor(typeName, "public");
        builder.AppendLineNoIndent($"({originalType} original)");
        using (builder.Block())
        {
            builder.AppendLine($"{member} = original;");
        }
    }
    /// <summary>
    /// 生成ToString
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="originalType"></param>
    /// <param name="originalNullCondition"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static void BuildToString(SourceTextBuilder builder, string originalType, bool originalNullCondition, string memberName = "_original")
    {
        builder.BuildMethod("ToString", "string", "public", "override");
        builder.AppendLineNoIndent("()");
        using (builder.Block())
        {
            if (originalType is "string")
                builder.AppendLine($"return {memberName};");
            else if (originalNullCondition)
                builder.AppendLine($"return {memberName}?.ToString();");
            else
                builder.AppendLine($"return {memberName}.ToString();");
        }
    }
    /// <summary>
    /// 生成Equals和重载运算符
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="descriptor"></param>
    /// <param name="rule"></param>
    /// <param name="originalNullCondition"></param>
    /// <param name="memberName"></param>
    public static void BuildEqualOperator(SourceTextBuilder builder, INamedTypeSymbol symbol, CompositeProvider descriptor, PropertyRule rule, bool originalNullCondition, string memberName)
    {
        //var symbol = descriptor.Symbol;
        // record默认实现Equals和重载运算符,无需生成
        if (symbol.IsRecord)
            return;
        var typeName = symbol.Name;
        var nullCondition = symbol.IsNullable();
        //bool hasEquals;
        var hasEquals = descriptor.MethodContains("Equals", []);
        // 定义 Equals
        if (rule.EqualsMethod)
        {
            if (!hasEquals)
            {
                BuildEquals(builder, typeName, nullCondition, originalNullCondition, memberName);
                hasEquals = true;
            }
            //hasEquals = true;
        }
        //else
        //{
        //    hasEquals = equalsMethod is not null;
        //}
        // 重载需要调用Equals
        if (hasEquals && rule.Operator)
        {
            builder.BuildEqualOperator(typeName, nullCondition);
        }
        
    }
    /// <summary>
    /// 生成Equals
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="typeName"></param>
    /// <param name="nullCondition"></param>
    /// <param name="originalNullCondition"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static void BuildEquals(SourceTextBuilder builder, string typeName, bool nullCondition, bool originalNullCondition, string memberName = "_original")
    {
        string[] modifiers = ["public"];
        if (nullCondition)
        {
            builder.AppendLine("#nullable enable");
            builder.BuildMethod("Equals", "bool", modifiers);
            builder.AppendLineNoIndent($"({typeName}? other)");
            builder.AppendLine("#nullable disable");
        }
        else
        {
            //struct对virtual无效
            builder.BuildMethod("Equals", "bool", modifiers);
            //struct定义Equals不加可空
            builder.AppendLineNoIndent($"({typeName} other)");
        }
        using (builder.Block())
        {
            if (nullCondition)
            {
                builder.AppendLine("if (other is null) return false;");
                builder.AppendLine("if (ReferenceEquals(this, other)) return true;");
            }
            if (originalNullCondition)
            {
                builder.AppendLine($"var otherOriginal = other.{memberName};");
                builder.AppendLine($"if ({memberName} is null)");
                using (builder.Block())
                {
                    builder.AppendLine("if (otherOriginal is null) return true;");
                    builder.AppendLine("return false;");
                }
                builder.AppendLine($"return {memberName}.Equals(otherOriginal);");
            }
            else
            {
                builder.AppendLine($"return {memberName}.Equals(other.{memberName});");
            }
        }

        builder.OverrideObjectEquals(typeName);
    }
    /// <summary>
    /// 生成GetHashCode
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="nullCondition"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static void BuildGetHashCode(SourceTextBuilder builder, bool nullCondition, string memberName = "_original")
    {
        builder.BuildMethod("GetHashCode", "int", "public", "override");
        builder.AppendLineNoIndent("()");
        using (builder.Block())
        {
            if (nullCondition)
                builder.AppendLine($"return {memberName} is null ? 0 : {memberName}.GetHashCode();");            
            else
                builder.AppendLine($"return {memberName}.GetHashCode();");
        }
    }
    ///// <summary>
    ///// 获取类型信息
    ///// </summary>
    ///// <param name="compilation"></param>
    ///// <param name="symbol"></param>
    ///// <returns></returns>
    //public static SymbolTypeDescriptor GetDescriptor(Compilation compilation, INamedTypeSymbol symbol)
    //{
    //    // 提取字段、属性、构造函数、运算符重载和方法等信息
    //    var builder = new SymbolTypeBuilder()
    //        .WithField()
    //        .WithProperty()
    //        .WithConstructor()
    //        .WithOperator()
    //        .WithMethod();
    //    return builder.Build(compilation, symbol);
    //}
}
