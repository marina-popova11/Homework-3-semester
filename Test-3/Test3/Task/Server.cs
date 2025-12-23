// <copyright file="Server.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test3;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class for server.
/// </summary>
public static class Server
{
    /// <summary>
    /// Runs server.
    /// </summary>
    /// <param name="port">port.</param>
    /// <returns>the result of connecting.</returns>
    public static async Task StartServerAsync(int port)
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server running on port {port}.");

        using var client = await listener.AcceptTcpClientAsync();
        listener.Stop();
        Console.WriteLine("Client was connected.");

        await Chat.CreateChatAsync(client);
    }
}