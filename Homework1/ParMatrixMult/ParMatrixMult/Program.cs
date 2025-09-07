// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using ParMatrixMult;

if (args.Length == 0)
{
    RunPerformanceTests();
}
else if (args.Length == 3)
{
    MultiplyTwoMatrices(args[0], args[1], args[2]);
}
else
{
    System.Console.WriteLine("For performance tests: ParMatrixMult");
    System.Console.WriteLine("For file multiplication: ParMatrixMult matrix1.txt matrix2.txt result.txt");
}

static void MultiplyTwoMatrices(string file1, string file2, string outputFile)
{
    try
    {
        var firstMatrix = new Matrix();
        firstMatrix.GetMatrixFromFile(file1);

        var secondMatrix = new Matrix();
        secondMatrix.GetMatrixFromFile(file2);

        var result = new Multiplication();
        if (!result.CompareMatrixDim(firstMatrix, secondMatrix))
        {
            throw new InvalidOperationException(
                $"Matrices are incompatible for multiplication: " +
                $"Matrix1 [{firstMatrix.GetRows()}x{firstMatrix.GetColumns()}] and " +
                $"Matrix2 [{secondMatrix.GetRows()}x{secondMatrix.GetColumns()}]");
        }

        System.Console.WriteLine("Sequential matrix multiplication: ");
        Matrix sequentialMatrix = result.SequentialMult(firstMatrix, secondMatrix);
        System.Console.WriteLine($"Result: Matrix [{sequentialMatrix.GetRows()}x{sequentialMatrix.GetColumns()}]");

        System.Console.WriteLine("Parallel matrix multiplication: ");
        Matrix parallelMatrix = result.ParallelMult(firstMatrix, secondMatrix, 4);
        System.Console.WriteLine($"Result: Matrix [{parallelMatrix.GetRows()}x{parallelMatrix.GetColumns()}]");

        if ((sequentialMatrix.GetRows() == parallelMatrix.GetRows()) && (sequentialMatrix.GetColumns() == parallelMatrix.GetColumns()))
        {
            parallelMatrix.PutMatrixToFile(outputFile);
        }
    }
    catch (Exception ex)
    {
        System.Console.WriteLine($"Error: {ex.Message}");
    }
}

static void RunPerformanceTests()
{
    var testMatrices = new[]
    {
        (100, 100, 100),
        (500, 500, 500),
        (1000, 1000, 1000),
    };

    var runs = 5;
    var threads = Environment.ProcessorCount;

    using var writer = new StreamWriter("performanceResults.txt");
    writer.WriteLine("Performance testing results: ");
    writer.WriteLine("Dimensions\tSequential(ms)\tParallel(ms)\tSpeedup");
    foreach (var (rows, columns, common) in testMatrices)
    {
        System.Console.WriteLine($"[{rows}x{common}] * [{common}x{columns}];");
        var multiply = new Multiplication();
        var matrix1 = multiply.CreateRandomMatrix(rows, common);
        var matrix2 = multiply.CreateRandomMatrix(common, columns);

        double sequentialTime = multiply.PerformanceMeasurement(
        () => multiply.SequentialMult(matrix1, matrix2), runs);

        double parallelTime = multiply.PerformanceMeasurement(
        () => multiply.ParallelMult(matrix1, matrix2, Environment.ProcessorCount), runs);

        double speedup = sequentialTime / parallelTime;

        writer.WriteLine($"[{rows}x{common}] * [{common}x{columns}]\t{sequentialTime:F2}\t{parallelTime:F2}\t{speedup:F2}x");
        Console.WriteLine($"Sequential: {sequentialTime:F2}ms ± {sequentialTime:F2}ms");
        Console.WriteLine($"Parallel:   {parallelTime:F2}ms ± {parallelTime:F2}ms");
        Console.WriteLine($"Speedup:    {speedup:F2}x");
    }
}