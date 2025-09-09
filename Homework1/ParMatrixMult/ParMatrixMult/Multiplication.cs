// <copyright file="Multiplication.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace ParMatrixMult;

using System.Diagnostics;
using System.Net.NetworkInformation;

/// <summary>
/// Class with multiplication functions.
/// </summary>
public class Multiplication
{
    /// <summary>
    /// Compare two matrix`s dims to multiply them.
    /// </summary>
    /// <param name="first">The first matrix.</param>
    /// <param name="second">The second matrix.</param>
    /// <returns>True if their dimensions match, False otherwise.</returns>
    /// <exception cref="ArgumentException">If matrices` sizes doesn`t match.</exception>
    public bool CompareMatrixDim(Matrix first, Matrix second)
    {
        if (first.GetColumns() != second.GetRows())
        {
            throw new ArgumentException();
        }

        return true;
    }

    /// <summary>
    /// Multiplies matrices in parallel.
    /// </summary>
    /// <param name="first">The first matrix.</param>
    /// <param name="second">The second matrix.</param>
    /// <param name="maxThread">The max number of parallel threads.</param>
    /// <returns>Matrix of multiplication.</returns>
    /// <exception cref="ArgumentException">If matrices` sizes doesn`t match.</exception>
    public Matrix ParallelMultiply(Matrix first, Matrix second, int maxThread)
    {
        if (!this.CompareMatrixDim(first, second))
        {
            throw new ArgumentException();
        }

        var newMatrix = new Matrix(first.GetRows(), second.GetColumns());
        var semaphore = new SemaphoreSlim(maxThread, maxThread);
        var threads = new List<Thread>();
        for (int i = 0; i < first.GetRows(); ++i)
        {
            int currentIndex = i;
            var thread = new Thread(() =>
            {
                semaphore.Wait(); // Wait permissions from the semaphore before work, until at least one permitted thread is executed
                try
                {
                    this.MultiplyRow(first, second, newMatrix, currentIndex);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            threads.Add(thread);
            thread.Start(); // Here thread start his work which i specified it in line 28
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return newMatrix;
    }

    /// <summary>
    /// Multiplies matrices sequentially.
    /// </summary>
    /// <param name="first">The first matrix.</param>
    /// <param name="second">The second matrix.</param>
    /// <returns>Matrix of multiplication.</returns>
    /// <exception cref="ArgumentException">If matrices` sizes doesn`t match.</exception>
    public Matrix SequentialMultiply(Matrix first, Matrix second)
    {
        if (!this.CompareMatrixDim(first, second))
        {
            throw new ArgumentException();
        }

        var newMatrix = new Matrix(first.GetRows(), second.GetColumns());
        for (int i = 0; i < first.GetRows(); ++i)
        {
            for (int j = 0; j < second.GetColumns(); ++j)
            {
                int sum = 0;
                for (int k = 0; k < first.GetColumns(); ++k)
                {
                    sum += first.GetMatrix()[i, k] * second.GetMatrix()[k, j];
                }

                newMatrix.SetMatrix(i, j, sum);
            }
        }

        return newMatrix;
    }

    /// <summary>
    /// Create random matrix with certain count of rows and columns.
    /// </summary>
    /// <param name="rows">Rows.</param>
    /// <param name="columns">Columns.</param>
    /// <returns>random matrix.</returns>
    public Matrix CreateRandomMatrix(int rows, int columns)
    {
        var rand = new Random();
        var newMatrix = new Matrix(rows, columns);
        for (int i = 0; i < newMatrix.GetRows(); ++i)
        {
            for (int j = 0; j < newMatrix.GetColumns(); ++j)
            {
                newMatrix.SetMatrix(i, j, rand.Next(1, 20));
            }
        }

        return newMatrix;
    }

    /// <summary>
    /// Measures the multiplication performance.
    /// </summary>
    /// <param name="operation">A pointer to the function that needs to be measured.</param>
    /// <param name="runs">The number of launches from which we take the average value.</param>
    /// <returns>The average arithmetic execution time for all runs, mean and standard deviation.</returns>
    public (double AverageArithmetic, double Mean, double StdDev) PerformanceMeasurement(Func<Matrix> operation, int runs)
    {
        var measurements = new List<double>();
        operation();
        var stopwatch = new Stopwatch();
        for (int i = 0; i < runs; ++i)
        {
            stopwatch.Restart();
            operation();
            stopwatch.Stop();
            measurements.Add(stopwatch.Elapsed.TotalMilliseconds);
        }

        var (mean, stdDev) = this.CalculateMeasurements(measurements);

        return (measurements.Sum() / runs, mean, stdDev);
    }

    /// <summary>
    /// Calculate the mean and standard deviation.
    /// </summary>
    /// <param name="measurements">The list of time stamps.</param>
    /// <returns>Mean and standard deviation.</returns>
    public (double Mean, double StdDev) CalculateMeasurements(List<double> measurements)
    {
        double mean = this.CalculateMean(measurements);
        double stdDev = this.CalculateStandardDeviation(measurements);
        return (mean, stdDev);
    }

    private double CalculateMean(List<double> values)
    {
        if (values == null || values.Count() == 0)
        {
            throw new ArgumentException();
        }

        return values.Average();
    }

    private double CalculateStandardDeviation(List<double> values)
    {
        if (values == null || values.Count() < 2)
        {
            throw new ArgumentException();
        }

        var mean = this.CalculateMean(values);
        var sumOfSquaresOfDeviation = values.Select(x => Math.Pow(x - mean, 2)).Sum();
        return Math.Sqrt(sumOfSquaresOfDeviation / values.Count());
    }

    private void MultiplyRow(Matrix first, Matrix second, Matrix result, int index)
    {
        for (int i = 0; i < second.GetColumns(); ++i)
        {
            int sum = 0;
            for (int k = 0; k < first.GetColumns(); ++k)
            {
                sum += first.GetMatrix()[index, k] * second.GetMatrix()[k, i];
            }

            result.SetMatrix(index, i, sum);
        }

        System.Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished the row number {index + 1}!");
    }
}