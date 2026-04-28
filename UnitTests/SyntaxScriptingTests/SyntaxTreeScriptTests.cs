using Hand;

namespace SyntaxScriptingTests;

public class SyntaxTreeScriptTests
{
    [Fact]
    public async Task ExecuteAsync()
    {
        var result = await SyntaxTreeDriver.ScriptDriver.ExecuteAsync<int>("2+3");
        Assert.Equal(5, result);
    }
    [Fact]
    public async Task ScriptExecuteAsync()
    {
        var script = SyntaxTreeDriver.ScriptDriver
            .CreateScript<int>("2+3");
        var result = await script.ExecuteAsync();
        Assert.Equal(5, result);
    }
    [Fact]
    public async Task GlobalsAsync()
    {
        var source = "x+y";
        var driver = SyntaxTreeDriver.ScriptDriver;
        var tree = driver.Parse(source);
        Assert.NotNull(tree);
        var result = await driver.ExecuteAsync<int>(tree, globals: new Globals() { x = 2, y = 3 });
        Assert.Equal(5, result);
    }
    [Fact]
    public async Task ScriptGlobalsAsync()
    {
        var source = "return x+y;";
        var driver = SyntaxTreeDriver.DefaultDriver;
        var tree = driver.Parse(source);
        var script = driver.CreateScript<int>(tree, globalsType: typeof(Globals));
        var result = await script.ExecuteAsync(globals: new Globals() { x = 2, y = 3 });
        Assert.Equal(5, result);
    }

}

public class Globals
{
    public int x { get; set; }
    public int y { get; set; }
}
