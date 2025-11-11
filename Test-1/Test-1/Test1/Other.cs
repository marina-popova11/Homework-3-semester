// <copyright file="Other.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test1;

/// <summary>
/// Class for auxiliary functions.
/// </summary>
public static class Other
{
    /// <summary>
    /// Converts a byte array to a hexadecimal string.
    /// </summary>
    /// <param name="hash">Byte array for conversion.</param>
    /// <returns>Hexadecimal representation of the hash.</returns>
    /// <exception cref="ArgumentNullException">If byte array is null.</exception>
    public static string ToHexString(this byte[] hash)
    {
        if (hash == null)
        {
            throw new ArgumentNullException(nameof(hash));
        }

        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }

    /// <summary>
    /// Formats a time interval in a readable format (milliseconds or seconds).
    /// </summary>
    /// <param name="timeSpan">Time interval for formatting.</param>
    /// <returns>A string with formatted time.</returns>
    public static string FormatElapsedTime(this TimeSpan timeSpan)
    {
        return timeSpan.TotalMilliseconds < 1000 ? $"{timeSpan.TotalMilliseconds:F2} ms" : $"{timeSpan.TotalSeconds:F2} s";
    }
}
