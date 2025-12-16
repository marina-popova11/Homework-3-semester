// <copyright file="Analyzer.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test2;

using System.Reflection;
using System.Text;

/// <summary>
/// Class for type analyzer.
/// </summary>
public class Analyzer
{
    /// <summary>
    /// Creates and describes a class with all its methods and fields.
    /// </summary>
    /// <param name="someClass">The type of some class, that will be written to the file.</param>
    /// <exception cref="ArgumentNullException">if class is null.</exception>
    public static void PrintStructure(Type someClass)
    {
        if (someClass == null)
        {
            throw new ArgumentNullException(nameof(someClass));
        }

        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(someClass.Namespace))
        {
            sb.AppendLine($"namespace {someClass.Namespace};\n");
        }

        sb.AppendLine(GetType(someClass) + "{");
        foreach (var f in someClass.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
        {
            sb.AppendLine($"{GetField(f)};");
        }

        foreach (var m in someClass.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
        {
            sb.AppendLine($"{GetMethod(m, false)}");
        }

        sb.AppendLine("}");

        File.WriteAllText($"{someClass.Name}.cs", sb.ToString());
    }

    private static string GetType(Type type) =>
        $"{(type.IsPublic || type.IsNestedPublic ? "public " :
            type.IsNestedPrivate ? "private " :
            type.IsNestedFamily ? "protected " : string.Empty)}" +
        $"{(type.IsSealed && type.IsAbstract ? "static " :
            type.IsAbstract ? "abstract " :
            type.IsSealed ? "sealed " : string.Empty)}" +
        $"{(type.IsInterface ? "interface " : type.IsEnum ? "enum " : type.IsValueType ? "struct " : "class ")}" +
        $"{type.Name}{(type.IsGenericType ? $"<{string.Join(", ", type.GetGenericArguments().Select(x => x.Name))}>" : string.Empty)}\n";

    private static string GetField(FieldInfo field) =>
        $"{(field.IsPublic ? "public " : field.IsPrivate ? "private " : field.IsFamily ? "protected " : string.Empty)}" +
        $"{(field.IsStatic ? "static " : string.Empty)}{(field.IsInitOnly ? "readonly " : string.Empty)}" +
        $"{SimplifyTypeName(field.FieldType)} {field.Name}" +
        $"{(field.IsLiteral ? $" = {FormatValue(field.GetValue(null)!)}" : string.Empty)}";

    private static string GetMethod(MethodInfo method, bool isHasBody) =>
        $"{(method.IsPublic ? "public " : method.IsPrivate ? "private " : method.IsFamily ? "protected " : string.Empty)}" +
        $"{(method.IsStatic ? "static " : string.Empty)}{SimplifyTypeName(method.ReturnType)} {method.Name}" +
        $"{(method.IsGenericMethod ? $"<{string.Join(", ", method.GetGenericArguments().Select(x => x.Name))}>" : string.Empty)}" +
        $"({string.Join(", ", method.GetParameters().Select(p => $"{SimplifyTypeName(p.ParameterType)} {p.Name}"))})" +
        (isHasBody ? $"\n    {{\n        {(method.ReturnType != typeof(void) ? $"return {DefaultValue(method.ReturnType)};" : string.Empty)}\n    }}" : ";");

    private static string SimplifyTypeName(Type type) =>
        type.IsGenericType ? $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(SimplifyTypeName))}>" : type.Name;

    private static string DefaultValue(Type type) =>
        type.IsValueType ? (type == typeof(bool) ? "false" : type == typeof(char) ? "'\\0'" : "default") : "null";

    private static string FormatValue(object ob) =>
        ob == null ? "null" : ob is string ? $"\"{ob}\"" : ob is char ? $"'{ob}'" : ob.ToString()!;
}