// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using System.Diagnostics;
using Test1;

if (args.Length == 0)
{
    Console.WriteLine("Usage: DirectoryHashCalculator <directory_path>");
    return;
}

var directoryPath = args[0];

if (!Directory.Exists(directoryPath))
{
    Console.WriteLine($"Directory not found: {directoryPath}");
    return;
}

Console.WriteLine($"Calculating hash for directory: {directoryPath}");
Console.WriteLine();

var singleThreadCalculator = new SingleThread();
var singleThreadStopwatch = Stopwatch.StartNew();
var singleThreadHash = await singleThreadCalculator.CalculateDirectoryHashAsync(directoryPath);
singleThreadStopwatch.Stop();

Console.WriteLine($"Single-threaded result: {singleThreadHash.ToHexString()}");
Console.WriteLine($"Single-threaded time: {singleThreadStopwatch.Elapsed.FormatElapsedTime()}");
Console.WriteLine();

var multiThreadCalculator = new MultiThread();
var multiThreadStopwatch = Stopwatch.StartNew();
var multiThreadHash = await multiThreadCalculator.CalculateDirectoryHashAsync(directoryPath);
multiThreadStopwatch.Stop();

Console.WriteLine($"Multi-threaded result: {multiThreadHash.ToHexString()}");
Console.WriteLine($"Multi-threaded time: {multiThreadStopwatch.Elapsed.FormatElapsedTime()}");
Console.WriteLine();

Console.WriteLine($"Hashes match: {singleThreadHash.SequenceEqual(multiThreadHash)}");
Console.WriteLine($"Speedup: {(double)singleThreadStopwatch.ElapsedMilliseconds / multiThreadStopwatch.ElapsedMilliseconds:F2}x");
