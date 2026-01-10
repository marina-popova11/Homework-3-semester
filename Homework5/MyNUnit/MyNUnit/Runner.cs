// <copyright file="Runner.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

using MyNUnit.Attributes;

/// <summary>
/// Class for test runner.
/// </summary>
public class Runner
{
    private Searcher searcher = new();
    private Executor executor = new();
    private Reporter reporter = new();

    /// <summary>
    /// Runs all the tests.
    /// </summary>
    /// <param name="path">The path to run assemblies.</param>
    /// <returns>The list of results.</returns>
    public Reporter TestRun(string path)
    {
        string[] allDlls = Directory.GetFiles(path, "*Tests*.dll", SearchOption.AllDirectories);
        string[] allExes = Directory.GetFiles(path, "*Tests*.exe", SearchOption.AllDirectories);
        var allAssemblies = allDlls.Concat(allExes);
        var allTests = new List<TestClassInfo>();
        foreach (var assemblyPath in allAssemblies)
        {
            var testClasses = this.searcher.TestSearch(assemblyPath);
            allTests.AddRange(testClasses);
        }

        var result = this.executor.TestExecute(allTests);
        var report = this.reporter.CreateReport(result);
        return report;
    }
}
