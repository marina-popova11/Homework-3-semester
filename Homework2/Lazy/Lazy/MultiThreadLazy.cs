// <copyright file="MultiThreadLazy.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

/// <summary>
/// Class for Multi Thread.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class MultiThreadLazy<T> : ILazy<T>
{
    private Func<T> supplier;
    private T? value;
    private bool isValueCreated;
    private object lockObject = new();
    private Exception? exception;
    private volatile bool isComputing = false; // volatile ensures the visibility of changes between threads

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiThreadLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">The supplier of function.</param>
    /// <exception cref="ArgumentNullException">If supplier is a null function.</exception>
    public MultiThreadLazy(Func<T> supplier)
    {
        if (supplier == null)
        {
            throw new ArgumentNullException(nameof(supplier));
        }

        this.supplier = supplier;
    }

    /// <summary>
    /// Returns the value of the element.
    /// </summary>
    /// <returns>the value.</returns>
    public T Get()
    {
        if (this.isValueCreated)
        {
            return this.value!;
        }

        if (this.exception != null)
        {
            throw this.exception;
        }

        lock (this.lockObject) // blocked access for one thread
            {
                if (!this.isValueCreated)
                {
                    this.isComputing = true;
                    if (this.exception != null)
                    {
                        throw this.exception;
                    }

                    try
                    {
                        this.value = this.supplier();
                        this.isValueCreated = true;
                        this.supplier = null!;
                    }
                    catch (Exception ex)
                    {
                        this.exception = ex;
                        throw;
                    }
                    finally
                    {
                        this.isComputing = false;
                    }
                }
            }

        while (this.isComputing)
        {
            Thread.Yield();
        }

        return this.value!;
    }
}