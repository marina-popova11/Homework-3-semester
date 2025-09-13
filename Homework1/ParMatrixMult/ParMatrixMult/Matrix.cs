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
        this.matrix = new int[0, 0];
        this.rows = this.matrix.GetLength(0);
        this.columns = this.matrix.GetLength(1);
    }

    /// <summary>
    /// Gets matrix`s rows.
    /// </summary>
    public int Rows => this.rows;

    /// <summary>
    /// Gets matrix`s columns.
    /// </summary>
    public int Columns => this.columns;

    /// <summary>
    /// The index property.
    /// </summary>
    /// <param name="row">The required row.</param>
    /// <param name="column">The required column.</param>
    /// <returns>The value from the desired row and column.</returns>
    public int this[int row, int column]
    {
        get
        {
            if (row < 0 || row >= this.rows)
            {
                throw new IndexOutOfRangeException($"Row index {row} is out of range [0, {this.rows - 1}]");
            }

            if (column < 0 || column >= this.columns)
            {
                throw new IndexOutOfRangeException($"Column index {column} is out of range [0, {this.columns - 1}]");
            }

            return this.matrix[row, column];
        }

        set
        {
            if (row < 0 || row >= this.rows)
            {
                throw new IndexOutOfRangeException($"Row index {row} is out of range [0, {this.rows - 1}]");
            }

            if (column < 0 || column >= this.columns)
            {
                throw new IndexOutOfRangeException($"Column index {column} is out of range [0, {this.columns - 1}]");
            }

            this.matrix[row, column] = value;
        }
    }

    /// <summary>
    /// Returns a matrix from a file.
    /// </summary>
    /// <param name="filename">Name of file.</param>
    /// <exception cref="ArgumentException">If filename does not include characters.</exception>
    /// <exception cref="FileNotFoundException">If file doesn`t exist.</exception>
    /// <returns>Matrix.</returns>
    public static Matrix GetMatrixFromFile(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentException(nameof(filename));
        }

        if (!File.Exists(filename))
        {
            throw new FileNotFoundException();
        }

        string[] lines = File.ReadAllLines(filename);
        var rows = lines.Length;
        var columns = lines[0].Split(",", StringSplitOptions.RemoveEmptyEntries).Length;
        var matrix = new Matrix(rows, columns);
        for (int i = 0; i < rows; ++i)
        {
            string cleared = lines[i].Replace("(", string.Empty).Replace(")", string.Empty).Trim();
            string[] numbers = cleared.Split(",", StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < columns; ++j)
            {
                matrix[i, j] = int.Parse(numbers[j].Trim());
            }
        }

        return matrix;
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
    /// Changes the value of rows.
    /// </summary>
    /// <param name="value">New number of rows.</param>
    public void SetRows(int value)
    {
        if (value < 0)
        {
            throw new InvalidDataException(nameof(value));
        }

        if (value == this.rows)
        {
            return;
        }

        var newMatrix = new int[value, this.columns];
        var newRows = Math.Min(value, this.rows);

        for (int i = 0; i < newRows; ++i)
        {
            for (int j = 0; j < this.columns; ++j)
            {
                newMatrix[i, j] = this.matrix[i, j];
            }
        }

        this.matrix = newMatrix;
        this.rows = value;
    }

    /// <summary>
    /// Changes the value of columns.
    /// </summary>
    /// <param name="value">New number of columns.</param>
    public void SetColumns(int value)
    {
        if (value < 0)
        {
            throw new InvalidDataException(nameof(value));
        }

        if (value == this.columns)
        {
            return;
        }

        var newMatrix = new int[this.rows, value];
        var newColumns = Math.Min(value, this.rows);

        for (int i = 0; i < this.rows; ++i)
        {
            for (int j = 0; j < newColumns; ++j)
            {
                newMatrix[i, j] = this.matrix[i, j];
            }
        }

        this.matrix = newMatrix;
        this.columns = value;
    }
}
