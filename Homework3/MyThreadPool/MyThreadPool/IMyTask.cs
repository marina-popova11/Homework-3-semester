// <copyright file="IMyTask.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Class for interface My Task.
/// </summary>
/// <typeparam name="TResult">The type of data.</typeparam>
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
    /// Returns an element that can itself become a new task.
    /// </summary>
    /// <typeparam name="TNewResult">The type of data received at the end of the task.</typeparam>
    /// <param name="func">An object of type Func that can be applied
    /// to the result of a given task X and returns a new task Y that has been
    /// accepted for execution.</param>
    /// <returns>Element that can itself become a new task.</returns>
    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
}