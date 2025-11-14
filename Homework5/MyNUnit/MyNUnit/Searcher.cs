// <copyright file="Searcher.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace MyNUnit;

using System.Reflection;
using MyNUnit.Attributes;

/// <summary>
/// .
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
        string[] allDlls = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
        string[] allExes = Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories);
        var allAssemblies = allDlls.Concat(allExes);
        foreach (var assemblyPath in allAssemblies)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                var classInfo = new TestClassInfo { ClassType = type };
                MethodInfo[] methods = type.GetMethods();
                foreach (var method in methods)
                {
                    TestAttribute? testAt = method.GetCustomAttribute<TestAttribute>();
                    if (testAt != null)
                    {
                        classInfo.TestMethods!.Add(method);
                    }

                    Before? before = method.GetCustomAttribute<Before>();
                    if (before != null)
                    {
                        classInfo.BeforeMethods!.Add(method);
                    }

                    After? after = method.GetCustomAttribute<After>();
                    if (before != null)
                    {
                        classInfo.AfterMethods!.Add(method);
                    }

                    BeforeClass? beforeClass = method.GetCustomAttribute<BeforeClass>();
                    if (beforeClass != null)
                    {
                        if (!method.IsStatic)
                        {
                            Console.WriteLine("BeforeClass Method should be static");
                            classInfo.BeforeClassMethods!.Add(method);
                        }
                    }

                    AfterClass? afterClass = method.GetCustomAttribute<AfterClass>();
                    if (afterClass != null)
                    {
                        if (!method.IsStatic)
                        {
                            Console.WriteLine("AfterClass Method should be static");
                            classInfo.AfterClassMethods!.Add(method);
                        }
                    }

                    if (classInfo.TestMethods!.Count > 0)
                    {
                        testClasses.Add(classInfo);
                    }
                }
            }
        }

        return testClasses;
    }
}
