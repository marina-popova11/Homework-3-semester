// <copyright file="LazySingleTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Lazy.Tests;

public abstract class LazySingleTests
{
    [Test]
    public void 
    protected abstract ILazy<T> CreateLazy<T>(Func<T> supplier);
}