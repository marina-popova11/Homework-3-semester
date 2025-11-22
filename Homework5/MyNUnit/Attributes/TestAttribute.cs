// <copyright file="TestAttribute.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Attributes;

/// <summary>
/// The method as callable from the test runner.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the type of expected expression.
    /// </summary>
    public Type? Expected { get; set; }

    /// <summary>
    /// Gets or sets the reason to ignore test.
    /// </summary>
    public string? Ignored { get; set; }
}