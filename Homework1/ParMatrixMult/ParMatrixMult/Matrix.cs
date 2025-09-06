// <copyright file="Matrix.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace ParMatrixMult;

/// <summary>
/// Class matrices.
/// </summary>
public class Matrix
{
    private int[,] matrix;
    private int rows;
    private int columns;

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    public Matrix()
    {
        this.matrix = new int[0, 0];
    }

    /// <summary>
    /// Compare two matrix`s dims to multiply them.
    /// </summary>
    /// <param name="first">The first matrix.</param>
    /// <param name="second">The second matrix.</param>
    /// <returns>True if their dimensions match, False otherwise.</returns>
    public bool CompareMatrixDim(Matrix first, Matrix second)
    {
        if (first.columns != second.rows)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns a matrix from a file.
    /// </summary>
    /// <param name="filename">Name of file.</param>
    public void GetMatrixFromFile(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        this.rows = lines.Length;
        this.columns = lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Length;
        this.matrix = new int[this.rows, this.columns];
        for (int i = 0; i < this.rows; ++i)
        {
            string cleared = lines[i].Replace("(", "").Replace(")", "").Trim();
            string[] numbers = cleared.Split(",", StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < this.columns; ++j)
            {
                this.matrix[i, j] = int.Parse(numbers[j].Trim());
            }
        }
    }

    /// <summary>
    /// Outputs the matrix to a file.
    /// </summary>
    /// <param name="matrix">Matrix.</param>
    /// <param name="filename">Name of file.</param>
    /// <returns>True, if it is good.</returns>
    public bool PutMatrixToFile(Matrix matrix, string filename)
    {
        using var writer = new StreamWriter(filename);
        writer.Write("(");
        for (int i = 0; i < matrix.GetRows(); ++i)
        {
            if (i > 0)
            {
                writer.Write("\n ");
            }

            for (int j = 0; j < matrix.GetColumns(); ++j)
            {
                writer.Write(matrix.GetMatrix()[i, j]);
                if (j <= matrix.GetColumns() - 1)
                {
                    writer.Write(", ");
                }
            }
        }

        writer.Write(")");
        return true;
    }

    /// <summary>
    /// Get int matrix.
    /// </summary>
    /// <returns>Two-dimensional array.</returns>
    public int[,] GetMatrix() => this.matrix;

    /// <summary>
    /// Get matrix`s rows.
    /// </summary>
    /// <returns>Rows.</returns>
    public int GetRows() => this.rows;

    /// <summary>
    /// Get matrix`s columns.
    /// </summary>
    /// <returns>Columns.</returns>
    public int GetColumns() => this.columns;
}
