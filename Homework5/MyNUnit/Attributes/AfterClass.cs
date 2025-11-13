// <copyright file="AfterClass.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

/// <summary>
/// Identifies a static method to be called once after all tests have run.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class AfterClass : Attribute
{
}