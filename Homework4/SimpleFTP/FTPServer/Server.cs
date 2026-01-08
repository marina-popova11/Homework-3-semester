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
public class Server : IAsyncDisposable
{
    private TcpListener listener;
    private List<Task> activeClients = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="address">IP address for access.</param>
    /// <param name="port">The port number we want to connect to.</param>
    public Server(IPAddress address, int port)
    {
        this.listener = new TcpListener(address, port);
    }

    /// <summary>
    /// Starts the server operation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Completed task.</returns>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        this.listener.Start();

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient? client = null;

                try
                {
                    client = await this.listener.AcceptTcpClientAsync(cancellationToken);
                    var clientTask = this.HandleClientAsync(client, cancellationToken);
                    lock (this.activeClients)
                    {
                        this.activeClients.Add(clientTask);
                    }

                    _ = clientTask.ContinueWith(
                        t =>
                    {
                        lock (this.activeClients)
                        {
                            this.activeClients.Remove(t);
                        }
                    },
                        TaskScheduler.Default);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                    client?.Close();
                }
            }
        }
        finally
        {
            await this.WaitForClientsToCompleteAsync();

            this.listener.Stop();
            Console.WriteLine("Server stopped.");
        }
    }

    /// <summary>
    /// Handles client requests.
    /// </summary>
    /// <param name="client">Tcp client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Completed task.</returns>
    public async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken = default)
    {
        using (client)
        using (var stream = client.GetStream())
        using (var reader = new StreamReader(stream))
        using (var writer = new StreamWriter(stream))
        {
            try
            {
                while (client.Connected && !cancellationToken.IsCancellationRequested)
                {
                    var request = await reader.ReadLineAsync(cancellationToken);
                    if (string.IsNullOrEmpty(request))
                    {
                        continue;
                    }

                    await this.ProcessRequest(request, client, writer, cancellationToken);
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
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>completed task.</returns>
    public async Task ProcessRequest(string request, TcpClient client, StreamWriter writer, CancellationToken cancellationToken = default)
    {
        var parts = request.Split(' ');
        if (parts.Length < 2)
        {
            await writer.WriteLineAsync("Command error: invalid format");
            await writer.FlushAsync();
            return;
        }

        var command = parts[0];
        var path = parts[1];

        switch (command)
        {
            case "1":
                await this.HandleListAsync(path, writer, cancellationToken);
                break;
            case "2":
                await this.HandleGetAsync(path, client, writer, cancellationToken);
                break;
            default:
                await writer.WriteLineAsync("Command error!");
                await writer.FlushAsync();
                break;
        }
    }

    /// <summary>
    /// Processes the get request.
    /// </summary>
    /// <param name="path">the path to file/directory.</param>
    /// <param name="client">Tcp client.</param>
    /// <param name="writer">the writer to record the results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>completed task.</returns>
    public async Task HandleGetAsync(string path, TcpClient client, StreamWriter writer, CancellationToken cancellationToken = default)
    {
        try
        {
            var stream = client.GetStream();
            if (!File.Exists(path))
            {
                var messageEr = "-1" + Environment.NewLine;
                await writer.WriteAsync(messageEr.AsMemory(), cancellationToken);
                await writer.FlushAsync();
                return;
            }

            var fileInfo = new FileInfo(path);
            long size = fileInfo.Length;
            var message = $"{size} " + Environment.NewLine;
            await writer.WriteAsync(message.AsMemory(), cancellationToken);
            await writer.FlushAsync();

            using (var fileStream = File.OpenRead(path))
            {
                var buffer = new byte[4096];
                var allBytes = 0;
                while ((allBytes = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await stream.WriteAsync(buffer, 0, allBytes, cancellationToken);
                }

                await stream.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get error: {ex.Message}");
            var messageEr = "-1" + Environment.NewLine;
            await writer.WriteLineAsync(messageEr.AsMemory(), cancellationToken);
            await writer.FlushAsync();
        }
    }

    /// <summary>
    /// Processes the list request.
    /// </summary>
    /// <param name="path">the path to file/directory.</param>
    /// <param name="writer">the writer to record the results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>completed task.</returns>
    public async Task HandleListAsync(string path, StreamWriter writer, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                var messageEr = "-1" + Environment.NewLine;
                await writer.WriteAsync(messageEr.AsMemory(), cancellationToken);
                await writer.FlushAsync();
                return;
            }

            var files = Directory.GetFileSystemEntries(path);
            var responseParts = new List<string> { files.Length.ToString() };
            foreach (var file in files)
            {
                var isDir = (File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory;
                responseParts.Add(Path.GetFileName(file));
                responseParts.Add(isDir.ToString().ToLower());
            }

            var response = string.Join(' ', responseParts);
            var message = response + Environment.NewLine;
            await writer.WriteLineAsync(message.AsMemory(), cancellationToken);
            await writer.FlushAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"List error: {ex.Message}");
            var messageEr = "-1" + Environment.NewLine;
            await writer.WriteLineAsync(messageEr.AsMemory(), cancellationToken);
            await writer.FlushAsync();
        }
    }

    /// <summary>
    /// Disposes the server resources.
    /// </summary>
    /// <returns>A value task representing the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        this.listener.Stop();

        await this.WaitForClientsToCompleteAsync();
        GC.SuppressFinalize(this);
    }

    private async Task WaitForClientsToCompleteAsync()
    {
        Task[] tasks;
        lock (this.activeClients)
        {
            tasks = this.activeClients.ToArray();
        }

        if (tasks.Length > 0)
        {
            Console.WriteLine($"Waiting for {tasks.Length} client(s) to finish...");
            await Task.WhenAll(tasks);
        }
    }
}