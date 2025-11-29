// <copyright file="LazySingleTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Lazy.Tests;

[TestFixture]
public class LazySingleTests : LazyCommonTests
{
    [Test]
    public void Test_GetForSingleThreadSupplierCallOnce()
    {
        var increasingSuppliersCall = 0;
        var lazy = this.CreateLazy(() =>
        {
            ++increasingSuppliersCall;
            return "Test";
        });

        for (int i = 0; i < 10; ++i)
        {
            lazy.Get();
        }

        Assert.That(increasingSuppliersCall, Is.EqualTo(1));
    }

    protected override ILazy<T> CreateLazy<T>(Func<T> supplier) => new SingleThreadLazy<T>(supplier);
}