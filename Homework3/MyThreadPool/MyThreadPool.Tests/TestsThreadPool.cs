// <copyright file="TestsThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool.Tests;

public class TestsThreadPool
{
    private MyyThreadPool threadPool;

    [SetUp]
    public void SetUp()
    {
        this.threadPool = new MyyThreadPool(Environment.ProcessorCount);
    }

    [TearDown]
    public void TearDown()
    {
        this.threadPool.Dispose();
    }

    [Test]
    public void Test_CreateThreadPoolWithNegativeThreads()
    {
        Assert.Throws<ArgumentNullException>(() => new MyyThreadPool(-10));
    }

    [Test]
    public void Test_ThrownArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var task = this.threadPool.Submit<int>(null!);
        });
    }

    [Test]
    public void Test_ShutDown()
    {
        this.threadPool.ShutDown();
        Assert.That(this.threadPool.IsShitDown, Is.True);
    }

    [Test]
    public void Test_IsCompleted()
    {
        var task = this.threadPool.Submit<int>(() => 2 * 3);
        Assert.That(task.IsCompleted, Is.True);
    }

    [Test]
    public void Test_TaskRun()
    {
        var task = this.threadPool.Submit<int>(() => 2 * 3);
        Assert.That(task.Result, Is.EqualTo(6));
    }

    [Test]
    public void Test_ContinueWithNormalNextFunction()
    {
        var task = this.threadPool.Submit<int>(() => 10 * 10).ContinueWith<string>(x => x.ToString());
        Assert.That(task.Result, Is.EqualTo("100"));
    }

    [Test]
    public void Test_ContinueWithNullNextFunction()
    {
        var task = this.threadPool.Submit<int>(() => 10 * 10);
        Assert.Throws<ArgumentNullException>(() => task.ContinueWith<string>(null!));
    }

    [Test]
    public void Test_ConcurrentTasksRun()
    {
        var tasks = new IMyTask<int>[10];
        for (int i = 0; i < tasks.Length; ++i)
        {
            var square = i;
            tasks[i] = this.threadPool.Submit<int>(() => square * square);
        }

        for (int i = 0; i < tasks.Length; ++i)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(i * i));
        }
    }

    [Test]
    public void Test_DisposeThreadPool()
    {
        this.threadPool.Dispose();
        Assert.Throws<InvalidOperationException>(() => this.threadPool.Submit<int>(() => 2 + 10));
    }
}
