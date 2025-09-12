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
    private T value;
    private bool isValueCreated;

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
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public T Get()
    {
        if (this.isValueCreated)
        {
            return this.value;
        }

        this.InitializeValue();
    }

    private T InitializeValue()
    {

    }
}