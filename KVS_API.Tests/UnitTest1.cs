using KVS_API.Models;
using Xunit;
using Xunit.Abstractions;

namespace KVS_API.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public void Test1()
    {
        Konto privat = new Privatkonto();
        _output.WriteLine(privat.Kontonummer);
    }
}
