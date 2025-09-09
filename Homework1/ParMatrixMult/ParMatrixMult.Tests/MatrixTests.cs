// <copyright file="MatrixTests.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace ParMatrixMult.Tests;

public class MatrixTests()
{
    [Test]
    public void Test_CreateMatrix()
    {
        var matrix = new Matrix(10, 12);
        Assert.That(matrix.GetRows(), Is.EqualTo(10));
        Assert.That(matrix.GetColumns(), Is.EqualTo(12));
    }

    [Test]
    public void Test_ChangingTheValuesOfTheMatrixParameters()
    {
        var matrix = new Matrix(10, 12);
        matrix.SetRows(30);
        matrix.SetColumns(10);
        Assert.That(matrix.GetRows(), Is.EqualTo(30));
        Assert.That(matrix.GetColumns(), Is.EqualTo(10));
    }

    [Test]
    public void Test_ThrowsFileNotFoundException()
    {
        var matrix = new Matrix();
        Assert.Throws<FileNotFoundException>(() => matrix.GetMatrixFromFile("filename.txt"));
    }
}