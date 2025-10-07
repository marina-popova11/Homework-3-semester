// <copyright file="Client.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace FTPClient;

using System.Net.Sockets;

/// <summary>
/// Class for ftp client,
/// you can listing files and
/// get a file from server.
/// </summary>
public class Client : IDisposable
{
    private TcpClient client;
    private bool isDispose;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    public Client()
    {
        this.client = new TcpClient();
    }

    /// <summary>
    /// Terminates the client's work.
    /// </summary>
    public void Dispose()
    {
        this.client.Dispose();
    }
}