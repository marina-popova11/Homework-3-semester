// <copyright file="Multiplication.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace ParMatrixMult;

using System.Collections.Concurrent;
using System.Diagnostics;

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
    public bool CompareMatrixDim(Matrix first, Matrix second)
    {
        if (first.Columns != second.Rows)
        {
            return false;
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

        var newMatrix = new Matrix(first.Rows, second.Columns);
        var queue = new ConcurrentQueue<int>(); // We will put the row numbers that need to be calculated in this queue. Each thread will take the next available row from this queue
        var threads = new List<Thread>();
        var completed = new ManualResetEvent(false); // It will notify the main thread that all worker threads have completed their work
        int activeThread = 0;
        int processedCount = 0; // Total number of processed rows

        for (int i = 0; i < first.Rows; ++i) // The queue contains the indices of all rows of the resulting matrix that need to be calculated
        {
            queue.Enqueue(i);
        }

        for (int i = 0; i < maxThread; ++i)
        {
            var thread = new Thread(() => // Once a thread has started and begun executing, it atomically increments the activeThread count by 1
            {
                Interlocked.Increment(ref activeThread); // Increasing the active thread counter
                while (queue.TryDequeue(out int rowIndex))
                {
                    this.MultiplyRow(first, second, newMatrix, rowIndex);
                    Interlocked.Increment(ref processedCount); // Atomic increase of the processed rows counter
                }

                if (Interlocked.Decrement(ref activeThread) == 0)
                {
                    completed.Set();
                }
            });

            threads.Add(thread);
            thread.Start();
        }

        completed.WaitOne();

        // var semaphore = new SemaphoreSlim(maxThread, maxThread);
        // var threads = new List<Thread>();
        // for (int i = 0; i < first.Rows; ++i)
        // {
        //     int currentIndex = i;
        //     var thread = new Thread(() =>
        //     {
        //         semaphore.Wait(); // Wait permissions from the semaphore before work, until at least one permitted thread is executed
        //         try
        //         {
        //             this.MultiplyRow(first, second, newMatrix, currentIndex);
        //         }
        //         finally
        //         {
        //             semaphore.Release();
        //         }
        //     });
        //     threads.Add(thread);
        //     thread.Start(); // Here thread start his work which i specified it in line 28
        // }

        // foreach (var thread in threads)
        // {
        //     thread.Join();
        // }
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

        var newMatrix = new Matrix(first.Rows, second.Columns);
        for (int i = 0; i < first.Rows; ++i)
        {
            for (int j = 0; j < second.Columns; ++j)
            {
                int sum = 0;
                for (int k = 0; k < first.Columns; ++k)
                {
                    sum += first[i, k] * second[k, j];
                }

                newMatrix[i, j] = sum;
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
        var newMatrix = new Matrix(rows, columns);
        for (int i = 0; i < newMatrix.Rows; ++i)
        {
            for (int j = 0; j < newMatrix.Columns; ++j)
            {
                newMatrix[i, j] = Random.Shared.Next(1, 20);
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
            throw new ArgumentException(nameof(values));
        }

        return values.Average();
    }

    private double CalculateStandardDeviation(List<double> values)
    {
        if (values == null || values.Count() < 2)
        {
            throw new ArgumentException(nameof(values));
        }

        var mean = this.CalculateMean(values);
        var sumOfSquaresOfDeviation = values.Select(x => Math.Pow(x - mean, 2)).Sum();
        return Math.Sqrt(sumOfSquaresOfDeviation / values.Count());
    }

    private void MultiplyRow(Matrix first, Matrix second, Matrix result, int index)
    {
        for (int i = 0; i < second.Columns; ++i)
        {
            int sum = 0;
            for (int k = 0; k < first.Columns; ++k)
            {
                sum += first[index, k] * second[k, i];
            }

            result[index, i] = sum;
        }

        System.Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished the row number {index + 1}!");
    }
}