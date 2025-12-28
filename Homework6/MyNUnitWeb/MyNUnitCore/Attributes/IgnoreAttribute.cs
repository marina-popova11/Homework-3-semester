// <copyright file="IgnoreAttribute.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// Identifies tests that will be ignored when running.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class IgnoreAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IgnoreAttribute"/> class.
    /// </summary>
    /// <param name="reason">The reason for ignoring.</param>
    public IgnoreAttribute(string reason = "")
    {
        this.Reason = reason;
    }

    /// <summary>
    /// Gets the reason.
    /// </summary>
    public string Reason { get; }
}