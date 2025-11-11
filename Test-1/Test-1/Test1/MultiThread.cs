// <copyright file="MultiThread.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test1;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Multithreaded implementation of calculating the checksum of a directory.
/// </summary>
public class MultiThread : ICheckSum
{
    private readonly SemaphoreSlim semaphore;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiThread"/> class.
    /// </summary>
    /// <param name="maxDegree">The maximum degree of parallelism.</param>
    public MultiThread(int maxDegree = -1)
    {
        this.semaphore = new SemaphoreSlim(maxDegree == -1 ? Environment.ProcessorCount : maxDegree);
    }

    /// <summary>
    /// Calculates the MD5 hash for the specified directory and all its subdirectories and files using a multi-threaded approach.
    /// </summary>
    /// <param name="directoryPath">Directory path for calculating the hash.</param>
    /// <returns>MD5 hash in the form of an array of bytes.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown if the specified directory does not exist.</exception>
    public async Task<byte[]> CalculateDirectoryHashAsync(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        return await this.CalculateHashForDirectoryAsync(directoryPath);
    }

    /// <summary>
    /// Frees up the resources used by the calculator.
    /// </summary>
    public void Dispose()
    {
        this.semaphore?.Dispose();
    }

    private async Task<byte[]> CalculateHashForDirectoryAsync(string directoryPath)
    {
        var directoryName = Path.GetFileName(directoryPath);
        var directoryNameBytes = Encoding.UTF8.GetBytes(directoryName);

        using var md5 = MD5.Create();
        md5.TransformBlock(directoryNameBytes, 0, directoryNameBytes.Length, null, 0);

        var subdirectories = Directory.GetDirectories(directoryPath).OrderBy(d => d, StringComparer.Ordinal).ToArray();

        var files = Directory.GetFiles(directoryPath).OrderBy(f => f, StringComparer.Ordinal).ToArray();

        var subdirectoryTasks = subdirectories.Select(async subdirectory =>
        {
            await this.semaphore.WaitAsync();
            try
            {
                return await this.CalculateHashForDirectoryAsync(subdirectory);
            }
            finally
            {
                this.semaphore.Release();
            }
        }).ToArray();

        var subdirectoryHashes = await Task.WhenAll(subdirectoryTasks);

        foreach (var hash in subdirectoryHashes)
        {
            md5.TransformBlock(hash, 0, hash.Length, null, 0);
        }

        var fileTasks = files.Select(async file =>
        {
            await this.semaphore.WaitAsync();
            try
            {
                return await this.CalculateHashForFileAsync(file);
            }
            finally
            {
                this.semaphore.Release();
            }
        }).ToArray();

        var fileHashes = await Task.WhenAll(fileTasks);

        foreach (var hash in fileHashes)
        {
            md5.TransformBlock(hash, 0, hash.Length, null, 0);
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