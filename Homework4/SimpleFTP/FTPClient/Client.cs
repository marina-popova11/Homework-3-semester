// <copyright file="Client.cs" company="_">
// Marina Popova, 2025, under MIT License.
// </copyright>

namespace FTPClient;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

/// <summary>
/// Class for ftp client,
/// you can listing files and
/// get a file from server.
/// </summary>
public class Client
{
    private TcpClient client;
    private Stream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private bool isConnected;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    public Client()
    {
        this.client = new TcpClient();
        this.isConnected = false;
    }

    /// <summary>
    /// Gets flag for isConnected.
    /// </summary>
    /// <returns>True or false.</returns>
    public bool IsConnect() => this.isConnected;

    /// <summary>
    /// Connects the client to the server.
    /// </summary>
    /// <param name="ip">Ip address for connect to.</param>
    /// <param name="port">Port number for connect to.</param>
    /// <returns>Completed task.</returns>
    public async Task Connect(IPAddress ip, int port)
    {
        await this.client.ConnectAsync(ip, port);
        this.stream = this.client.GetStream();
        this.writer = new StreamWriter(this.stream);
        this.reader = new StreamReader(this.stream);
        this.isConnected = true;
    }

    /// <summary>
    /// Viewing files in a directory on the server.
    /// </summary>
    /// <param name="path">What the file will be viewed.</param>
    /// <returns>List of files in directory.</returns>
    public async Task<List<MyFileInfo>> CommandList(string path)
    {
        if (!this.IsConnect())
        {
            await this.writer.WriteLineAsync("Client does not connect!");
            return new List<MyFileInfo>();
        }

        await this.writer.WriteLineAsync($"1 {path}");
        await this.writer.FlushAsync();
        var response = await this.reader.ReadLineAsync();
        if (response == "-1")
        {
            return new List<MyFileInfo>();
        }

        var parts = response!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (!int.TryParse(parts[0], out var size) || size < 0)
        {
            return new List<MyFileInfo>();
        }

        var list = new List<MyFileInfo>();
        var index = 1;
        for (int i = 0; i < size; ++i)
        {
            if (index + 1 >= parts.Length)
            {
                break;
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
    /// <returns>True if file was successfully downloaded, false overwise.</returns>
    public async Task<bool> CommandGet(string path, string localPath)
    {
        if (!this.IsConnect())
        {
            await this.writer.WriteLineAsync("Client does not connect!");
            return false;
        }

        await this.writer.WriteLineAsync($"2 {path}");
        await this.writer.FlushAsync();
        var response = await this.reader.ReadLineAsync();
        if (response == "-1")
        {
            return false;
        }

        if (!long.TryParse(response, out var size) || size < 0)
        {
            return false;
        }

        using (var localFile = File.Create(localPath))
        {
            var buffer = new byte[4096];
            var allBytes = 0;
            while (allBytes < size)
            {
                var remainingBytes = await this.stream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, size - allBytes));
                await localFile.WriteAsync(buffer, 0, remainingBytes);
                allBytes += remainingBytes;
            }

            return allBytes == size;
        }
    }

    /// <summary>
    /// Terminates the client's work.
    /// </summary>
    /// <returns>Completed task.</returns>
    public async Task Disconnect()
    {
        this.reader.Close();
        this.writer.Close();
        this.stream.Close();
        this.client.Close();
    }
}