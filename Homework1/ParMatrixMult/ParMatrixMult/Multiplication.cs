// <copyright file="Multiplication.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace ParMatrixMult;

/// <summary>
/// Class with multiplication functions.
/// </summary>
public class Multiplication
{
    /// <summary>
    /// Multiplies matrices in parallel.
    /// </summary>
    /// <param name="first">The first matrix.</param>
    /// <param name="second">The second matrix.</param>
    /// <returns>Matrix of multiplication.</returns>
    public Matrix ParallelMult(Matrix first, Matrix second)
    {
        var newMatrix = new Matrix();
        var threads = new Thread[first.GetRows()];
        for (int i = 0; i < threads.Length; ++i)
        {
            var currentIndex = i;
            threads[i] = new Thread(() => this.MultiplyRow(first, second, newMatrix, currentIndex));
            threads[i].Start();
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
    public Matrix SequentialMult(Matrix first, Matrix second)
    {
        var newMatrix = new Matrix();
        for (int i = 0; i < first.GetRows(); ++i)
        {
            for (int j = 0; j < second.GetColumns(); ++j)
            {
                int sum = 0;
                for (int k = 0; k < first.GetColumns(); ++k)
                {
                    sum += first.GetMatrix()[i, k] * second.GetMatrix()[k, j];
                }

                newMatrix.GetMatrix()[i, j] = sum;
            }
        }

        return newMatrix;
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

            result.GetMatrix()[index, i] = sum;
        }

        System.Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} finished the row number {index + 1}!");
    }
}