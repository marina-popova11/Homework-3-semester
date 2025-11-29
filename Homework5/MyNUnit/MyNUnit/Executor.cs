// <copyright file="Executor.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

using System.Reflection;
using MyNUnit.Attributes;

/// <summary>
/// Class for execute all found tests.
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
    /// Runs a TestClassRunner for each class.
    /// </summary>
    /// <param name="testClass">Test class for execute.</param>
    /// <returns>Execution results.</returns>
    public List<Reporter.TestResult> TestExecutor(List<TestClassInfo> testClass)
    {
        var results = new List<Reporter.TestResult>();
        Parallel.ForEach(testClass, classInfo =>
        {
            try
            {
                var classResults = this.classRunner.RunTests(classInfo);
                lock (results)
                {
                    results.AddRange(classResults);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        });

        return results;
    }

    /// <summary>
    /// manages the lifecycle of tests within a single class.
    /// </summary>
    private class TestClassRunner
    {
        public List<Reporter.TestResult> RunTests(TestClassInfo classInfo)
        {
            var result = new List<Reporter.TestResult>();
            this.RunBeforeClassMethods(classInfo);
            foreach (var method in classInfo.TestMethods!)
            {
                var testResult = this.RunSingleTest(classInfo, method);
                result.Add(testResult);
            }

            this.RunAfterClassMethods(classInfo);
            return result;
        }

        public Reporter.TestResult RunSingleTest(TestClassInfo classInfo, MethodInfo method)
        {
            var testInfo = new Reporter.TestResult
            {
                Name = method.Name,
                ClassName = classInfo.ClassType!.Name,
            };

            object testInstance = null!;
            try
            {
                testInstance = Activator.CreateInstance(classInfo.ClassType)!;
                this.RunBeforeMethods(classInfo, testInstance);
                method.Invoke(testInstance, null);
                testInfo.Status = Reporter.Status.Passed;
            }
            catch (Exception ex)
            {
                testInfo.Status = Reporter.Status.Failed;
                testInfo.Error = ex.Message;
            }
            finally
            {
                this.RunAfterMethods(classInfo, testInstance);
            }

            return testInfo;
        }

        public void RunBeforeClassMethods(TestClassInfo classInfo)
        {
            foreach (var method in classInfo.BeforeClassMethods!)
            {
                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"BeforeClass method {method.Name} failed: {ex.Message}");
                }
            }
        }

        public void RunAfterClassMethods(TestClassInfo classInfo)
        {
            foreach (var method in classInfo.AfterClassMethods!)
            {
                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"AfterClass method {method.Name} failed: {ex.Message}");
                }
            }
        }

        public void RunAfterMethods(TestClassInfo classInfo, object testInstance)
        {
            foreach (var method in classInfo.AfterMethods!)
            {
                method.Invoke(testInstance, null);
            }
        }

        public void RunBeforeMethods(TestClassInfo classInfo, object testInstance)
        {
            foreach (var method in classInfo.BeforeMethods!)
            {
                method.Invoke(testInstance, null);
            }
        }
    }
}