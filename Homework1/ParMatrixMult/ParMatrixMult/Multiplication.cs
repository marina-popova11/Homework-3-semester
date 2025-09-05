// <copyright file="Multiplication.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace ParMatrixMult;

/// <summary>
/// 
/// </summary>
public class Multiplication
{
    /// <summary>
    /// Multiplies matrices in parallel.
    /// </summary>
    /// <param name="first">The first matrix.</param>
    /// <param name="second">The second matrix.</param>
    /// <returns>Matrix of multiplication.</returns>
    public int[,] ParallelMult(Matrix first, Matrix second)
    {
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
}