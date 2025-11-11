// <copyright file="SingleThread.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test1;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Single-threaded implementation of calculating the checksum of a directory.
/// </summary>
public class SingleThread : ICheckSum
{
    /// <summary>
    /// Calculates the MD5 hash for the specified directory and all its subdirectories and files
    /// using a single-threaded approach.
    /// </summary>
    /// <param name="path">Directory path for calculating the hash.</param>
    /// <returns>MD5 hash in the form of an array of bytes.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown if the specified directory does not exist.</exception>
    public async Task<byte[]> CalculateDirectoryHashAsync(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        return await this.CalculateHashForDirectoryAsync(path);
    }

    private async Task<byte[]> CalculateHashForDirectoryAsync(string path)
    {
        var directoryName = Path.GetFileName(path);
        var directoryNameBytes = Encoding.UTF8.GetBytes(directoryName);

        using var md5 = MD5.Create();
        md5.TransformBlock(directoryNameBytes, 0, directoryNameBytes.Length, null, 0);

        var subdirectories = Directory.GetDirectories(path).OrderBy(d => d, StringComparer.Ordinal);

        foreach (var subdirectory in subdirectories)
        {
            var subdirectoryHash = await this.CalculateHashForDirectoryAsync(subdirectory);
            md5.TransformBlock(subdirectoryHash, 0, subdirectoryHash.Length, null, 0);
        }

        var files = Directory.GetFiles(path).OrderBy(f => f, StringComparer.Ordinal);

        foreach (var file in files)
        {
            var fileHash = await this.CalculateHashForFileAsync(file);
            md5.TransformBlock(fileHash, 0, fileHash.Length, null, 0);
        }

        md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return md5.Hash!;
    }

    private async Task<byte[]> CalculateHashForFileAsync(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var fileNameBytes = Encoding.UTF8.GetBytes(fileName);

        using var md5 = MD5.Create();
        md5.TransformBlock(fileNameBytes, 0, fileNameBytes.Length, null, 0);

        byte[] fileContent;
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
        {
            fileContent = new byte[fileStream.Length];
            await fileStream.ReadAsync(fileContent, 0, (int)fileStream.Length);
        }

        md5.TransformBlock(fileContent, 0, fileContent.Length, null, 0);
        md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return md5.Hash!;
    }
}
