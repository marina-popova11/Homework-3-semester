// <copyright file="ICheckSum.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test1;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Interface for calculating the checksum of a file system directory.
/// </summary>
public interface ICheckSum
{
    /// <summary>
    /// Calculates the MD5 hash for the specified directory and all its subdirectories and files.
    /// </summary>
    /// <param name="directoryPath">Directory path for calculating the hash.</param>
    /// <returns>MD5 hash in the form of an array of bytes.</returns>
    Task<byte[]> CalculateDirectoryHashAsync(string directoryPath);
}
