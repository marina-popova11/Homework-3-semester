// <copyright file="MyTask.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly object lockObject = new object();
    private readonly Func<TResult> function;
    private TResult result;
    private volatile bool isCompleted = false;
    private Exception exception;

    public MyTask(Func<TResult> function)
    {
        this.function = function;
    }

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

    public IMyTask<TNewResult> ContinueWith(Func<TResult, TNewResult> func)
    {
        throw new NotImplementedException();
    }
}