// <copyright file="LazyCommonTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Lazy.Tests;

[TestFixture]
public abstract class LazyCommonTests
{
    [Test]
    public void Test_GetReturnsValue()
    {
        var lazy = this.CreateLazy(() => 10);
        Assert.That(lazy.Get(), Is.EqualTo(10));
    }

    [Test]
    public void Test_GetSupplierThrownException()
    {
        var exception = new InvalidDataException("Exception!");
        var lazy = this.CreateLazy<int>(() => throw exception);
        Assert.Throws<InvalidDataException>(() => lazy.Get());
    }

    [Test]
    public void Test_GetReturnNull()
    {
        var lazy = this.CreateLazy<string>(() => null!);
        Assert.That(lazy.Get(), Is.Null);
    }

    [Test]
    public void Test_GetForSameObjectsSameGets()
    {
        var lazy = this.CreateLazy<object>(() => new object());
        var firstObject = lazy.Get();
        var secondObject = lazy.Get();
        Assert.That(firstObject, Is.EqualTo(secondObject));
    }

    [Test]
    public void Test_ConstructorArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => this.CreateLazy<string>(null!));
    }

    protected abstract ILazy<T> CreateLazy<T>(Func<T> supplier);
}
