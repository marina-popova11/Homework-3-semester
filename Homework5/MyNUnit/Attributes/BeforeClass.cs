// <copyright file="BeforeClass.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Identifies a static method to be called once before all tests have run.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class BeforeClass : Attribute
{
}