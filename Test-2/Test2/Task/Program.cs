// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using Test2;

Analyzer.PrintStructure(typeof(Classes.FirstClass));
Console.WriteLine("File created: " + File.Exists("FirstClass.cs"));

var diff = TypeComparer.DiffClasses(typeof(Classes.FirstClass), typeof(Classes.SecondClass));
diff.Print();