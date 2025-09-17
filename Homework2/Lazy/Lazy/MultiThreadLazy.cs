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
    private volatile bool isComputing = false; // volatile ensures the visibility of changes between threads

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiThreadLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">The supplier of function.</param>
    public MultiThreadLazy(Func<T> supplier)
    {
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

        lock (this.lockObject) // blocked access for one thread
        {
            if (!this.isValueCreated)
            {
                this.isComputing = true;
                try
                {
                    this.value = this.supplier();
                    this.isValueCreated = true;
                    this.supplier = null!;
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