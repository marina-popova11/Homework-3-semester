// <copyright file="Reporter.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

using MyNUnit.Attributes;

/// <summary>
/// .
/// </summary>
public class Reporter
{
    /// <summary>
    /// States that the test can enter after execution begins.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// If Test was passed.
        /// </summary>
        Passed,

        /// <summary>
        /// If test was failed.
        /// </summary>
        Failed,
    }

    /// <summary>
    /// Gets the list of results.
    /// </summary>
    public List<TestResult>? Results { get; } = new();

    /// <summary>
    /// Gets the list of errors.
    /// </summary>
    public List<string> Errors { get; } = new();

    /// <summary>
    /// Gets or sets the count of failed tests.
    /// </summary>
    /// <returns>The number of failed tests.</returns>
    public int FailedCount() => this.Results!.Count(x => x.Status == Status.Failed);

    /// <summary>
    /// Creates the report.
    /// </summary>
    /// <param name="results">The results of all tests.</param>
    /// <returns>Report about all results and their errors, if they are.</returns>
    public Reporter CreateReport(List<TestResult> results)
    {
        foreach (var result in results)
        {
            this.AddResult(result);
        }

        return this;
    }

    /// <summary>
    /// Adds result of one test to the overall result.
    /// </summary>
    /// <param name="result">Result of one test.</param>
    public void AddResult(TestResult result)
    {
        this.Results!.Add(result);
    }

    /// <summary>
    /// Adds error of one test to the overall errors.
    /// </summary>
    /// <param name="error">Error of one test.</param>
    public void AddErrors(string error)
    {
        this.Errors!.Add(error);
    }

    /// <summary>
    /// Class for test`s results.
    /// </summary>
    public record TestResult(
        string? Name,
        string? ClassName,
        Status Status,
        string? Error = null);

    // {
    //     /// <summary>
    //     /// Gets or sets the name of test.
    //     /// </summary>
    //     public string? Name { get; set; }
    //     /// <summary>
    //     /// Gets or sets the error of test.
    //     /// </summary>
    //     public string? Error { get; set; }
    //     /// <summary>
    //     /// Gets or sets the status of the test.
    //     /// </summary>
    //     public Status? Status { get; set; }
    //     /// <summary>
    //     /// Gets or sets the class name.
    //     /// </summary>
    //     public string? ClassName { get; set; }
    // }
}