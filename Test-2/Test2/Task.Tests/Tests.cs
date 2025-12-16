// <copyright file="Tests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>
namespace Task.Tests;

public class Tests
{
    public class TestClass1
    {
        public int Number;

        public void Method1()
        {
        }

        public string Method2(int x) => string.Empty;
    }

    public class TestClass2
    {
        public int Number;

        public void Method1()
        {
        }

        public void NewMethod()
        {
        }
    }

    [Test]
    public void Test_SimpleTest()
    {
        Assert.Pass();
    }
}
