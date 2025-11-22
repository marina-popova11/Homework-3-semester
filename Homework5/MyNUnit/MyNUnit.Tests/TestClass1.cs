// <copyright file="TestClass1.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit.Tests;

using Attributes;

public class TestClass1
{
    [BeforeClass]
    public static void BeforeClassTest()
    {
        Console.WriteLine("BeforeClass attribute in class!");
    }

    [AfterClass]
    public static void AfterClassTest()
    {
        Console.WriteLine("AfterClass attribute in class!");
    }

    [Before]
    public void BeforeTest()
    {
        Console.WriteLine("Before each test!");
    }

    [Test]
    public void Test_SimplePassedTest()
    {
        Console.WriteLine("This test should pass");
    }

    [Test]
    public void Test_FailedTest()
    {
        Console.WriteLine("This test will fail");
        throw new Exception("This test failed intentionally");
    }

    [After]
    public void AfterTest()
    {
        Console.WriteLine("After each test!");
    }
}
