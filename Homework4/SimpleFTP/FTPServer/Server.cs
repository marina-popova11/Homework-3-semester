// <copyright file="Server.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace FTPServer;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

/// <summary>
/// Class for ftp server.
/// </summary>
public class Server
{
    private TcpListener listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="address">IP address for access.</param>
    /// <param name="port">The port number we want to connect to.</param>
    public Server(IPAddress address, int port)
    {
        this.listener = new TcpListener(address, port);
        this.listener.Start();
    }

    /// <summary>
    /// Starts the server operation.
    /// </summary>
    /// <returns>Completed task.</returns>
    public Task Run()
    {
        while (true)
        {
            var client = this.listener.AcceptTcpClient();
            Task.Run(() => this.HandleClient(client));
        }
    }

    /// <summary>
    /// Handles client requests.
    /// </summary>
    /// <param name="client">Tcp client.</param>
    /// <returns>Completed task.</returns>
    public async Task HandleClient(TcpClient client)
    {
        using (client)
        using (var stream = client.GetStream())
        using (var reader = new StreamReader(stream))
        using (var writer = new StreamWriter(stream))
        {
            try
            {
                while (client.Connected)
                {
                    var request = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(request))
                    {
                        continue;
                    }

                    await this.ProcessRequest(request, client, writer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Starts the process of listening to the request.
    /// </summary>
    /// <param name="request">The request from client.</param>
    /// <param name="client">Tcp client.</param>
    /// <param name="writer">the writer to record the results.</param>
    /// <returns>completed task.</returns>
    public async Task ProcessRequest(string request, TcpClient client, StreamWriter writer)
    {
        var parts = request.Split(' ');
        var command = parts[0];
        var path = parts[1];

        switch (command)
        {
            case "1":
                await this.HandlerList(path, writer);
                break;
            case "2":
                await this.HandlerGet(path, client, writer);
                break;
            default:
                await writer.WriteLineAsync("Command error!");
                break;
        }
    }

    /// <summary>
    /// Processes the get request.
    /// </summary>
    /// <param name="path">the path to file/directory.</param>
    /// <param name="client">Tcp client.</param>
    /// <param name="writer">the writer to record the results.</param>
    /// <returns>completed task.</returns>
    public async Task HandlerGet(string path, TcpClient client, StreamWriter writer)
    {
        try
        {
            if (!File.Exists(path))
            {
                await writer.WriteAsync("-1");
                await writer.FlushAsync();
                return;
            }

            var fileInfo = new FileInfo(path);
            var size = fileInfo.Length.ToString();
            await writer.WriteLineAsync(size);
            await writer.FlushAsync();

            using (var fileStream = File.OpenRead(path))
            using (var netStream = client.GetStream())
            {
                await fileStream.CopyToAsync(netStream);
            }

            byte[] content = await File.ReadAllBytesAsync(path);
            await writer.WriteLineAsync($"{content}");
        }
        catch (Exception ex)
        {
            await writer.WriteLineAsync($"Get error: {ex.Message}");
            await writer.FlushAsync();
        }
    }

    /// <summary>
    /// Processes the list request.
    /// </summary>
    /// <param name="path">the path to file/directory.</param>
    /// <param name="writer">the writer to record the results.</param>
    /// <returns>completed task.</returns>
    public async Task HandlerList(string path, StreamWriter writer)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                await writer.WriteAsync("-1");
                await writer.FlushAsync();
                return;
            }

            var files = Directory.GetFileSystemEntries(path);
            await writer.WriteLineAsync(files.Length.ToString());
            await writer.FlushAsync();
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                var isDir = (File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory;
                var size = isDir ? info.Length : -1;

                await writer.WriteLineAsync($"({Path.GetFileName(file)} {isDir})\n");
                await writer.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            await writer.WriteLineAsync($"List error: {ex.Message}");
            await writer.FlushAsync();
        }
    }
}