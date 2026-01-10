// <copyright file="TestRunner.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit.Tests;

using MyNUnit;

[TestFixture]
public class TestRunner
{
    private Searcher searcher;
    private Executor executor;
    private string assemblyPath;

    [SetUp]
    public void SetUp()
    {
        this.searcher = new Searcher();
        this.executor = new Executor();
        this.assemblyPath = this.GetFakeTestsAssemblyPath();
    }

    [Test]
    public void Test_ExecutorRunsPassingTests()
    {
        var classes = this.searcher.TestSearch(this.assemblyPath!);
        var results = this.executor.TestExecute(classes);
        var simpleResults = results.Where(r => r.ClassName == "SimpleTests").ToList();
        Assert.That(simpleResults.Count(), Is.EqualTo(3));
        Assert.That(simpleResults.All(r => r.Status == Reporter.Status.Passed));
    }

    [Test]
    public void Test_ExecutorRunsFailingTests()
    {
        var classes = this.searcher.TestSearch(this.assemblyPath!);
        var results = this.executor.TestExecute(classes);
        var failingResults = results.Where(r => r.ClassName == "FailedTests").ToList();
        Assert.That(failingResults.Count(), Is.EqualTo(2));
        Assert.That(failingResults.All(r => r.Status == Reporter.Status.Failed));
    }

    [Test]
    public void Test_ReporterCollectResults()
    {
        var reporter = new Reporter();
        var classes = this.searcher.TestSearch(this.assemblyPath!);
        var results = this.executor.TestExecute(classes);
        var report = reporter.CreateReport(results);

        Assert.That(report.Results?.Count, Is.EqualTo(5));
        Assert.That(report.FailedCount(), Is.EqualTo(2));
    }

    [Test]
    public void Test_Runner()
    {
        var runner = new Runner();
        var report = runner.TestRun(Path.GetDirectoryName(this.assemblyPath)!);

        Assert.That(report.Results, Has.Some.Property("ClassName").EqualTo("SimpleTests"));
        Assert.That(report.Results, Has.Some.Property("ClassName").EqualTo("FailedTests"));
        var passing = report.Results.Where(r => r.ClassName == "SimpleTests");
        Assert.That(passing.All(r => r.Status == Reporter.Status.Passed));

        var failing = report.Results.Where(r => r.ClassName == "FailedTests");
        Assert.That(failing.All(r => r.Status == Reporter.Status.Failed));
    }

    private string GetFakeTestsAssemblyPath()
    {
        try
        {
            var type = Type.GetType("MyNUnit.FakeTests.SimpleTests, MyNUnit.FakeTests");
            if (type == null)
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "MyNUnit.FakeTests");

                if (assembly != null)
                {
                    return assembly.Location;
                }

                var dllPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "MyNUnit.FakeTests.dll");

                if (File.Exists(dllPath))
                {
                    return dllPath;
                }

                throw new FileNotFoundException($"MyNUnit.FakeTests assembly not found. Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
            }

            return type.Assembly.Location;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetFakeTestsAssemblyPath: {ex.Message}");
            throw;
        }
    }
}
