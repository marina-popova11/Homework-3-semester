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
    public Executor() => this.classRunner = new TestClassRunner();

    /// <summary>
    /// Runs a TestClassRunner for each class.
    /// </summary>
    /// <param name="testClass">Test class for execute.</param>
    /// <returns>Execution results.</returns>
    public List<Reporter.TestResult> TestExecute(List<TestClassInfo> testClass)
    {
        var results = new List<Reporter.TestResult>();
        Parallel.ForEach(testClass, classInfo =>
        {
            var classResults = this.classRunner.RunTests(classInfo);
            lock (results)
            {
                results.AddRange(classResults);
            }
        });

        return results;
    }

    /// <summary>
    /// Manages the lifecycle of tests within a single class.
    /// </summary>
    private class TestClassRunner
    {
        public List<Reporter.TestResult> RunTests(TestClassInfo classInfo)
        {
            if (classInfo.ClassType == null)
            {
                throw new InvalidOperationException(nameof(classInfo));
            }

            var result = new List<Reporter.TestResult>();
            this.RunBeforeClassMethods(classInfo);
            if (classInfo.TestMethods == null)
            {
                return result;
            }

            var testResults = new List<Reporter.TestResult>();
            Parallel.ForEach(classInfo.TestMethods, method =>
            {
                var testResult = this.RunSingleTest(classInfo, method);
                lock (testResults)
                {
                    testResults.Add(testResult);
                }
            });

            result.AddRange(testResults);
            try
            {
                this.RunAfterClassMethods(classInfo);
            }
            catch
            {
            }

            return result;
        }

        public Reporter.TestResult RunSingleTest(TestClassInfo classInfo, MethodInfo method)
        {
            var status = Reporter.Status.Passed;
            string? error = null;
            object? testInstance = null;

            if (classInfo.ClassType == null)
            {
                throw new InvalidOperationException(nameof(classInfo));
            }

            try
            {
                testInstance = Activator.CreateInstance(classInfo.ClassType) ?? throw new InvalidOperationException(nameof(testInstance));
                this.RunBeforeMethods(classInfo, testInstance);
                method.Invoke(testInstance, null);
            }
            catch (TargetInvocationException ex)
            {
                status = Reporter.Status.Failed;
                error = ex.InnerException?.Message ?? ex.Message;
            }
            catch (Exception ex)
            {
                status = Reporter.Status.Failed;
                error = ex.Message;
            }
            finally
            {
                if (testInstance != null)
                {
                    try
                    {
                        this.RunAfterMethods(classInfo, testInstance);
                    }
                    catch (Exception afterEx)
                    {
                        error = error == null
                            ? $"After method failed: {afterEx.Message}"
                            : $"{error}; After failed: {afterEx.Message}";
                        status = Reporter.Status.Failed;
                    }
                }
            }

            var testInfo = new Reporter.TestResult(
                Name: method.Name,
                ClassName: classInfo.ClassType!.Name,
                Status: status,
                Error: error);

            return testInfo;
        }

        public void RunBeforeClassMethods(TestClassInfo classInfo)
        {
            if (classInfo.BeforeClassMethods == null)
            {
                return;
            }

            foreach (var method in classInfo.BeforeClassMethods)
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
            if (classInfo.AfterClassMethods == null)
            {
                return;
            }

            foreach (var method in classInfo.AfterClassMethods)
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
            this.InvokeMethods(classInfo.AfterMethods, testInstance);
        }

        public void RunBeforeMethods(TestClassInfo classInfo, object testInstance)
        {
            this.InvokeMethods(classInfo.BeforeMethods, testInstance);
        }

        private void InvokeMethods(IEnumerable<MethodInfo>? methods, object? instance)
        {
            if (methods == null)
            {
                return;
            }

            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(instance, null);
                }
                catch
                {
                }
            }
        }
    }
}