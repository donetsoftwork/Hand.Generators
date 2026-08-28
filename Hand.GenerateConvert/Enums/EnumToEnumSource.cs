using Hand.Builders;
using Hand.Converters;
using Hand.Sources;
using Hand.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hand.Enums;

/// <summary>
/// 枚举转枚举
/// </summary>
/// <param name="compilation"></param>
/// <param name="sourceType"></param>
/// <param name="destInfo"></param>
/// <param name="methodName"></param>
/// <param name="sourceBundle"></param>
/// <param name="destBundle"></param>
public class EnumToEnumSource(Compilation compilation, TypeSyntax sourceType, EnumTypeInfo destInfo, string methodName, IEnumBundle sourceBundle, IEnumBundle destBundle)
    : MethodSource(compilation, methodName, sourceType, destInfo)
{
    #region 配置
    private readonly EnumTypeInfo _destInfo = destInfo;
    private readonly IEnumBundle _sourceBundle = sourceBundle;
    private readonly IEnumBundle _destBundle = destBundle;
    private static readonly SimpleNameSyntax _hasFlagMethod = SyntaxFactory.IdentifierName(nameof(HasFlag));
    #endregion

    /// <inheritdoc />
    public override MethodDeclarationSyntax BuildBody(SyntaxGenerator generator, MethodDeclarationSyntax method, ExpressionSyntax @this)
    {        
        if (_sourceBundle.HasFlag && _sourceBundle is FlagEnumBundle sourceFlagBundle)
        {
            if (_destBundle.HasFlag && _destBundle is FlagEnumBundle destFlagBundle)
                return FlagToFlag(generator, method, @this, sourceFlagBundle, destFlagBundle);
            return FlagToEnum(generator, method, @this, sourceFlagBundle.Fields, _destBundle.Fields);
        }
        return EnumToEnum(generator, method, @this, [.. _sourceBundle.Fields], _destBundle.Fields);
    }
    /// <summary>
    /// 调用HasFlag方法
    /// </summary>
    /// <param name="this"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static ExpressionSyntax HasFlag(ExpressionSyntax @this, ExpressionSyntax flag)
        => @this.Access(_hasFlagMethod).Invocation([flag]);
    /// <summary>
    /// 位域转位域
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="method"></param>
    /// <param name="this"></param>
    /// <param name="sourceBundle"></param>
    /// <param name="destBundle"></param>
    /// <returns></returns>
    public MethodDeclarationSyntax FlagToFlag(SyntaxGenerator generator, MethodDeclarationSyntax method, ExpressionSyntax @this, FlagEnumBundle sourceBundle, FlagEnumBundle destBundle)
    {
        var builder = method.ToBuilder();
        var result = SyntaxFactory.IdentifierName("result");
        // ulong result = 0UL;
        builder.Declare(SyntaxGenerator.ULongType.Variable(result.Identifier, SyntaxGenerator.Literal(0UL)));
        //var sourceType
        foreach (var sourceField in sourceBundle.Fields)
        {
            if (sourceField.Flag == 0UL)
                continue;
            var destFields = MapFlag(sourceField, destBundle);
            if (destFields.Length == 0)
                continue;
            var flag = destFields.Aggregate(0UL, (a, b) => a | b.Flag);
            if (flag == 0UL)
                continue;
            // if(this & source.Expression == source.Expression)
            //      result |= dest.Expression;
            var expression = sourceField.GetExpression(_thisType);
            builder.If(HasFlag(@this, expression))
                 .AddExpression(result.OrAssign(SyntaxGenerator.Literal(flag)))
                 .End();
        }
        var returnType = generator.Display(_returnInfo);
        // return (TEnum)result;
        return builder.Return(CastConverter.Convert(result, returnType))
            .WithSummary(ConvertBuilder.GetMethodSummary(_returnInfo));
    }
    /// <summary>
    /// 位域转枚举
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="method"></param>
    /// <param name="this"></param>
    /// <param name="sourceFields"></param>
    /// <param name="destFields"></param>
    /// <returns></returns>
    public MethodDeclarationSyntax FlagToEnum(SyntaxGenerator generator, MethodDeclarationSyntax method, ExpressionSyntax @this, List<FlagEnumField> sourceFields, IEnumerable<IEnumField> destFields)
    {
        var builder = method.ToBuilder();
        var memberCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var members = new List<FlagEnumField>(sourceFields.Count);
        var enumType = generator.Display(_returnInfo);
        foreach (var sourceField in sourceFields)
        {
            if (sourceField.Flag == 0UL)
                continue;
            var name = sourceField.Name;
            if (destFields.FirstOrDefault(item => item.Match(name)) is IEnumField destField)
            {
                // if(this & source.Expression == source.Expression)
                //      return dest.Expression;
                var expression = sourceField.GetExpression(_thisType);
                builder.If(HasFlag(@this, expression))
                     .Return(destField.GetExpression(enumType));
                memberCheck.Add(name);
                continue;
            }
            var member = sourceField.Member;
            if (string.IsNullOrEmpty(member) || memberCheck.Contains(member))
                continue;
            members.Add(sourceField);
        }
        foreach (var sourceField in members)
        {
            var member = sourceField.Member;
            if (memberCheck.Contains(member))
                continue;
            if (destFields.FirstOrDefault(item => item.MatchMember(member)) is IEnumField destField)
            {
                // if(this & source.Expression == source.Expression)
                //      return dest.Expression;
                var expression = sourceField.GetExpression(_thisType);
                builder.If(HasFlag(@this, expression))
                     .Return(destField.GetExpression(enumType));
                memberCheck.Add(member);
                continue;
            }
        }
        // return default;
        return builder.Return(SyntaxGenerator.DefaultLiteral)
            .WithSummary(ConvertBuilder.GetMethodSummary(_destInfo));
    }
    /// <summary>
    /// 枚举转枚举
    /// </summary>
    /// <param name="generator"></param>
    /// <param name="method"></param>
    /// <param name="this"></param>
    /// <param name="sourceFields"></param>
    /// <param name="destFields"></param>
    /// <returns></returns>
    public MethodDeclarationSyntax EnumToEnum(SyntaxGenerator generator, MethodDeclarationSyntax method, ExpressionSyntax @this, IEnumField[] sourceFields, IEnumerable<IEnumField> destFields)
    {
        var memberCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var members = new List<IEnumField>(sourceFields.Length);
        // @this switch {
        var @switch = @this.SwitchExpression();
        var enumType = generator.Display(_returnInfo);
        foreach (var sourceField in sourceFields)
        {
            var name = sourceField.Name;
            if (destFields.FirstOrDefault(item => item.Match(name)) is IEnumField destField)
            {
                // sourceField.Expression => destField.Expression,
                @switch.Case(sourceField.GetExpression(_thisType), destField.GetExpression(enumType));
                memberCheck.Add(name);
                continue;
            }
            var member = sourceField.Member;
            if (string.IsNullOrEmpty(member) || memberCheck.Contains(member))
                continue;
            members.Add(sourceField);
        }
        foreach (var sourceField in members)
        {
            var member = sourceField.Member;
            if (memberCheck.Contains(member))
                continue;
            if (destFields.FirstOrDefault(item => item.MatchMember(member)) is IEnumField destField)
            {
                // sourceField.Expression => destField.Expression,
                @switch.Case(sourceField.GetExpression(_thisType), destField.GetExpression(enumType));
                memberCheck.Add(member);
                continue;
            }
        }
        // _ => default }
        var expression = @switch.Default(SyntaxGenerator.DefaultLiteral)
            .Build();
        return method.WithExpressionBody(expression)
            .WithSummary(ConvertBuilder.GetMethodSummary(_destInfo));
    }

    ///// <summary>
    ///// 按源字段映射到目标字段(优先Member)
    ///// </summary>
    ///// <param name="sourceField"></param>
    ///// <param name="destBundle"></param>
    ///// <returns></returns>
    //private static IEnumField? Map(IEnumField sourceField, IEnumBundle destBundle)
    //    => destBundle.GetFieldByName(sourceField.Name) ?? destBundle.GetFieldByMemberName(sourceField.Member);
    /// <summary>
    /// 按源字段映射到位域字段
    /// </summary>
    /// <param name="sourceField"></param>
    /// <param name="destBundle"></param>
    /// <returns></returns>
    private static FlagEnumField[] MapFlag(FlagEnumField sourceField, FlagEnumBundle destBundle)
        => [.. destBundle.GetFieldsByName(sourceField.Name, sourceField.Member)];
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceFields"></param>
    /// <param name="destFields"></param>
    /// <returns></returns>
    public static Dictionary<FlagEnumField, EnumField> MapFlag(List<FlagEnumField> sourceFields, List<EnumField> destFields)
    {
        var memberCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var members = new List<FlagEnumField>(sourceFields.Count);
        var map = new Dictionary<FlagEnumField, EnumField>();
        foreach (var sourceField in sourceFields)
        {
            if (sourceField.Flag == 0UL)
                continue;
            var name = sourceField.Name;
            if (destFields.FirstOrDefault(item => item.Match(name)) is EnumField destField)
            {
                map[sourceField] = destField;
                memberCheck.Add(name);
                continue;
            }
            var member = sourceField.Member;
            if (string.IsNullOrEmpty(member) || memberCheck.Contains(member))
                continue;
            members.Add(sourceField);
        }
        foreach (var sourceField in members)
        {
            var member = sourceField.Member;
            if(memberCheck.Contains(member))
                continue;
            if (destFields.FirstOrDefault(item => item.MatchMember(member)) is EnumField destField)
            {
                map[sourceField] = destField;
                memberCheck.Add(member);
                continue;
            }
        }
        return map;
    }
}