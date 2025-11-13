// <copyright file="Executor.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

/// <summary>
/// .
/// </summary>
public class Executor
{
    private readonly TestClassRunner classRunner;

    /// <summary>
    /// Initializes a new instance of the <see cref="Executor"/> class.
    /// </summary>
    public Executor()
    {
        this.classRunner = new TestClassRunner();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="testClass"></param>
    /// <returns>Execution results.</returns>
    public Reporter TestExecutor(List<TestClassInfo> testClass)
    {
        var result = new Reporter();
        Parallel.ForEach(testClass, classInfo =>
        {
            try
            {
                var classResults = this.classRunner.RunTests(classInfo);
                lock (result)
                {
                    result.AddResult(classResults);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        });

        return result;
    }

    private class TestClassRunner
    {
        public Task RunTests(TestClassInfo classInfo)
        {
            
        }

    }
}