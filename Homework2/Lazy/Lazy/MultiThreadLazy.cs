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
        throw new NotImplementedException();
    }
}