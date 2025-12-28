// <copyright file="HomeController.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnitWeb.Controllers;

using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNUnit;
using MyNUnitWeb.Data;
using MyNUnitWeb.Models;

/// <summary>
/// Handles assembly uploads, test execution, and retrieval of test run history.
/// </summary>
public class HomeController : Controller
{
    private readonly IWebHostEnvironment environment;
    private readonly History history;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="environment">Web host environment for file system access.</param>
    /// <param name="context">The history of running tests.</param>
    public HomeController(IWebHostEnvironment environment, History context)
    {
        this.environment = environment;
        this.history = context;
    }

    /// <summary>
    /// .
    /// </summary>
    /// <returns>d.</returns>
    public IActionResult Index()
    {
        return this.View();
    }

    /// <summary>
    /// Handles the upload of DLL files containing tests.
    /// </summary>
    /// <param name="files">Array of files uploaded by the client.</param>
    /// <returns>OK result with upload directory path on success,
    /// BadRequest if no files were provided.</returns>
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile[] files)
    {
        if (files == null || files.Length == 0)
        {
            return this.BadRequest("No files");
        }

        var uploadDir = Path.Combine(this.environment.ContentRootPath, "Uploads", Guid.NewGuid().ToString());
        Directory.CreateDirectory(uploadDir);

        foreach (var file in files)
        {
            if (file.FileName.EndsWith(".dll"))
            {
                var path = Path.Combine(uploadDir, file.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
            }
        }

        return this.Ok(new { uploadDir });
    }

    /// <summary>
    /// Executes tests from uploaded assemblies and stores results in the database.
    /// </summary>
    /// <param name="body">JSON body containing the upload directory path.</param>
    /// <returns>OK result with test execution report on success,
    /// BadRequest if the upload directory is invalid or doesn't exist.</returns>
    [HttpPost]
    public async Task<IActionResult> Run([FromBody] JsonElement body)
    {
        var uploadDir = body.GetProperty("uploadDir").GetString();
        if (string.IsNullOrEmpty(uploadDir) || !Directory.Exists(uploadDir))
        {
            return this.BadRequest("Invalid upload directory");
        }

        var runner = new Runner();

        var reporter = runner.TestRunner(uploadDir);
        var results = reporter.Results;
        if (results == null || results.Count == 0)
        {
            return this.Ok(new { success = true, message = "No tests found.", reports = new object[0] });
        }

        var groups = results!.GroupBy(r => r.AssemblyName);
        var reports = new List<AssemblyReport>();

        foreach (var group in groups)
        {
            var assemblyReport = new AssemblyReport
            {
                AssemblyName = group.Key,
                StartedAt = DateTime.UtcNow,
            };

            foreach (var result in group)
            {
                var testReport = new TestReport
                {
                    MethodName = result.Name!,
                    ClassName = result.ClassName!,
                    Status = result.Status switch
                    {
                        Reporter.Status.Passed => "Passed",
                        Reporter.Status.Failed => "Failed",
                        Reporter.Status.Ignored => "Ignored",
                        _ => "Unknown",
                    },
                    DurationMs = result.DurationMs,
                    IgnoreReason = result.Status == Reporter.Status.Ignored ? result.IgnoreReason : null,
                    ErrorMessage = result.Status == Reporter.Status.Failed ? result.Error : null,
                };

                assemblyReport.Tests.Add(testReport);
            }

            reports.Add(assemblyReport);
        }

        await this.history.Assemblies.AddRangeAsync(reports);
        await this.history.SaveChangesAsync();

        return this.Ok(new
        {
            success = true,
            runId = reports.First().Id,
            reports = reports.Select(r => new
            {
                r.AssemblyName,
                Passed = r.Tests.Count(t => t.Status == "Passed"),
                Failed = r.Tests.Count(t => t.Status == "Failed"),
                Ignored = r.Tests.Count(t => t.Status == "Ignored"),
                Tests = r.Tests.Select(t => new
                {
                    t.MethodName,
                    t.ClassName,
                    t.Status,
                    t.DurationMs,
                    t.IgnoreReason,
                    t.ErrorMessage,
                }),
            }),
        });
    }

    /// <summary>
    /// Retrieves the history of all test runs from the database.
    /// </summary>
    /// <returns>OK result with list of test run summaries, ordered by most recent first.</returns>
    [HttpGet]
    public async Task<IActionResult> History()
    {
        var history = await this.history.Assemblies
            .Select(a => new
            {
                a.AssemblyName,
                a.Id,
                Passed = a.Passed,
                Failed = a.Failed,
                Ignored = a.Ignored,
            })
            .OrderByDescending(a => a.Id)
            .ToListAsync();

        return this.Ok(history);
    }
}
