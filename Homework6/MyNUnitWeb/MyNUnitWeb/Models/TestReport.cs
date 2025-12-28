// <copyright file="TestReport.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnitWeb.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the result of a single test method.
/// </summary>
public class TestReport
{
    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the test method.
    /// </summary>
    [Required]
    public string MethodName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the test class.
    /// </summary>
    [Required]
    public string ClassName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the test status: Passed, Failed, Ignored.
    /// </summary>
    [Required]
    public string Status { get; set; } = "Unknown"; // Passed / Failed / Ignored

    /// <summary>
    /// Gets or sets the execution time in milliseconds.
    /// </summary>
    public double DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the ignore reason (if status == "Ignored").
    /// </summary>
    public string? IgnoreReason { get; set; }

    /// <summary>
    /// Gets or sets the error message (if status == "Failed").
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets foreign key.
    /// </summary>
    public int AssemblyReportId { get; set; }

    /// <summary>
    /// Gets or sets a navigation property to the parent assembly report.
    /// </summary>
    public AssemblyReport AssemblyReport { get; set; } = null!;
}
