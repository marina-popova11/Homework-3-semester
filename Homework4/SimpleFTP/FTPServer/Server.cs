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
    public async Task Run()
    {
        while (true)
        {
            var client = this.listener.AcceptTcpClient();
            _ = Task.Run(async () => this.HandleClient(client));
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
            var stream = client.GetStream();
            if (!File.Exists(path))
            {
                await writer.WriteAsync("-1");
                await writer.FlushAsync();
                return;
            }

            var fileInfo = new FileInfo(path);
            long size = fileInfo.Length;
            await writer.WriteAsync($"{size} ");
            await writer.FlushAsync();

            using (var fileStream = File.OpenRead(path))
            {
                var buffer = new byte[4096];
                var allBytes = 0;
                while ((allBytes = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await stream.WriteAsync(buffer, 0, allBytes);
                }

                await stream.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get error: {ex.Message}");
            await writer.WriteAsync("-1");
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
            var responseParts = new List<string> { files.Length.ToString() };
            foreach (var file in files)
            {
                var isDir = (File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory;
                responseParts.Add(file);
                responseParts.Add(isDir.ToString().ToLower());
            }

            var response = string.Join(' ', responseParts);
            await writer.WriteLineAsync(response);
            await writer.FlushAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"List error: {ex.Message}");
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
        }
    }
}