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
    public List<TestClassInfo> TestSearch(string path)
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
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                bool hasTest = false;
                foreach (var method in methods)
                {
                    TestAttribute? testAt = method.GetCustomAttribute<TestAttribute>();
                    if (testAt != null)
                    {
                        if (this.ValidateTestMethod(method, "Test"))
                        {
                            classInfo.TestMethods!.Add(method);
                            Console.WriteLine($"Found test method: {method.Name}");
                            hasTest = true;
                        }

                        continue;
                    }
                    else
                    {
                        Console.WriteLine($"No Test attribute on {method.Name}");
                        var allAttributes = method.GetCustomAttributes();
                        foreach (var attr in allAttributes)
                        {
                            Console.WriteLine($"Attribute: {attr.GetType().FullName}");
                        }
                    }

                    BeforeAttribute? before = method.GetCustomAttribute<BeforeAttribute>();
                    if (before != null)
                    {
                        if (this.ValidateInstanceMethod(method, "Before"))
                        {
                            classInfo.BeforeMethods!.Add(method);
                            Console.WriteLine($"Found Before method: {method.Name}");
                        }

                        continue;
                    }

                    AfterAttribute? after = method.GetCustomAttribute<AfterAttribute>();
                    if (after != null)
                    {
                        if (this.ValidateInstanceMethod(method, "After"))
                        {
                            classInfo.AfterMethods!.Add(method);
                            Console.WriteLine($"Found After method: {method.Name}");
                        }

                        continue;
                    }

                    BeforeClassAttribute? beforeClass = method.GetCustomAttribute<BeforeClassAttribute>();
                    if (beforeClass != null)
                    {
                        if (this.ValidateStaticMethod(method, "BeforeClass"))
                        {
                            classInfo.BeforeClassMethods!.Add(method);
                            Console.WriteLine($"Found BeforeClass method: {method.Name}");
                        }

                        continue;
                    }

                    AfterClassAttribute? afterClass = method.GetCustomAttribute<AfterClassAttribute>();
                    if (afterClass != null)
                    {
                        if (this.ValidateStaticMethod(method, "AfterClass"))
                        {
                            classInfo.AfterClassMethods!.Add(method);
                            Console.WriteLine($"Found AfterClass method: {method.Name}");
                        }

                        continue;
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

    private bool ValidateTestMethod(MethodInfo method, string kind)
    {
        if (method.IsStatic)
        {
            Console.WriteLine($"{kind} method '{method.Name}' must not be static.");
            return false;
        }

        if (method.ReturnType != typeof(void))
        {
            Console.WriteLine($"{kind} method '{method.Name}' must return void.");
            return false;
        }

        if (method.GetParameters().Length > 0)
        {
            Console.WriteLine($"{kind} method '{method.Name}' must not have parameters.");
            return false;
        }

        return true;
    }

    private bool ValidateInstanceMethod(MethodInfo method, string kind)
    {
        if (method.IsStatic)
        {
            Console.WriteLine($"{kind} method '{method.Name}' must not be static.");
            return false;
        }

        if (method.ReturnType != typeof(void))
        {
            Console.WriteLine($"{kind} method '{method.Name}' must return void.");
            return false;
        }

        if (method.GetParameters().Length > 0)
        {
            Console.WriteLine($"{kind} method '{method.Name}' must not have parameters.");
            return false;
        }

        return true;
    }

    private bool ValidateStaticMethod(MethodInfo method, string kind)
    {
        if (!method.IsStatic)
        {
            Console.WriteLine($"{kind} method '{method.Name}' must be static.");
            return false;
        }

        if (method.ReturnType != typeof(void))
        {
            Console.WriteLine($"{kind} method '{method.Name}' must return void.");
            return false;
        }

        if (method.GetParameters().Length > 0)
        {
            Console.WriteLine($"{kind} method '{method.Name}' must not have parameters.");
            return false;
        }

        return true;
    }
}
