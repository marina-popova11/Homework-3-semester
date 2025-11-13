// <copyright file="TestInfo.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

using System.Reflection;
using MyNUnit.Attributes;

public class TestInfo
{
    /// <summary>
    /// Gets or sets.
    /// </summary>
    public MethodInfo Method { get; set; }

    /// <summary>
    /// Gets or sets.
    /// </summary>
    public TestAttribute Attribute { get; set; }
}