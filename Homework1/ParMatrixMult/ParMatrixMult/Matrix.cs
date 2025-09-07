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
    /// <param name="rows">Number of rows.</param>
    /// <param name="columns">Number of columns.</param>
    public Matrix(int rows, int columns)
    {
        this.matrix = new int[rows, columns];
        this.rows = this.matrix.GetLength(0);
        this.columns = this.matrix.GetLength(1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    public Matrix()
    {
    }

    /// <summary>
    /// Returns a matrix from a file.
    /// </summary>
    /// <param name="filename">Name of file.</param>
    /// <exception cref="FileNotFoundException">If file doesn`t exist.</exception>
    public void GetMatrixFromFile(string filename)
    {
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException();
        }

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
    /// <param name="filename">Name of file.</param>
    /// <returns>True, if it is good.</returns>
    public bool PutMatrixToFile(string filename)
    {
        using var writer = new StreamWriter(filename);
        writer.Write("(");
        for (int i = 0; i < this.rows; ++i)
        {
            if (i > 0)
            {
                writer.Write("\n ");
            }

            for (int j = 0; j < this.columns; ++j)
            {
                writer.Write(this.matrix[i, j]);
                if (j <= this.columns - 1)
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
    /// Changes the value in the cell.
    /// </summary>
    /// <param name="first">First coordinate.</param>
    /// <param name="second">Second coordinate..</param>
    /// <param name="value">New value.</param>
    public void SetMatrix(int first, int second, int value) => this.matrix[first, second] = value;

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

    /// <summary>
    /// Changes the value of rows.
    /// </summary>
    /// <param name="value">New number of rows.</param>
    public void SetRows(int value) => this.rows = value;

    /// <summary>
    /// Changes the value of columns.
    /// </summary>
    /// <param name="value">New number of columns.</param>
    public void SetColumns(int value) => this.columns = value;
}
