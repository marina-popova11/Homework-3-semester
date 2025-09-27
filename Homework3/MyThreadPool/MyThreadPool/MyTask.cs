// <copyright file="MyTask.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

internal class MyTask<TResult> : IMyTask<TResult>
{
    private readonly object lockObject = new object();
    private readonly Func<TResult> function;
    private TResult result = default!;
    private volatile bool isCompleted = false;
    private Exception exception = null!;
    private MyyThreadPool threadPool;
    private List<Action> followingTasks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResults}"/> class.
    /// </summary>
    /// <param name="function">The function given for calculating.</param>
    /// <param name="threadPool">The thread pool.</param>
    public MyTask(Func<TResult> function, MyyThreadPool threadPool)
    {
        if (function == null || threadPool == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        this.function = function;
        this.threadPool = threadPool;
    }

    /// <summary>
    /// Gets a value indicating whether. Returns true if the task is completed.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsCompleted => this.isCompleted;

    /// <summary>
    /// Gets the result of the task execution.
    /// </summary>
    /// <returns>The result of the task execution.</returns>
    /// <exception cref="AggregateException">If the function corresponding to the task has terminated with an exception.</exception>
    public TResult Result
    {
        get
        {
            if (!this.isCompleted)
            {
                lock (this.lockObject)
                {
                    while (!this.isCompleted)
                    {
                        Monitor.Wait(this.lockObject);
                    }
                }
            }

            if (this.exception != null)
            {
                throw new AggregateException("Task failed", this.exception);
            }

            return this.result;
        }
    }

    /// <summary>
    /// Returns an element that can itself become a new task.
    /// </summary>
    /// <param name="nextFunction">An object of type Func that can be applied
    /// to the result of a given task X and returns a new task Y that has been
    /// accepted for execution.</param>
    /// <returns>Element that can itself become a new task.</returns>
    /// <exception cref="InvalidOperationException">If thread pool is shut down.</exception>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> nextFunction)
    {
        if (nextFunction == null)
        {
            throw new ArgumentNullException(nameof(nextFunction));
        }

        if (this.threadPool.IsShitDown())
        {
            throw new InvalidOperationException("Thread pool is shut down.");
        }

        return this.threadPool.CreatingQueuingTask(() => nextFunction(this.Result));

        // var nextTask = new MyTask<TNewResult>(() => nextFunction(this.result), this.threadPool);
        // lock (this.lockObject)
        // {
        //     if (!this.isCompleted)
        //     {
        //         this.followingTasks.Add(() => nextTask.Run());
        //     }
        //     else
        //     {
        //         this.threadPool.CreatingQueuingTask(() => nextTask.Run());
        //     }
        // }

        // return nextTask;
    }

    /// <summary>
    /// Performs the task.
    /// </summary>
    public void Run()
    {
        try
        {
            this.result = this.function();
        }
        catch (Exception ex)
        {
            this.exception = ex;
        }
        finally
        {
            lock (this.lockObject)
            {
                this.isCompleted = true;
                foreach (var task in this.followingTasks)
                {
                    this.threadPool.EnqueueTask(task);
                }

                this.followingTasks.Clear();

                Monitor.PulseAll(this.lockObject);
            }
        }
    }
}