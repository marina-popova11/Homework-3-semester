// <copyright file="LazyMultiTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Lazy.Tests;

[TestFixture]
public class LazyMultiTests : LazyCommonTests
{
    [Test]
    public void Test_GetForMultiALLThreadGetSameExceptions()
    {
        var threadCount = 20;
        var exceptionCount = 0;
        var exception = new InvalidDataException("Exception!");
        var barrier = new Barrier(threadCount);

        var lazy = this.CreateLazy<int>(() =>
        {
            Interlocked.Increment(ref exceptionCount);
            throw exception;
        });

        var newExceptions = new Exception[threadCount];
        var threads = new Thread[threadCount];

        for (int i = 0; i < threadCount; ++i)
        {
            int index = i;
            threads[i] = new Thread(() =>
            {
                barrier.SignalAndWait();
                try
                {
                    lazy.Get();
                }
                catch (Exception ex)
                {
                    newExceptions[index] = ex;
                }
            });

            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.Multiple(() =>
        {
            Assert.That(exceptionCount, Is.EqualTo(1));
            Assert.That(newExceptions, Is.All.InstanceOf<InvalidDataException>());
            Assert.That(newExceptions.Select(e => e.Message), Is.All.EqualTo("Exception!"));
        });
    }

    protected override ILazy<T> CreateLazy<T>(Func<T> supplier) => new MultiThreadLazy<T>(supplier);
}