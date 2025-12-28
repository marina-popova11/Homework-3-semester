// <copyright file="History.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnitWeb.Data;

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MyNUnitWeb.Models;

/// <summary>
/// Database context for storing test run history.
/// </summary>
public class History : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="History"/> class.
    /// </summary>
    /// <param name="options">DbContext options.</param>
    public History(DbContextOptions<History> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of test run reports.
    /// </summary>
    public DbSet<AssemblyReport> Assemblies { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of individual test results.
    /// </summary>
    public DbSet<TestReport> TestReports { get; set; } = null!;

    /// <summary>
    /// Configures the model.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssemblyReport>()
            .HasMany(a => a.Tests)
            .WithOne(t => t.AssemblyReport)
            .HasForeignKey(t => t.AssemblyReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}