// <copyright file="ThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool.Tests;

using System.Collections.Concurrent;
using System.Diagnostics;

public class ThreadPool
{
    private MyyThreadPool threadPool;

    [SetUp]
    public void SetUp() => this.threadPool = new MyyThreadPool(Environment.ProcessorCount);

    [TearDown]
    public void TearDown() => this.threadPool.Dispose();

    [Test]
    public void Test_CreateThreadPoolWithNegativeThreads() => Assert.Throws<ArgumentOutOfRangeException>(() => new MyyThreadPool(-10));

    [Test]
    public void Test_CreateThreadPoolWithValidNumberOfThreads()
    {
        var threadCount = 4;
        var pool = new MyyThreadPool(threadCount);
        Assert.That(pool.IsShutdown, Is.False);
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
        this.threadPool.Shutdown();
        Assert.That(this.threadPool.IsShutdown, Is.True);
    }

    [Test]
    public void Test_IsCompleted()
    {
        var task = this.threadPool.Submit<int>(() => 2 * 3);
        var result = task.Result;
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

    [Test]
    public void Test_ThreadPoolUsesMultiThreads()
    {
        var threadCount = 4;
        var pool = new MyyThreadPool(threadCount);
        var tasks = new IMyTask<int>[threadCount];
        var threadIds = new ConcurrentBag<int>();

        for (int i = 0; i < threadCount; ++i)
        {
            tasks[i] = pool.Submit<int>(() =>
            {
                Thread.Sleep(50);
                var threadId = Thread.CurrentThread.ManagedThreadId;
                threadIds.Add(threadId);
                return threadId;
            });
        }

        var results = new List<int>();
        foreach (var task in tasks)
        {
            results.Add(task.Result);
        }

        var distinctThreads = results.Distinct().Count();
        Assert.That(distinctThreads, Is.GreaterThan(1), "Tasks should execute in parallel, not sequentially");

        pool.Dispose();
    }

    [Test]
    public void Test_ConcurrentTaskSubmission_ThreadSafety()
    {
        var taskCount = 50;
        var results = new ConcurrentBag<int>();
        var tasks = new List<Task>();
        for (int i = 0; i < 10; ++i)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < taskCount / 10; j++)
                {
                var task = this.threadPool.Submit<int>(() =>
                {
                    Thread.SpinWait(1000);
                    return Thread.CurrentThread.ManagedThreadId;
                });
                results.Add(task.Result);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
        Assert.That(results.Count, Is.EqualTo(taskCount));
    }
}
