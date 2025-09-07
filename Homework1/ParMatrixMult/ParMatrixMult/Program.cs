// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using ParMatrixMult;

string file1 = "file1.txt";
string file2 = "file2.txt";
var matr1 = new Matrix();
matr1.GetMatrixFromFile(file1);
var matr2 = new Matrix();
matr2.GetMatrixFromFile(file2);
var result = new Multiplication();

// if (!result.CompareMatrixDim(matr1, matr2)) ;
Matrix resMatr = result.SequentialMult(matr1, matr2);
System.Console.WriteLine(resMatr.GetColumns());
Matrix resMatr2 = result.ParallelMult(matr1, matr2, 4);
System.Console.WriteLine(resMatr2.GetRows());