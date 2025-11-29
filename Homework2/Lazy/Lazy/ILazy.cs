// <copyright file="ILazy.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

/// <summary>
/// A class for the Lazy interface that uses single- and multi-threaded function calls.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Returns the value of the element.
    /// </summary>
    /// <returns>The value of the element.</returns>
    T Get();
}