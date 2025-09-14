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
    Console.WriteLine("For performance tests: ParMatrixMult");
    Console.WriteLine("For file multiplication: ParMatrixMult matrix1.txt matrix2.txt result.txt");
}

static void MultiplyTwoMatrices(string file1, string file2, string outputFile)
{
    try
    {
        var firstMatrix = Matrix.GetMatrixFromFile(file1);

        var secondMatrix = Matrix.GetMatrixFromFile(file2);

        var result = new Multiplication();
        if (!result.CompareMatrixDim(firstMatrix, secondMatrix))
        {
            throw new InvalidOperationException(
                $"Matrices are incompatible for multiplication: " +
                $"Matrix1 [{firstMatrix.Rows}x{firstMatrix.Columns}] and " +
                $"Matrix2 [{secondMatrix.Rows}x{secondMatrix.Columns}]");
        }

        Console.WriteLine("Sequential matrix multiplication: ");
        Matrix sequentialMatrix = result.SequentialMultiply(firstMatrix, secondMatrix);
        Console.WriteLine($"Result: Matrix [{sequentialMatrix.Rows}x{sequentialMatrix.Columns}]");

        Console.WriteLine("Parallel matrix multiplication: ");
        Matrix parallelMatrix = result.ParallelMultiply(firstMatrix, secondMatrix, 4);
        Console.WriteLine($"Result: Matrix [{parallelMatrix.Rows}x{parallelMatrix.Columns}]");

        if ((sequentialMatrix.Rows == parallelMatrix.Rows) && (sequentialMatrix.Columns == parallelMatrix.Columns))
        {
            parallelMatrix.PutMatrixToFile(outputFile);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
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
    writer.WriteLine("Dimensions            \tSequential(ms)\tParallel(ms)\tSpeedup\tMean Sequential\tStd Dev Sequential\tMean Parallel\tStd Dev Parallel\t");
    foreach (var (rows, columns, common) in testMatrices)
    {
        Console.WriteLine($"[{rows}x{common}] * [{common}x{columns}];");
        var multiply = new Multiplication();
        var matrix1 = multiply.CreateRandomMatrix(rows, common);
        var matrix2 = multiply.CreateRandomMatrix(common, columns);

        var (sequentialTime, meanSequential, stdDevSequential) = multiply.PerformanceMeasurement(
        () => multiply.SequentialMultiply(matrix1, matrix2), runs);

        var (parallelTime, meanParallel, stdDevParallel) = multiply.PerformanceMeasurement(
        () => multiply.ParallelMultiply(matrix1, matrix2, Environment.ProcessorCount), runs);

        double speedup = sequentialTime / parallelTime;

        writer.WriteLine(
            $"[{rows}x{common}] * [{common}x{columns}]" +
            $"\t{sequentialTime,12:F2}" +
            $"\t{parallelTime,12:F2}" +
            $"\t{speedup,8:F2}x" +
            $"\t{meanSequential,8:F2}" +
            $"\t{stdDevSequential,8:F2}" +
            $"\t{meanParallel,8:F2}" +
            $"\t{stdDevParallel,8:F2}");
        Console.WriteLine($"Sequential:    {sequentialTime:F2}ms");
        Console.WriteLine($"Sequential mean and standard deviation:   {meanSequential:F2}, {stdDevSequential:F2}");
        Console.WriteLine($"Parallel:   {parallelTime:F2}ms");
        Console.WriteLine($"Parallel mean and standard deviation:   {meanParallel:F2}, {stdDevParallel:F2}");
        Console.WriteLine($"Speedup:    {speedup:F2}x");
    }
}