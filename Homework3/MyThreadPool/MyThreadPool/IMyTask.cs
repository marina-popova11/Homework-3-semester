// <copyright file="IMyTask.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether. Return true if the task is completed.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Gets task completion result.
    /// </summary>
    TResult Result { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    IMyTask<TNewResult> ContinueWith(Func<TResult, TNewResult> func);

}