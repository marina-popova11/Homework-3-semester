// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using System.Net;
using FTPClient;

if (args.Length != 2)
{
    Console.WriteLine("Please enter the IP address and port.");
    return;
}

var ip = args[0];
if (IPAddress.TryParse(ip, out _))
{
    Console.WriteLine("Incorrect IP address. Enter the right IP address.");
    return;
}