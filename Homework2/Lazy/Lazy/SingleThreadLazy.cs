// <copyright file="SingleThreadLazy.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

/// <summary>
/// Class for Single Thread.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public class SingleThreadLazy<T> : ILazy<T>
{
    private Func<T> supplier;
    private T? value;
    private bool isValueCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleThreadLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">The supplier of function.</param>
    public SingleThreadLazy(Func<T> supplier)
    {
        this.supplier = supplier;
    }

    /// <summary>
    /// Returns the value of the element.
    /// </summary>
    /// <returns>The value of the element.</returns>
    /// <exception cref="Exception">If the value won`t be calculating.</exception>
    public T Get()
    {
        if (!this.isValueCreated)
        {
            try
            {
                this.value = this.supplier();
                this.isValueCreated = true;
                this.supplier = null!;
            }
            catch (Exception ex)
            {
                ex = new Exception();
                throw;
            }
        }

        return this.value!;
    }
}