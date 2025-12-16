// <copyright file="TypeComparer.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test2;

using System.Reflection;
using System.Text;

/// <summary>
/// Class for comparing two classes.
/// </summary>
public class TypeComparer
{
    /// <summary>
    /// Compares two classes.
    /// </summary>
    /// <param name="a">first class.</param>
    /// <param name="b">second class.</param>
    /// <returns>results.</returns>
    public static DiffResult DiffClasses(Type a, Type b)
    {
        var result = new DiffResult();
        var fieldsA = a.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).ToDictionary(f => f.Name);

        var fieldsB = a.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).ToDictionary(f => f.Name);

        foreach (var f in fieldsA.Values)
        {
            if (!fieldsB.TryGetValue(f.Name, out var fb))
            {
                result.OnlyA.Add($"{f.FieldType.Name} {f.Name}");
            }
            else if (!AreEqual(f, fb))
            {
                result.Both.Add($"{f.Name}: {f.FieldType.Name} != {fb.FieldType.Name}");
            }

            fieldsB.Remove(f.Name);
        }

        foreach (var f in fieldsB.Values)
        {
            result.OnlyB.Add($"{f.FieldType.Name} {f.Name}");
        }

        return result;
    }

    private static bool AreEqual(FieldInfo a, FieldInfo b)
    {
        return a.FieldType == b.FieldType &&
            a.IsPublic == b.IsPublic &&
            a.IsStatic == b.IsStatic;
    }

    /// <summary>
    /// Class for having results.
    /// </summary>
    public class DiffResult
    {
        /// <summary>
        /// List for first class.
        /// </summary>
        public List<string> OnlyA = new();

        /// <summary>
        /// List for second class.
        /// </summary>
        public List<string> OnlyB = new();

        /// <summary>
        /// List for both classes.
        /// </summary>
        public List<string> Both = new();

        /// <summary>
        /// Print methods and fields of classes.
        /// </summary>
        public void Print()
        {
            Console.WriteLine("Only in A:");
            this.OnlyA.ForEach(x => Console.WriteLine($"{x}"));
            Console.WriteLine("\nOnly in B:");
            this.OnlyB.ForEach(x => Console.WriteLine($"{x}"));
            Console.WriteLine("\nBoth with different");
            this.Both.ForEach(x => Console.WriteLine($"{x}"));
        }
    }
}