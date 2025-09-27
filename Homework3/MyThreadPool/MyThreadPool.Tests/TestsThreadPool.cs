// <copyright file="TestsThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool.Tests;

public class TestsThreadPool
{
    private MyThreadPool threadPool;

    [SetUp]
    public void SetUp()
    {
        this.threadPool = new MyThreadPool(Environment.ProcessorCount);
    }
    [Test]
    public void Test_ThrownArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new My)
    }
}
