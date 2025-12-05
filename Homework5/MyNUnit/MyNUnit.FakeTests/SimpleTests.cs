// <copyright file="SimpleTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit.FakeTests;

using MyNUnit.Attributes;

public class SimpleTests
{
    [Test]
    public void Test_WithoutSomething()
    {
    }

    [Test]
    public void Test2()
    {
        int x = 1 + 1;
        Console.WriteLine($"Result: {x}");
    }

    [Test]
    public void Test_WithLogic()
    {
        var list = new List<int> { 1, 2, 3 };
        var sum = list.Sum();
        
        if (sum != 6)
        {
            throw new Exception($"Expected sum 6, got {sum}");
        }
    }
}