// <copyright file="AssemblyReport.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnitWeb.Models;

using System.ComponentModel.DataAnnotations;
using static MyNUnit.Reporter;

/// <summary>
/// Represents a test run for a single assembly (.dll).
/// </summary>
public class AssemblyReport
{
    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the assembly (DLL file).
    /// </summary>
    [Required]
    public string AssemblyName { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

    /// <summary>
    /// Gets or sets the UTC timestamp when the test run started.
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of tests` results.
    /// </summary>
    public List<TestReport> Tests { get; set; } = new();

    /// <summary>
    /// Gets the number of tests that passed in this assembly run.
    /// </summary>
    public int Passed => this.Tests.Count(t => t.Status == "Passed");

    /// <summary>
    /// Gets the number of tests that failed in this assembly run.
    /// </summary>
    public int Failed => this.Tests.Count(t => t.Status == "Failed");

    /// <summary>
    /// Gets the number of tests that ignored in this assembly run.
    /// </summary>
    public int Ignored => this.Tests.Count(t => t.Status == "Ignored");
}
