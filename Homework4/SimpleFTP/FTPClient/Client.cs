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
public class Client
{
    private TcpClient client;
    private Stream stream = null!;
    private StreamWriter writer = null!;
    private StreamReader reader = null!;
    private bool isConnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="ip">Ip address for connect to.</param>
    /// <param name="port">Port number for connect to.</param>
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
    /// <exception cref="InvalidOperationException">If client does not connect/ directory not found/ size is not correct/
    /// server return incomplete response.</exception>
    public async Task<List<MyFileInfo>> CommandList(string path)
    {
        if (!this.IsConnect())
        {
            throw new InvalidOperationException("Client does not connect!");
        }

        await this.writer.WriteLineAsync($"1 {path}");
        await this.writer.FlushAsync();
        var response = await this.reader.ReadLineAsync();
        if (response == "-1")
        {
            throw new InvalidOperationException($"Directory not found at path: {path}");
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
    /// <returns>True if file was successfully downloaded, false overwise.</returns>
    /// <exception cref="InvalidOperationException">If client does not connect/ server disconnected before sending size/
    /// file not found.</exception>
    public async Task<bool> CommandGet(string path, string localPath)
    {
        if (!this.IsConnect())
        {
            throw new InvalidOperationException("Client does not connect!");
        }

        await this.writer.WriteLineAsync($"2 {path}");
        await this.writer.FlushAsync();
        var size = new StringBuilder();
        while (true)
        {
            int b = this.stream.ReadByte();
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
                    throw new IOException("Connect close before file was fully.");
                }

                await localFile.WriteAsync(buffer, 0, bytesRead);
                allBytes += remainingBytes;
            }

            await localFile.FlushAsync();
            return allBytes == fileSize;
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