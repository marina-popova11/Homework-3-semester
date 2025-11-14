// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using MyNUnit;

if (args.Length == 0)
{
    Console.WriteLine("Specify the path to the assemblies!");
    return;
}

var path = args[0];
if (!File.Exists(path))
{
    Console.WriteLine("There are no builds on this path.");
    return;
}

var runner = new Runner();
try
{
    var report = runner.TestRunner(path);
    Environment.Exit(report.FailedCount() > 0 ? 1 : 0);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Environment.Exit(2);
}