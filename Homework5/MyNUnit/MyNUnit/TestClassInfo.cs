// <copyright file="TestClassInfo.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

using System.Reflection;
using Attributes;

/// <summary>
/// Class for storing information about a test class.
/// </summary>
public class TestClassInfo
{
    /// <summary>
    /// Gets or sets a test class type that contains methods for testing.
    /// </summary>
    public Type? ClassType { get; set; }

    /// <summary>
    /// Gets or sets list of methods marked as test methods (attribute [Test]).
    /// </summary>
    public List<MethodInfo> TestMethods { get; set; } = new();

    /// <summary>
    /// Gets or sets list of methods that are executed after each test method (attribute [After] or [TearDown]).
    /// </summary>
    public List<MethodInfo> AfterMethods { get; set; } = new();

    /// <summary>
    /// Gets or sets list of methods that are executed before each test method (attribute [Before] or [SetUp]).
    /// </summary>
    public List<MethodInfo> BeforeMethods { get; set; } = new();

    /// <summary>
    /// Gets or sets list of methods that are executed AFTER all tests in the class (attribute [AfterClass] or [OneTimeTearDown]).
    /// </summary>
    public List<MethodInfo> AfterClassMethods { get; set; } = new();

    /// <summary>
    /// Gets or sets list of methods that are executed BEFORE all tests in the class (attribute [BeforeClass] or [OneTimeSetUp]).
    /// </summary>
    public List<MethodInfo> BeforeClassMethods { get; set; } = new();
}