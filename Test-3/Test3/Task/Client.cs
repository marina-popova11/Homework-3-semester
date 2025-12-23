// <copyright file="Client.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace Test3;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Class for client.
/// </summary>
public class Client
{
    /// <summary>
    /// Runs client.
    /// </summary>
    /// <param name="serverIp">IP of server.</param>
    /// <param name="port">port.</param>
    /// <returns>Result of connecting.</returns>
    public static async Task StartClientAsync(IPAddress serverIp, int port)
    {
        using var client = new TcpClient();
        try
        {
            await client.ConnectAsync(serverIp, port);
            Console.WriteLine($"Client was connected to server {serverIp}:{port}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Couldn't connect: {ex.Message}");
        }

        await Chat.CreateChatAsync(client);
    }
}