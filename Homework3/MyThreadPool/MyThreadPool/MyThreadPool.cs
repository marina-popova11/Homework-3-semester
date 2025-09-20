// <copyright file="MyThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Class for thread pool.
/// </summary>
/// <typeparam name="TResult">The type of result data.</typeparam>
public class MyThreadPool<TResult>
{
    private readonly Thread[] threads;
    private readonly object lockObject = new object();
    private readonly CancellationTokenSource cts;
    private ConcurrentQueue<Action> taskQueue;
    private int numberThreads;
    private int activeThreads;
    private volatile bool isShutDown = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool{T}"/> class.
    /// </summary>
    /// <param name="numberThreads">The number of threads for simultaneous operation.</param>
    /// <exception cref="ArgumentNullException">If number of threads is null.</exception>
    public MyThreadPool(int numberThreads)
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
    }

    /// <summary>
    /// Adds an item to the pool queue.
    /// </summary>
    /// <param name="task">Current task.</param>
    public void Enqueue(Action task)
    {
        if (this.isShutDown)
        {
            return;
        }

        this.taskQueue.Enqueue(task);
    }

    private void Run()
    {
        if (!this.cts.IsCancellationRequested)
        {
            try
            {
                if (this.taskQueue.TryDequeue(out var taskRun))
                {
                    Interlocked.Increment(ref this.activeThreads);
                    try
                    {
                        taskRun();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref this.activeThreads);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task failed: {ex.Message}");
            }
        }
    }
}