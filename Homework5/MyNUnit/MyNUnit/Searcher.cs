// <copyright file="Searcher.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

using System.Reflection;
using MyNUnit.Attributes;

/// <summary>
/// Class for search tests with different attributes.
/// </summary>
public class Searcher
{
    /// <summary>
    /// Search all assembly.
    /// </summary>
    /// <param name="path">The path to search assemblies.</param>
    /// <returns>the list of testClasses.</returns>
    public List<TestClassInfo> TestSearcher(string path)
    {
        var testClasses = new List<TestClassInfo>();
        try
        {
            Console.WriteLine($"Loading assembly: {Path.GetFileName(path)}");
            Assembly assembly = Assembly.LoadFrom(path);
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                var classInfo = new TestClassInfo { ClassType = type };
                MethodInfo[] methods = type.GetMethods();
                bool hasTest = false;
                foreach (var method in methods)
                {
                    TestAttribute? testAt = method.GetCustomAttribute<TestAttribute>();
                    if (testAt != null)
                    {
                        classInfo.TestMethods!.Add(method);
                        Console.WriteLine($"Found test method: {method.Name}");
                        hasTest = true;
                    }
                    else
                    {
                        Console.WriteLine($"No Test attribute on {method.Name}");
                        var allAttributes = method.GetCustomAttributes();
                        foreach (var attr in allAttributes)
                        {
                            Console.WriteLine($"    - Attribute: {attr.GetType().FullName}");
                        }
                    }

                    BeforeAttribute? before = method.GetCustomAttribute<BeforeAttribute>();
                    if (before != null)
                    {
                        classInfo.BeforeMethods!.Add(method);
                        Console.WriteLine($"Found Before method: {method.Name}");
                    }

                    AfterAttribute? after = method.GetCustomAttribute<AfterAttribute>();
                    if (after != null)
                    {
                        classInfo.AfterMethods!.Add(method);
                        Console.WriteLine($"Found After method: {method.Name}");
                    }

                    BeforeClassAttribute? beforeClass = method.GetCustomAttribute<BeforeClassAttribute>();
                    if (beforeClass != null)
                    {
                        if (!method.IsStatic)
                        {
                            Console.WriteLine("BeforeClass Method should be static");
                        }

                        classInfo.BeforeClassMethods!.Add(method);
                        Console.WriteLine($"Found BeforeClass method: {method.Name}");
                    }

                    AfterClassAttribute? afterClass = method.GetCustomAttribute<AfterClassAttribute>();
                    if (afterClass != null)
                    {
                        if (!method.IsStatic)
                        {
                            Console.WriteLine("AfterClass Method should be static");
                        }

                        classInfo.AfterClassMethods!.Add(method);
                        Console.WriteLine($"Found AfterClass method: {method.Name}");
                    }
                }

                if (hasTest)
                {
                    testClasses.Add(classInfo);
                }
                else
                {
                    Console.WriteLine($"Skipping class {type.Name} - no test");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {Path.GetFileName(path)}: {ex.Message}");
        }

        return testClasses;
    }
}
