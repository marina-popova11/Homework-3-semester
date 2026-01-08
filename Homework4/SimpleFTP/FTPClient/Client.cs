// <copyright file="Client.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace FTPClient;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class for ftp client,
/// you can listing files and
/// get a file from server.
/// </summary>
public class Client : IAsyncDisposable
{
    private TcpClient? client;
    private Stream? stream;
    private StreamWriter? writer;
    private StreamReader? reader;
    private bool isConnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="ip">Ip address for connect to.</param>
    /// <param name="port">Port number for connect to.</param>
    public Client()
    {
        this.client = new TcpClient(AddressFamily.InterNetwork);
        this.isConnected = false;
    }

    /// <summary>
    /// Gets a value indicating whether the client is connected.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsConnect => this.isConnected;

    /// <summary>
    /// Connects the client to the server.
    /// </summary>
    /// <param name="ip">Ip address for connect to.</param>
    /// <param name="port">Port number for connect to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Completed task.</returns>
    public async Task ConnectAsync(IPAddress ip, int port, CancellationToken cancellationToken = default)
    {
        await this.client!.ConnectAsync(ip, port, cancellationToken);
        this.stream = this.client.GetStream();
        this.writer = new StreamWriter(this.stream, Encoding.UTF8, bufferSize: 4096, leaveOpen: true);
        this.reader = new StreamReader(this.stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true);
        this.isConnected = true;
    }

    /// <summary>
    /// Viewing files in a directory on the server.
    /// </summary>
    /// <param name="path">What the file will be viewed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of files in directory.</returns>
    /// <exception cref="InvalidOperationException">If client does not connect/ size is not correct/
    /// server return incomplete response.</exception>
    /// <exception cref="DirectoryNotFoundException">If directory not found.</exception>
    public async Task<List<MyFileInfo>> CommandListAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!this.IsConnect)
        {
            throw new InvalidOperationException("Client is not connected!");
        }

        var message = $"1 {path}" + Environment.NewLine;
        await this.writer!.WriteAsync(message.AsMemory(), cancellationToken);
        await this.writer.FlushAsync();
        var response = await this.reader!.ReadLineAsync(cancellationToken);
        if (response == "-1")
        {
            throw new DirectoryNotFoundException($"Directory not found at path: {path}");
        }

        var parts = response!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (!int.TryParse(parts[0], out var size) || size < 0)
        {
            throw new InvalidOperationException("Incorrect size.");
        }

        var list = new List<MyFileInfo>();
        var index = 1;
        for (int i = 0; i < size; ++i)
        {
            if (index + 1 >= parts.Length)
            {
                throw new InvalidOperationException("Server returned incomplete response.");
            }

            var name = parts[index++];
            if (!bool.TryParse(parts[index++], out var isDir))
            {
                continue;
            }

            list.Add(new MyFileInfo
            {
                Name = name,
                IsDir = isDir,
            });
        }

        return list;
    }

    /// <summary>
    /// Downloading a file from server.
    /// </summary>
    /// <param name="path">What the file will be copy.</param>
    /// <param name="localPath">Where file will be download to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if file was successfully downloaded, false overwise.</returns>
    /// <exception cref="InvalidOperationException">If client does not connect/ server disconnected before sending size/
    /// file not found.</exception>
    public async Task<bool> CommandGetAsync(string path, string localPath, CancellationToken cancellationToken = default)
    {
        if (!this.IsConnect)
        {
            throw new InvalidOperationException("Client is not connected!");
        }

        var message = $"2 {path}" + Environment.NewLine;
        await this.writer!.WriteAsync(message.AsMemory(), cancellationToken);
        await this.writer.FlushAsync();
        var size = new StringBuilder();
        while (true)
        {
            int b = this.stream!.ReadByte();
            if (b == -1)
            {
                throw new InvalidOperationException($"Server disconnected before sending size.");
            }

            if (b == ' ')
            {
                break;
            }

            size.Append((char)b);
        }

        var sizeString = size.ToString();
        if (sizeString == "-1")
        {
            throw new InvalidOperationException($"File not found at path: {path}");
        }

        if (!long.TryParse(sizeString, out var fileSize) || fileSize < 0)
        {
            return false;
        }

        using (var localFile = File.Create(localPath))
        {
            var buffer = new byte[4096];
            var allBytes = 0;
            while (allBytes < fileSize)
            {
                var remainingBytes = (int)Math.Min(buffer.Length, fileSize - allBytes);
                var bytesRead = await this.stream.ReadAsync(buffer, 0, remainingBytes);
                if (bytesRead == 0)
                {
                    throw new IOException("The connection was lost before the file was fully downloaded.");
                }

                await localFile.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                allBytes += remainingBytes;
            }

            await localFile.FlushAsync(cancellationToken);
            return allBytes == fileSize;
        }
    }

    /// <summary>
    /// Terminates the client's work.
    /// </summary>
    /// <returns>Completed task.</returns>
    public ValueTask Disconnect()
    {
        // if (!this.isConnected)
        // {
        //     return;
        // }

        // this.reader?.Dispose();
        // this.writer?.Dispose();
        // this.stream?.Dispose();
        // this.client?.Close();

        // this.reader = null;
        // this.writer = null;
        // this.stream = null;
        // this.client = null;
        // this.isConnected = false;
        if (!this.isConnected)
        {
            return ValueTask.CompletedTask;
        }

        this.reader?.Dispose();
        this.writer?.Dispose();
        this.client?.Close();

        this.reader = null;
        this.writer = null;
        this.stream = null;
        this.client = null;
        this.isConnected = false;

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Implementation of IAsyncDisposable.
    /// </summary>
    /// <returns>A ValueTask that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await this.Disconnect();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The class for file information.
    /// </summary>
    public class MyFileInfo
    {
        /// <summary>
        /// Gets or sets the name of file.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether.
        /// </summary>
        public bool IsDir { get; set; } = false;
    }
}