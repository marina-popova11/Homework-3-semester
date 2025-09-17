// <copyright file="LazyCommonTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Lazy.Tests;

public abstract class LazyCommonTests
{
    [Test]
    public void Test_GetReturnsValue()
    {
        var lazy = this.CreateLazy(() => 10);
        Assert.That(lazy.Get(), Is.EqualTo(42));
    }

    [Test]
    public void Test_GetSupplierThrownException()
    {
        var exception = new InvalidDataException("Exception!");
        var lazy = this.CreateLazy<int>(() => throw exception);
        Assert.Throws<InvalidDataException>(() => lazy.Get());
    }

    protected abstract ILazy<T> CreateLazy<T>(Func<T> supplier);
}
