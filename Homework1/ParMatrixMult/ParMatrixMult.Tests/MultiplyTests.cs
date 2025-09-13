// <copyright file="MultiplyTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace ParMatrixMult.Tests;

public class MultiplyTests
{
    private static Matrix firstMatrix;
    private static Matrix secondMatrix;
    private static Multiplication multiply = new();

    [SetUp]
    public static void SetUp()
    {
        firstMatrix = multiply.CreateRandomMatrix(15, 20);
        secondMatrix = multiply.CreateRandomMatrix(20, 30);
    }

    [Test]
    public void Test_CompareMatricesDimIfTrue()
    {
        Assert.That(multiply.CompareMatrixDim(firstMatrix, secondMatrix), Is.True);
    }

    [Test]
    public void Test_Test_CompareMatricesDimIfFalse()
    {
        firstMatrix.SetColumns(25);
        Assert.That(multiply.CompareMatrixDim(firstMatrix, secondMatrix), Is.False);
    }

    [Test]
    public void Test_SequentialMultiply()
    {
        var newMatrix = multiply.SequentialMultiply(firstMatrix, secondMatrix);
        Assert.That(newMatrix.Rows, Is.EqualTo(15));
        Assert.That(newMatrix.Columns, Is.EqualTo(30));
    }

    [Test]
    public void Test_ParallelMultiply()
    {
        var newMatrix = multiply.ParallelMultiply(firstMatrix, secondMatrix, Environment.ProcessorCount);
        Assert.That(newMatrix.Rows, Is.EqualTo(15));
        Assert.That(newMatrix.Columns, Is.EqualTo(30));
    }
}
