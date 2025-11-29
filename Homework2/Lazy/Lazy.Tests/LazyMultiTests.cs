// <copyright file="LazyMultiTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Lazy.Tests;

[TestFixture]
public class LazyMultiTests : LazyCommonTests
{
    [Test]
    public void Test_SupplierCallOnce()
    {
        var threadCount = 20;
        var callCount = 0;
        var barrier = new Barrier(threadCount);

        var lazy = this.CreateLazy<int>(() =>
        {
            Interlocked.Increment(ref callCount);
            return 10;
        });

        var results = new int[threadCount];
        var threads = new Thread[threadCount];

        for (int i = 0; i < threadCount; ++i)
        {
            int index = i;
            threads[index] = new Thread(() =>
            {
                barrier.SignalAndWait();
                results[index] = lazy.Get();
            });

            threads[index].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.Multiple(() =>
        {
            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(results, Is.All.EqualTo(10));
        });
    }

    /// <summary>
    /// when an exception occurs in the supplier, it is called once, and all threads receive the exception.
    /// </summary>
    [Test]
    public void Test_SupplierIsInvokedOnceOnException()
    {
        var threadCount = 20;
        var exception = "Error, Exception!";
        var callCount = 0;
        var barrier = new Barrier(threadCount);
        var lazy = this.CreateLazy<int>(() =>
        {
            Interlocked.Increment(ref callCount);
            throw new InvalidOperationException(exception);
        });

        var exceptions = new Exception[threadCount];
        var threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; ++i)
        {
            int index = i;
            threads[index] = new Thread(() =>
            {
                barrier.SignalAndWait();
                try
                {
                    lazy.Get();
                }
                catch (Exception ex)
                {
                    exceptions[index] = ex;
                }
            });
            threads[index].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.Multiple(() =>
        {
            Assert.That(callCount, Is.EqualTo(1));
            Assert.That(exceptions, Is.All.InstanceOf<InvalidOperationException>());
            Assert.That(exceptions.Select(e => e.Message), Is.All.EqualTo(exception));
        });
    }

    protected override ILazy<T> CreateLazy<T>(Func<T> supplier) => new MultiThreadLazy<T>(supplier);
}