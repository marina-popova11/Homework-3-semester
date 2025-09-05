// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using ParMatrixMult;

string file = "file1.txt";
var matr = new Matrix();
matr.GetMatrixFromFile(file);
System.Console.WriteLine(matr.GetRows());