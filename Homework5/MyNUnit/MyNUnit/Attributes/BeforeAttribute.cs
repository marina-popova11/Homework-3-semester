// <copyright file="BeforeAttribute.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// Identifies a static method to be called once before each test is run.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeAttribute : Attribute
{
}