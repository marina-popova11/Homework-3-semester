// <copyright file="TestAttribute.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// The method as callable from the test runner.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the type of expected expression.
    /// </summary>
    public Type Expected { get; set; }

}