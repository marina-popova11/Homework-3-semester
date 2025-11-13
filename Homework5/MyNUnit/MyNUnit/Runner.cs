// <copyright file="Runner.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

public abstract class Runner
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    public void TestRunner(string path)
    {
        string[] allDlls = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        string[] allExes = Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories);
        var allAssemblies = allDlls.Concat(allExes);
        var allTests = [];
        foreach (var assemblyPath in allAssemblies)
        {
            var searcher = new Searcher();
            var testClasses = searcher.TestSearcher(assemblyPath);
            allTests.Add(testClasses);
        }

        var executor = new Executor();
        var result = executor.TestExecutor(allTests);
    }
}

