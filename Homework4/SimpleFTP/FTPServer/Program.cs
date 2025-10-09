// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using System.Net;
using FTPServer;

if (args.Length != 2)
{
    Console.WriteLine("Please enter the IP address and port.");
    return;
}

if (!IPAddress.TryParse(args[0], out var ip))
{
    Console.WriteLine("Incorrect IP address. Enter the right IP address.");
    return;
}

if (!int.TryParse(args[1], out var port))
{
    Console.WriteLine("Incorrect port. Enter the port that is not busy and is working");
}

var server = new Server(ip, port);
await server.Run();