// <copyright file="Program.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

using System.Net;
using System.Net.Sockets;
using Test3;

if (args.Length == 1 && int.TryParse(args[0], out int port))
{
    await Server.StartServerAsync(port);
}
else if (args.Length == 2 && IPAddress.TryParse(args[0], out IPAddress? ip) && int.TryParse(args[1], out port))
{
    await Client.StartClientAsync(ip, port);
}
else
{
    Console.WriteLine("Process: ");
    return;
}