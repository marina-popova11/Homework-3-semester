// <copyright file="MyThreadPool.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Class for thread pool.
/// </summary>
/// <typeparam name="TResult">The type of result data.</typeparam>
public class MyThreadPool<TResult>
{
    private readonly Thread[] threads;
    private readonly object lockObject = new object();
    private Queue<Action> taskQueue = new();
    private int numberThreads;
    private bool expectation;
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

        this.expectation = true;
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
}