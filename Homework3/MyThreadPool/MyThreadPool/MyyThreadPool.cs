// <copyright file="MyyThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Class for thread pool.
/// </summary>
public class MyyThreadPool : IDisposable
{
    private readonly ManualResetEventSlim manualResetEvent = new(false);
    private readonly Thread[] threads;
    private readonly object lockObject = new object();
    private readonly CancellationTokenSource cts;
    private ConcurrentQueue<Action> taskQueue;
    private int activeThreads;
    private volatile bool isShutdown = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyyThreadPool"/> class.
    /// </summary>
    /// <param name="numberThreads">The number of threads for simultaneous operation.</param>
    /// <exception cref="InvalidOperationException">If number of threads is less than or equal to zero.</exception>
    public MyyThreadPool(int numberThreads)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(numberThreads, 0);

        this.threads = new Thread[numberThreads];
        this.cts = new();
        this.taskQueue = new();
        this.activeThreads = 0;

        for (int i = 0; i < numberThreads; ++i)
        {
            this.threads[i] = new Thread(() => this.Run());
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Gets a value indicating whether the thread pool has been shut down.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsShutdown => this.isShutdown;

    /// <summary>
    /// Shuts down threads.
    /// </summary>
    public void Shutdown()
    {
        lock (this.lockObject)
        {
            if (this.isShutdown)
            {
                return;
            }

            this.isShutdown = true;
        }

        this.manualResetEvent.Set();
        foreach (var thread in this.threads)
        {
            thread.Join();
        }

        while (this.taskQueue.TryDequeue(out var task))
        {
            try
            {
                task();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task failed during shutdown: {ex.Message}");
            }
        }

        this.cts.Cancel();
    }

    /// <summary>
    /// Create and queuing task.
    /// </summary>
    /// <typeparam name="TResult">The type of data.</typeparam>
    /// <param name="function">The function given for calculating.</param>
    /// <returns>Current task.</returns>
    /// <exception cref="InvalidOperationException">If thread pool is shut down.</exception>
    /// <exception cref="ArgumentNullException">If function is null.</exception>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        ArgumentNullException.ThrowIfNull(function);

        lock (this.lockObject)
        {
            if (this.isShutdown)
            {
                throw new InvalidOperationException("Thread pool is shut down.");
            }
        }

        var task = new MyTask<TResult>(function, this);
        lock (this.lockObject)
        {
            if (this.isShutdown)
            {
                throw new InvalidOperationException("Thread pool is shut down.");
            }

            this.EnqueueTask(() => task.Run());
        }

        this.manualResetEvent.Set();
        return task;
    }

    /// <summary>
    /// Adds an item to the pool queue.
    /// </summary>
    /// <param name="task">Current task.</param>
    public void EnqueueTask(Action task)
    {
        ArgumentNullException.ThrowIfNull(task);

        lock (this.lockObject)
        {
            if (this.isShutdown)
            {
                throw new InvalidOperationException("Thread pool is shut down.");
            }

            this.taskQueue.Enqueue(task);
        }

        this.manualResetEvent.Set();
    }

    /// <summary>
    /// Ensures that threads are terminated and resources are freed.
    /// </summary>
    public void Dispose()
    {
        this.Shutdown();
        this.cts.Dispose();
        this.manualResetEvent.Dispose();
    }

    private void Run()
    {
        while (true)
        {
            lock (this.lockObject)
            {
                if (this.isShutdown && this.taskQueue.IsEmpty)
                {
                    break;
                }
            }

            if (this.taskQueue.TryDequeue(out var taskRun))
            {
                Interlocked.Increment(ref this.activeThreads);
                try
                {
                    taskRun();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Task failed: {ex.Message}");
                }
                finally
                {
                    Interlocked.Decrement(ref this.activeThreads);
                }
            }
            else
            {
                this.manualResetEvent.Wait(1000);
                this.manualResetEvent.Reset();
            }
        }
    }
}