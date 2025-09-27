// <copyright file="MyyThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Class for thread pool.
/// </summary>
// / <typeparam name="TResult">The type of result data.</typeparam>
public class MyyThreadPool : IDisposable
{
    private readonly ManualResetEventSlim manualResetEvent = new(false);
    private readonly Thread[] threads;
    private readonly object lockObject = new object();
    private readonly CancellationTokenSource cts;
    private ConcurrentQueue<Action> taskQueue;
    private int numberThreads;
    private int activeThreads;
    private volatile bool isShutDown = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyyThreadPool"/> class.
    /// </summary>
    /// <param name="numberThreads">The number of threads for simultaneous operation.</param>
    /// <exception cref="ArgumentNullException">If number of threads is null.</exception>
    public MyyThreadPool(int numberThreads)
    {
        if (numberThreads <= 0)
        {
            throw new ArgumentNullException(nameof(numberThreads));
        }

        this.threads = new Thread[this.numberThreads];
        this.numberThreads = numberThreads;
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
    /// Indicates whether there was a signal to end the work.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsShitDown() => this.isShutDown;

    /// <summary>
    /// Shuts down threads.
    /// </summary>
    public void ShutDown()
    {
        lock (this.lockObject)
        {
            if (this.isShutDown)
            {
                return;
            }

            this.isShutDown = true;
        }

        this.cts.Cancel();
        this.manualResetEvent.Set();

        foreach (var thread in this.threads)
        {
            thread.Join();
        }
    }

    /// <summary>
    /// Create and queuing task.
    /// </summary>
    /// <typeparam name="TResult">The type of data.</typeparam>
    /// <param name="function">The function given for calculating.</param>
    /// <returns>Current task.</returns>
    /// <exception cref="InvalidOperationException">If thread pool is shut down.</exception>
    /// <exception cref="ArgumentNullException">If function is null.</exception>
    public IMyTask<TResult> CreatingQueuingTask<TResult>(Func<TResult> function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        lock (this.lockObject)
        {
            if (this.isShutDown)
            {
                throw new InvalidOperationException("Thread pool is shut down.");
            }
        }

        var task = new MyTask<TResult>(function, this);
        this.EnqueueTask(() => task.Run());
        return task;
    }

    /// <summary>
    /// Adds an item to the pool queue.
    /// </summary>
    /// <param name="task">Current task.</param>
    public void EnqueueTask(Action task)
    {
        if (this.isShutDown || this.cts.Token.IsCancellationRequested)
        {
            throw new InvalidOperationException("Thread pool is shut down.");
        }

        this.taskQueue.Enqueue(task);
        this.manualResetEvent.Set();
    }

    /// <summary>
    /// Ensures that threads are terminated and resources are freed.
    /// </summary>
    public void Dispose()
    {
        this.ShutDown();
        this.cts.Dispose();
        this.manualResetEvent.Dispose();
    }

    private void Run()
    {
        while (!this.isShutDown && !this.cts.Token.IsCancellationRequested)
        {
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