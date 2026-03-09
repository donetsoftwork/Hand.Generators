using Hand;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace EasySyntaxTests;

public class LockTests
{
    [Fact]
    public void Lock()
    {
        var listType = SyntaxGenerator.Generic("List", SyntaxGenerator.IntType);
        var list = SyntaxFactory.IdentifierName("list");
        var _list = SyntaxFactory.IdentifierName("_list");
        var value = SyntaxFactory.IdentifierName("value");
        var _listAdd = _list.Access("Add");
        var _listContains = _list.Access("Contains");

        // List<int> list
        var parameter = listType.Parameter(list.Identifier);
        // private readonly _list = list;
        var field = listType.Field(_list.Identifier, list)
            .Private()
            .ReadOnly();
        // void Add(int value)
        var method = SyntaxGenerator.VoidType.Method("Add", SyntaxGenerator.IntType.Parameter(value.Identifier))
            // {
            .ToBuilder()
            // if(_list.Contains(value))
            .If(_listContains.Invocation([value]))
                // return
                .Return()
            // lock(_list){
            .Lock(_list)
                // if(_list.Contains(value))
                .If(_listContains.Invocation([value]))
                    // return
                    .Return()
                // _list.Add(value)
                .Add(_listAdd.Invocation([value]))
                // }
                .End()
            // }
            .End()
            // public
            .Public();

        var type = SyntaxFactory.ClassDeclaration("SafeList")
            .AddParameterListParameters(parameter)
            .AddMembers(field, method)
            .Public();
        var code = type.NormalizeWhitespace().ToFullString();
        Assert.NotEmpty(code);
    }

    //public class SafeList(List<int> list)
    //{
    //    private readonly List<int> _list = list;
    //    public void Add(int value)
    //    {
    //        if (_list.Contains(value))
    //            return;
    //        lock (_list)
    //        {
    //            if (_list.Contains(value))
    //                return;
    //            _list.Add(value);
    //        }
    //    }
    //}
}
