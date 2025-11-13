// <copyright file="After.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Identifies a static method to be called once after each test is run.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class After : Attribute
{
}