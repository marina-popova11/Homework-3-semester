// <copyright file="FailedTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit.FakeTests;

using MyNUnit.Attributes;

public class FailedTests
{
    [Test]
    public void Test_AlwaysFails()
    {
        throw new Exception("Expected failure");
    }

    [Test]
    public void Test_AssertionFailure()
    {
        var num = 1;
        throw new InvalidOperationException($"Assertion failed: expected 2, got {num}");
    }
}
