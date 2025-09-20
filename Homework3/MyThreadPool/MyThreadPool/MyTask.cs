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
    private MyThreadPool<TResult> threadPool;
    private List<Action> followingTasks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResults}"/> class.
    /// </summary>
    /// <param name="function">The function given for calculating.</param>
    public MyTask(Func<TResult> function)
    {
        this.function = function;
    }

    TResult IMyTask<TResult>.Result => throw new NotImplementedException();

    /// <summary>
    /// Returns true if the task is completed.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsCompleted() => this.isCompleted;

    /// <summary>
    /// Returns the result of the task execution.
    /// </summary>
    /// <returns>The result of the task execution.</returns>
    /// <exception cref="AggregateException">If the function corresponding to the task has terminated with an exception.</exception>
    public TResult Result()
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

    /// <summary>
    /// Returns an element that can itself become a new task.
    /// </summary>
    /// <param name="nextFunction">An object of type Func that can be applied
    /// to the result of a given task X and returns a new task Y that has been
    /// accepted for execution.</param>
    /// <returns>Element that can itself become a new task.</returns>
    public IMyTask<TResult> ContinueWith(Func<TResult, TResult> nextFunction)
    {
        var nextTask = new MyTask<TResult>(() => nextFunction(Result), this.threadPool);
        lock (this.lockObject)
        {
            if (!this.isCompleted)
            {
                this.followingTasks.Add(nextTask.Execute());
            }
            else
            {
                this.threadPool.Enqueue(() => nextTask.Execute);
            }
        }

        return nextTask;
    }

    /// <summary>
    /// Performs the task.
    /// </summary>
    private void Execute()
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
                    try
                    {
                        this.threadPool.Enqueue(task);
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }

                this.followingTasks.Clear();

                Monitor.PulseAll(this.lockObject);
            }
        }
    }

    public IMyTask<TNewResult> ContinueWith(Func<TResult, TNewResult> func)
    {
        throw new NotImplementedException();
    }
}